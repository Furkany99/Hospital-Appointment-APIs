using AutoMapper;
using Common.Dto;
using Common.Models;
using Common.Models.ResponseModels.Appointment;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Services
{
	public class AppointmentService
	{
		private readonly HospitalAppointmentContext _context;
		private readonly DoctorService _doctorService;
		private readonly IMapper _mapper;

		public AppointmentService(HospitalAppointmentContext Context, IMapper mapper,DoctorService doctorService)
		{
			_context = Context;
			_doctorService = doctorService;
			_mapper = mapper;

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
			var appointment = _context.Appointments
							.Include(a => a.Status)
							.FirstOrDefault(a => a.Id == id &&
												 (a.StatusId == 14 || a.StatusId == 16));

			if (appointment.StatusId == 14)
			{
				throw new InvalidOperationException("Tamamlanan bir randevuyu iptal edemezsiniz.");
			}

			if (appointment.Date < DateTime.Today)
			{
				throw new InvalidOperationException("Geçmiş bir randevuyu iptal edemezsiniz.");
			}

			appointment.StatusId = 16;
			_context.SaveChanges();
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

			return false;
		}

		public List<AppointmentDto> GetPatientAppointments(AppointmentQueryParameter queryParameter)
		{
			DateTime today = DateTime.Today;

			var appointmentsQuery = _context.Appointments
			.Include(a => a.Status)
			.Include(a => a.AppointmentTimes)
			.Where(a => a.PatientId == queryParameter.PatientId &&
				a.Status.Id == queryParameter.StatusId &&
				a.DocId == queryParameter.DoctorId &&
				a.Department.Id == queryParameter.DepartmentId &&
				!queryParameter.startDate.HasValue || !queryParameter.endDate.HasValue || (a.Date >= queryParameter.startDate && a.Date <= queryParameter.endDate) &&
				queryParameter.startDate.HasValue && queryParameter.endDate.HasValue || a.Date >= today).ToList();

			var appointmentList = appointmentsQuery.Select(appointment => _mapper.Map<AppointmentDto>(appointment)).ToList();

			return appointmentList;
		}

		public List<AppointmentAvailabilityDto> GetDoctorAppointmentsAndSchedules(int departmentId ,DateOnly? startDate, DateOnly? endDate)
		{
			var doctors = _context.Doctors
			.Include(a => a.Departments)
			.Where(d => d.Departments.Any(a => a.Id == departmentId))
			.ToList();

			var doctorAppointmentsAndSchedules = new List<AppointmentAvailabilityDto>();

			foreach (var doctor in doctors)
			{
				var routinesAndOneTimes = _doctorService.GetDoctorRoutinesAndOneTimes(doctor.Id, startDate, endDate);
				var hasAppointments = HasDoctorAppointments(doctor.Id, startDate, endDate);
				

				var doctorAvailability = new AppointmentAvailabilityDto
				{
					DoctorId = doctor.Id,
					RoutinesAndOneTimes = routinesAndOneTimes,
					Appointments = hasAppointments ? GetPatientAppointments(new AppointmentQueryParameter
					{
						StatusId = 15,
						DoctorId = doctor.Id,
						DepartmentId = departmentId,
						startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day),
						endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day),
					}).Select(appointment => new AppointmentSimpleResponseBody
					{
						AppointmentId = appointment.Id,
						AppointmentDate = appointment.Date,
						TimeSlots = appointment.appointmentTimes
						.Select(a => new AppointmentResponseTimeModel
						{
							StartTime = a.StartTime,
							EndTime = a.EndTime
						})
						.ToList()
					}).ToList() : new List<AppointmentSimpleResponseBody>()
				};

				doctorAppointmentsAndSchedules.Add(doctorAvailability);
			}

			return doctorAppointmentsAndSchedules;
		}

		private bool HasDoctorAppointments(int doctorId, DateOnly? startDate, DateOnly? endDate)
		{
			
			var appointments = _context.Appointments
				.Where(a => a.DocId == doctorId &&
							a.Date >= new DateTime(startDate.Value.Year,startDate.Value.Month,startDate.Value.Day) && 
							a.Date <= new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day))
				.ToList();

			return appointments.Any();
		}

	}
}