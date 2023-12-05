using AutoMapper;
using Common.Dto;
using Common.Models;
using Common.Models.RequestModels.Patient;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

		public PatientDto Update(int id, PatientUpdateRequestModel patientUpdate)
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

			Status defaultStatus = _context.Statuses.FirstOrDefault(s => s.Id == 15);

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

			if (defaultStatus != null)
			{
				appointment.StatusId = defaultStatus.Id;
			}
			else
			{
				throw new InvalidOperationException("Varsayılan statü bulunamadı!");
			}

			bool isDoctorAvailable = IsDoctorAvailableForAppointment(appointmentDto.DocId, appointmentDto.Date, appointmentDto.appointmentTimes.Select(s => s.StartTime).FirstOrDefault(), appointmentDto.appointmentTimes.Select(s => s.EndTime).FirstOrDefault());

			if (!isDoctorAvailable)
			{
				throw new InvalidOperationException("Randevu oluşturulamadı. Doktor müsait değil.");
			}

			appointment.PatientId = appointmentDto.PatientId;

			_context.Appointments.Add(appointment);
			_context.SaveChanges();
		}

		public void DeleteAppointment(int id)
		{
			var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);

			Status cancelledStatus = _context.Statuses.FirstOrDefault(a => a.Id == 16);
			Status completedStatus = _context.Statuses.FirstOrDefault(s => s.Id == 14);

			if (appointment != null && cancelledStatus != null && completedStatus != null)
			{
				if (appointment.StatusId == completedStatus.Id)
				{
					throw new InvalidOperationException("Tamamlanan bir randevuyu iptal edemezsiniz.");
				}

				if (appointment.Date < DateTime.Today)
				{
					throw new InvalidOperationException("Geçmiş bir randevuyu iptal edemezsiniz.");
				}
				else 
				{
					appointment.StatusId = cancelledStatus.Id;
					_context.SaveChanges();
				}
					
			}
		}

		public bool IsDoctorAvailableForAppointment(int doctorId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
		{
			if (date < DateOnly.FromDateTime(DateTime.Today))
			{
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
					var hasAppointment = _context.Appointments.Any(a =>
					a.DocId == doctorId &&
					a.Date == new DateTime(date.Year, date.Month, date.Day) &&
					a.AppointmentTimes.Any(at =>
					(
						at.StartTime <= startTime.ToTimeSpan() && at.EndTime >= startTime.ToTimeSpan() ||
						at.StartTime <= endTime.ToTimeSpan() && at.EndTime >= endTime.ToTimeSpan() ||
						at.StartTime >= startTime.ToTimeSpan() && at.EndTime <= endTime.ToTimeSpan()
					)
				)
			);

					return !hasAppointment;
				}

				return false;

			}

			return false; // Belirtilen tarih doktorun rutin günleri arasında değilse
		}

		public List<AppointmentDto> GetPatientAppointments(int patientId, AppointmentQueryParameter queryParameter)
		{
			DateTime today = DateTime.Today;

			var appointmentsQuery = _context.Appointments
			.Include(a => a.Status)
			.Include(a => a.AppointmentTimes)
			.Where(a => a.PatientId == patientId &&
				a.Status.Id == queryParameter.StatusId &&
				a.DocId == queryParameter.DoctorId &&
				a.Department.Id == queryParameter.DepartmentId &&
				!queryParameter.startDate.HasValue || !queryParameter.endDate.HasValue || (a.Date >= queryParameter.startDate && a.Date <= queryParameter.endDate) &&
				queryParameter.startDate.HasValue && queryParameter.endDate.HasValue || a.Date >= today).ToList();

			var appointmentList = appointmentsQuery.Select(appointment => _mapper.Map<AppointmentDto>(appointment)).ToList();

			return appointmentList;
		}

	}
}