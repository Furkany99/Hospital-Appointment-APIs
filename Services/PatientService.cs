using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels.Appointment;
using Common.Models.RequestModels.Patient;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PatientService
	{
		private readonly HospitalAppointmentContext _context;
		private readonly IMapper _mapper;

		public PatientService(HospitalAppointmentContext Context, IMapper mapper)
        {
			_context = Context;
			_mapper = mapper;
		}

		public void CreatePatient(PatientDto patientDto)
		{
			
			Patient patient = _mapper.Map<Patient>(patientDto);
			Account account = _mapper.Map<Account>(patientDto);

			patient.Account = account;

			_context.Patients.Add(patient);
			_context.Accounts.Add(account);

			_context.SaveChanges();




		}

		public PatientDto Update(int id,PatientUpdateRequestModel patientUpdate) 
		{
			var existingPatient = _context.Patients.Find(id);

			if (existingPatient != null && patientUpdate != null)
			{
				existingPatient.Name = patientUpdate.Name;
				existingPatient.Surname = patientUpdate.Surname;

				_context.SaveChanges();

				PatientDto updatedPatientDto = _mapper.Map<PatientDto>(existingPatient);
				return updatedPatientDto;

			}

			else
			{
				throw new KeyNotFoundException("Hasta bulunamadı veya güncelleme verisi eksik.");
			}

		}

		public void Delete(int id) 
		{

			var existingPatient = _context.Patients.Find(id);

			if (existingPatient != null)
			{
				_context.Patients.Remove(existingPatient);
				_context.SaveChanges();

			}
			
		}

		public List<PatientDto> GetPatients() 
		{
			var patients = _context.Patients.Include(p => p.Account).ToList();
			var patientsList = patients.Select(patient => _mapper.Map<PatientDto>(patient)).ToList();
			return patientsList;
			
		}


		public PatientDto GetPatientById(int id) 
		{
			var patientById = _context.Patients
			.Include(x => x.Account)
			.FirstOrDefault(x => x.Id == id);
			if (patientById == null)
			{
				throw new KeyNotFoundException("Hasta bulunamadı");
			}

			var patientDto = _mapper.Map<PatientDto>(patientById);

			return patientDto;
		}

		public void CreateAppointment(AppointmentDto appointmentDto)
		{
			Appointment appointment = _mapper.Map<Appointment>(appointmentDto);
			Status status = _mapper.Map<Status>(appointmentDto);

			DateTime now = DateTime.Now;

			var doctorOnLeaveDays = _context.OneTimes
				.Where(ot => ot.DoctorId == appointmentDto.DocId && ot.IsOnLeave)
				.Select(ot => DateOnly.FromDateTime(ot.Day))
				.ToList();

			DateOnly appointmentDate = appointmentDto.Date;

			if (doctorOnLeaveDays.Contains(appointmentDate))
			{
				throw new InvalidOperationException("Randevu oluşturulamadı. Doktor izinli olduğu bir tarih için randevu alınamaz.");
			}

			if (appointment.Date < now.Date)
			{
				throw new InvalidOperationException("Randevu oluşturulamadı. Geçmiş bir tarih için randevu alınamaz.");
			}

			if (appointment.Date < now)
			{
				status.Name = "Randevu Tamamlandı";
			}
			else if (appointment.Date > now)
			{
				status.Name = "Beklemede";
			}
			else
			{
				status.Name = "Randevu İptal Edildi";
			}

			bool isDoctorAvailable = IsDoctorAvailableForAppointment(appointmentDto.DocId, appointmentDto.Date, appointmentDto.appointmentTimes.Select(s => s.StartTime).First(), appointmentDto.appointmentTimes.Select(s => s.EndTime).First());

			if (!isDoctorAvailable)
			{
				throw new InvalidOperationException("Randevu oluşturulamadı. Doktor müsait değil.");
			}

			appointment.PatientId = appointmentDto.PatientId;
			appointment.Status = status;

			_context.Statuses.Add(status);
			_context.Appointments.Add(appointment);
			_context.SaveChanges();
		}


		public void DeleteAppointment(int id)
		{
			var appoinment = _context.Appointments.Find(id);

			if (appoinment != null)
			{
				_context.Appointments.Remove(appoinment);
				_context.SaveChanges();
			}
		}


		public bool IsDoctorAvailableForAppointment(int doctorId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
		{
			if (date < DateOnly.FromDateTime(DateTime.Today))
			{
				// Geçmiş bir tarih için randevu alınamaz
				return false;
			}

			var doctorRoutine = _context.Routines
				.Include(r => r.TimeBlocks)
				.FirstOrDefault(r => r.DoctorId == doctorId && (int)r.DayOfWeek == (int)date.DayOfWeek);

			if (doctorRoutine != null)
			{
				var isWithinTimeBlocks = doctorRoutine.TimeBlocks.Any(tb => startTime.ToTimeSpan() >= tb.StartTime && endTime.ToTimeSpan() <= tb.EndTime);

				if (isWithinTimeBlocks)
				{
					// Doktorun rutin saatleri içinde ise, randevu alabilir
					return true;
				}

				// Doktorun rutin saatleri içinde değilse, randevularına da bakarak kontrol et
				var hasAppointment = _context.Appointments.Any(a => a.DocId == doctorId && DateOnly.FromDateTime(a.Date) == date && a.AppointmentTimes.Any(at => at.StartTime <= startTime.ToTimeSpan() && at.EndTime >= endTime.ToTimeSpan()));

				return !hasAppointment;
			}

			return false; // Belirtilen tarih doktorun rutin günleri arasında değilse
		}


		public List<AppointmentDto> GetPatientAppointments(int patientId)
		{
			DateTime today = DateTime.Today;

			var appointments = _context.Appointments.Include(a => a.Status).Include(x => x.AppointmentTimes)
				.Where(a => a.PatientId == patientId)
				.ToList();

			foreach (var appointment in appointments)
			{
				if (appointment.Date < today)
				{
					if (appointment.Status.Name == "Beklemede")
					{
						appointment.Status.Name = "Randevu Tamamlandı";
					}
					else if (appointment.Status.Name != "Randevu Tamamlandı")
					{
						appointment.Status.Name = "Randevu İptal Edildi";
					}

				}
			}
			_context.SaveChanges();

			var appointmentList = appointments.Select(appointment => _mapper.Map<AppointmentDto>(appointment)).ToList();

			return appointmentList;
		}

		public AppointmentDto UpdateAppointment(int Appointmentid, AppointmentUpdateRequestModel updateRequestModel)
		{
			var PatientAppointment = _context.Patients
			.Include(p => p.Appointments).ThenInclude(a => a.Status)
			.FirstOrDefault(p => p.Appointments.Any(a => a.Id == Appointmentid));

			if (PatientAppointment == null || updateRequestModel == null)
			{
				return null;
			}

			var appointmentToUpdate = PatientAppointment.Appointments.FirstOrDefault(a => a.Id == Appointmentid);

			if (appointmentToUpdate == null || appointmentToUpdate.Status.Name == "Randevu Tamamlandı" || updateRequestModel.Date < DateOnly.FromDateTime(DateTime.Today))
			{
				return null;
			}

			var existingTimeBlocks = _context.AppointmentTimes.Where(t => t.AppointmentId == Appointmentid).ToList();
			_context.AppointmentTimes.RemoveRange(existingTimeBlocks);

			
			_mapper.Map(updateRequestModel, appointmentToUpdate);

			if (updateRequestModel.appointmentTimes != null)
			{
				foreach (var updatedTimeBlock in updateRequestModel.appointmentTimes)
				{
					var newTimeBlock = _mapper.Map<AppointmentTime>(updatedTimeBlock);
				}
			}

			_context.SaveChanges();

			AppointmentDto updatedAppointmentDto = _mapper.Map<AppointmentDto>(appointmentToUpdate);
			return updatedAppointmentDto;

		}


	}

	
}