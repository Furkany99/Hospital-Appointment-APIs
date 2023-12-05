using AutoMapper;
using Common.Dto;
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

			Status defaultStatus = _context.Statuses.FirstOrDefault(s => s.Name == "Beklemede");

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

		public List<AppointmentDto> GetPatientAppointments(int patientId, int statusId, int doctorId, int departmentId, DateTime? startTime = null, DateTime? endTime = null)
		{

			var appointmentsQuery = _context.Appointments
				.Include(a => a.Status)
				.Include(x => x.AppointmentTimes)
				.Where(a => a.PatientId == patientId);

			DateTime today = DateTime.Today;

			var pendingStatus = _context.Statuses.FirstOrDefault(s => s.Name == "Beklemede");
			var completedStatus = _context.Statuses.FirstOrDefault(s => s.Name == "Tamamlandi");
			var cancelledStatus = _context.Statuses.FirstOrDefault(s => s.Name == "Iptal edildi");

			
			if (statusId != 0)
			{
				appointmentsQuery = appointmentsQuery.Where(a => a.Status.Id == statusId);
			}

			if (doctorId != 0)
			{
				appointmentsQuery = appointmentsQuery.Where(a => a.DocId == doctorId);
			}

			if (departmentId != 0)
			{
				appointmentsQuery = appointmentsQuery.Where(a => a.Department.Id == departmentId);
			}

			if (startTime.HasValue && endTime.HasValue)
			{
				appointmentsQuery = appointmentsQuery.Where(a => a.Date >= startTime && a.Date <= endTime);
			}
			else
			{
				appointmentsQuery = appointmentsQuery.Where(a => a.Date >= today); // Sadece gelecekteki randevuları getir
			}

			var appointments = appointmentsQuery.ToList();

			var pastAppointments = _context.Appointments
			.Where(a => a.Date < today && a.Status != completedStatus && a.Status != cancelledStatus)
			.ToList();

			foreach (var appointment in pastAppointments)
			{
				appointment.Status = appointment.Status.Name == pendingStatus.Name ? completedStatus : cancelledStatus;
			}

			_context.SaveChanges();

			var appointmentList = appointments.Select(appointment => _mapper.Map<AppointmentDto>(appointment)).ToList();

			return appointmentList;
		}

	}
}