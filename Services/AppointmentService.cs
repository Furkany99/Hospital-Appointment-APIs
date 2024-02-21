using AutoMapper;
using Common.Dto;
using Common.Models;
using Common.Models.ResponseModels.Appointment;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using static Common.Exceptions.ExceptionHandlingMiddleware;

namespace Services
{
	public class AppointmentService
	{
		private readonly HospitalAppointmentContext _context;
		private readonly DoctorService _doctorService;
		private readonly IMapper _mapper;

		public AppointmentService(HospitalAppointmentContext Context, IMapper mapper, DoctorService doctorService)
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
				throw new BadRequestException("The appointment could not be created. Appointments cannot be made for a date when the doctor is on leave..");
			}

			if (appointment.Date < now.Date)
			{
				throw new BadRequestException("The appointment could not be created. Appointments cannot be made for a past date..");
			}

			if (defaultStatus != null)
			{
				appointment.StatusId = defaultStatus.Id;
			}
			else
			{
				throw new NotFoundException("Default status not found!");
			}

			bool isDoctorAvailable = IsDoctorAvailableForAppointment(appointmentDto.DocId, appointmentDto.Date, appointmentDto.appointmentTimes.Select(s => s.StartTime).FirstOrDefault(), appointmentDto.appointmentTimes.Select(s => s.EndTime).FirstOrDefault());

			if (!isDoctorAvailable)
			{
				throw new NotFoundException("The appointment could not be created. Doctor is not available.");
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
												 (a.StatusId == 14 || a.StatusId == 16 || a.StatusId == 15));

			if (appointment.StatusId == 14)
			{
				throw new BadRequestException("You cannot cancel a completed appointment.");
			}

			if (appointment.Date < DateTime.Today)
			{
				throw new BadRequestException("You cannot cancel a past appointment.");
			}

			if(appointment.StatusId == 17)
			{
				throw new BadRequestException("The patient did not come to the appointment. Canceled by doctor");
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
					a.Date == new DateTime(date.Year, date.Month, date.Day) && a.StatusId != 16 &&
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

		public List<AppointmentAvailabilityDto> GetDoctorAppointmentsAndSchedules(int departmentId, DateOnly? startDate, DateOnly? endDate)
		{
			var doctors = _context.Doctors
				.Include(a => a.Departments)
				.Where(d => d.Departments.Any(a => a.Id == departmentId))
				.ToList();

			var doctorAppointmentsAndSchedules = doctors.Select(doctor =>
			{
				var routinesAndOneTimes = _doctorService.GetDoctorRoutinesAndOneTimes(doctor.Id, startDate, endDate);
				var hasAppointments = HasDoctorAppointments(doctor.Id, startDate, endDate);

				return new AppointmentAvailabilityDto
				{
					DoctorId = doctor.Id,
					RoutinesAndOneTimes = routinesAndOneTimes
						.SelectMany(routine =>
						{
							var slots = new List<DateInfoTimeDto>();
							foreach (var dateInfo in routine.dateInfoTimeDtos)
							{
								GenerateTimeSlots(dateInfo.StartTime, dateInfo.EndTime, slots);
							}
							return slots.Select(slot => new DateInfoDto
							{
								DayOfWeek = routine.DayOfWeek,
								IsOnLeave = routine.IsOnLeave,
								Day = routine.Day,
								dateInfoTimeDtos = new List<DateInfoTimeDto> { slot }
							});
						})
						.ToList(),
					Appointments = hasAppointments
						? GetPatientAppointments(new AppointmentQueryParameter
						{
							StatusId = 15,
							DoctorId = doctor.Id,
							DepartmentId = departmentId,
							startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day),
							endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day),
						})
							.Select(appointment => new AppointmentSimpleResponseBody
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
							})
							.ToList()
						: new List<AppointmentSimpleResponseBody>()
				};
			}).ToList();

			return doctorAppointmentsAndSchedules;
		}

		private bool HasDoctorAppointments(int doctorId, DateOnly? startDate, DateOnly? endDate)
		{
			DateTime today = DateTime.Today;
			DateTime endDay = today.AddDays(5);

			if (!startDate.HasValue || !endDate.HasValue)
			{
				startDate = DateOnly.FromDateTime(today);
				endDate = DateOnly.FromDateTime(endDay.Date);
			}

			var appointments = _context.Appointments
				.Where(a => a.DocId == doctorId &&
							a.Date >= new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day) &&
							a.Date <= new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day))
				.ToList();

			return appointments.Any();
		}

		private void GenerateTimeSlots(TimeSpan startTime, TimeSpan endTime, List<DateInfoTimeDto> slots)
		{
			while (startTime < endTime)
			{
				slots.Add(new DateInfoTimeDto
				{
					StartTime = startTime,
					EndTime = startTime.Add(TimeSpan.FromMinutes(30))
				});
				startTime = startTime.Add(TimeSpan.FromMinutes(30));
			}
		}
	}
}