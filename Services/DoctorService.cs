﻿using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels.Doctor;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using static Common.Exceptions.ExceptionHandlingMiddleware;

namespace Services
{
	public class DoctorService
	{
		private readonly HospitalAppointmentContext context;
		private readonly IMapper _mapper;

		public DoctorService(HospitalAppointmentContext Context, IMapper mapper)
		{
			context = Context;
			_mapper = mapper;
		}

		public void CreateDoctor(DoctorDto doctorDto)
		{
			Account account = _mapper.Map<Account>(doctorDto);
			Doctor doctor = _mapper.Map<Doctor>(doctorDto);

			doctor.Account = account;

			if (doctorDto.DepartmentIds != null)
			{
				foreach (int departmentId in doctorDto.DepartmentIds)
				{
					Department department = context.Departments.Find(departmentId);
					if (department != null)
					{
						doctor.Departments.Add(department);
					}
				}
			}

			context.Doctors.Add(doctor);
			context.Accounts.Add(account);
			context.SaveChanges();
		}

		public DoctorDto UpdateDoctor(int id, DoctorUpdateRequestModel doctorUpdate)
		{
			var existingDoctor = context.Doctors.Find(id);

			if (existingDoctor == null && doctorUpdate == null)
			{
				throw new NotFoundException();
			}

			existingDoctor.Name = doctorUpdate.Name;
			existingDoctor.Surname = doctorUpdate.Surname;

			context.SaveChanges();

			DoctorDto doctorDto = _mapper.Map<DoctorDto>(existingDoctor);
			return doctorDto;
		}

		public DoctorDto UpdateDoctorDepartment(int id, List<int> departmentIds)
		{
			var existingDoctor = context.Doctors
				.Include(d => d.Departments)
				.FirstOrDefault(d => d.Id == id);

			if (existingDoctor == null)
			{
				throw new NotFoundException("Doctor ID not found: " + id);
			}

			if (existingDoctor != null && departmentIds != null)
			{
				existingDoctor.Departments.Clear();

				foreach (var departmentId in departmentIds)
				{
					var department = context.Departments.FirstOrDefault(d => d.Id == departmentId);
					if (department != null)
					{
						existingDoctor.Departments.Add(department);
					}
					else
					{
						throw new BadRequestException("Invalid department ID: " + departmentId);
					}
				}

				context.SaveChanges();

				DoctorDto doctorDto = _mapper.Map<DoctorDto>(existingDoctor);
				return doctorDto;
			}
			else
			{
				throw new NotFoundException();
			}
		}

		public DoctorDto UpdateDoctorTitle(int id, int titleId)
		{
			var existingDoctor = context.Doctors
				.Include(d => d.Title)
				.FirstOrDefault(d => d.Id == id);

			if (existingDoctor == null)
			{
				throw new NotFoundException("Doctor ID not found: " + id);
			}

			var newTitle = context.Titles.FirstOrDefault(t => t.Id == titleId);

			if (newTitle == null)
			{
				throw new BadRequestException("Invalid title ID: " + titleId);
			}

			existingDoctor.Title = newTitle;
			context.SaveChanges();

			DoctorDto doctorDto = _mapper.Map<DoctorDto>(existingDoctor);
			return doctorDto;
		}

		public void DeleteDoctor(int id)
		{
			var deleteDoctor = context.Doctors.Find(id);

			if (deleteDoctor == null)
			{
				throw new NotFoundException("Doctor not found!");
			}
			context.Doctors.Remove(deleteDoctor);
			context.SaveChanges();
		}

		public List<DoctorDto> GetDoctors()
		{
			var doctors = context.Doctors.Include(p => p.Account).Include(x => x.Title).Include(a => a.Departments).ToList();
			var doctorList = new List<DoctorDto>();

			foreach (var doctor in doctors)
			{
				var doctorDto = _mapper.Map<DoctorDto>(doctor);
				doctorList.Add(doctorDto);
			}

			return doctorList;
		}

		public DoctorDto GetDoctorById(int id)
		{
			var doctorById = context.Doctors
			.Include(x => x.Account).Include(a => a.Title).Include(a => a.Departments)
			.FirstOrDefault(x => x.Id == id);
			if (doctorById == null)
			{
				throw new NotFoundException("Doctor not found!");
			}

			var doctorDto = _mapper.Map<DoctorDto>(doctorById);

			return doctorDto;
		}

		public void CreateRoutine(int doctorId, RoutineDto routineDto)
		{
			var doctor = context.Doctors
		.Include(d => d.Routines)
		.ThenInclude(r => r.TimeBlocks)
		.SingleOrDefault(d => d.Id == doctorId);

			if (doctor == null)
			{
				throw new KeyNotFoundException("Doctor not found.");
			}

			var existingRoutine = doctor.Routines.FirstOrDefault(r => r.DayOfWeek == (int)routineDto.DayOfWeek);

			try
			{
				if (existingRoutine != null)
				{
					throw new BadRequestException("You have a routine on the day you enter. Please update.");
				}
				else
				{
					var newRoutine = _mapper.Map<Routine>(routineDto);
					doctor.Routines.Add(newRoutine);
					context.SaveChanges();
				}
			}
			catch
			{
				throw new BadRequestException("An error occurred while creating the routine.");
			}
		}

		public void UpdateRoutineAndTimeBlocks(int doctorId, RoutineDto routineDto)
		{
			var doctor = context.Doctors
		.Include(d => d.Routines)
		.ThenInclude(r => r.TimeBlocks)
		.FirstOrDefault(d => d.Id == doctorId);

			if (doctor == null)
			{
				throw new NotFoundException("Doctor not found.");
			}

			var existingRoutine = doctor.Routines.FirstOrDefault(r => r.Id == routineDto.Id);

			if (existingRoutine == null)
			{
				throw new NotFoundException("The doctor's routine to update could not be found!");
			}

			existingRoutine.IsOnLeave = routineDto.IsOnLeave;

			foreach (var existingTimeBlock in existingRoutine.TimeBlocks.ToList())
			{
				context.TimeBlocks.Remove(existingTimeBlock);
			}

			if (routineDto.TimeBlocks != null)
			{
				foreach (var updatedTimeBlock in routineDto.TimeBlocks)
				{
					var newTimeBlock = _mapper.Map<TimeBlock>(updatedTimeBlock);
					existingRoutine.TimeBlocks.Add(newTimeBlock);
				}
			}

			context.SaveChanges();
		}

		public void DeleteRoutine(int doctorId, int routineId)
		{
			var doctor = context.Doctors
		.Include(d => d.Routines)
		.ThenInclude(r => r.TimeBlocks)
		.FirstOrDefault(d => d.Id == doctorId);

			if (doctor == null)
			{
				throw new NotFoundException("Doctor not found.");
			}

			var routineToDelete = doctor.Routines.FirstOrDefault(r => r.Id == routineId);

			if (routineToDelete == null)
			{
				throw new NotFoundException("Doctor's {dayOfWeek} routine not found.");
			}

			context.TimeBlocks.RemoveRange(routineToDelete.TimeBlocks);
			context.Routines.Remove(routineToDelete);
			context.SaveChanges();
		}

		public List<RoutineDto> GetDoctorRoutines(int doctorId)
		{
			var doctorRoutines = context.Routines
				.Where(routine => routine.DoctorId == doctorId)
				.Include(routine => routine.TimeBlocks)
				.ToList();

			return _mapper.Map<List<RoutineDto>>(doctorRoutines);
		}

		public void CreateOneTime(int doctorId, OneTimeDto oneTimeDto)
		{
			var oneTime = context.Doctors.Include(x => x.OneTimes).ThenInclude(x => x.OneTimeTimeBlocks).FirstOrDefault(x => x.Id == doctorId);

			if (oneTime == null)
			{
				throw new NotFoundException();
			}

			DateOnly oneTimeDtoDate = oneTimeDto.Day;

			var doctorsOneTime = oneTime.OneTimes.FirstOrDefault(x => x.Day == new DateTime(oneTimeDto.Day.Year, oneTimeDto.Day.Month, oneTimeDto.Day.Day));

			var oneTimes = _mapper.Map<OneTime>(oneTimeDto);
			oneTime.OneTimes.Add(oneTimes);
			context.SaveChanges();
		}

		public List<OneTime> GetDoctorOneTimes(int doctorId, DateOnly startDate, DateOnly endDate)
		{
			return context.Doctors
				.Where(d => d.Id == doctorId)
				.SelectMany(d => d.OneTimes)
				.Where(ot => ot.Day >= new DateTime(startDate.Year, startDate.Month, startDate.Day) && ot.Day <= new DateTime(endDate.Year, endDate.Month, endDate.Day))
				.Include(a => a.OneTimeTimeBlocks)
				.ToList();
		}

		public void DeleteOneTime(int doctorId, int oneTimeId)
		{
			var doctor = context.Doctors
				.Include(d => d.OneTimes)
				.ThenInclude(ot => ot.OneTimeTimeBlocks)
				.FirstOrDefault(d => d.Id == doctorId);

			if (doctor == null)
			{
				throw new NotFoundException("Doctor not found");
			}

			var oneTime = doctor.OneTimes.FirstOrDefault(ot => ot.Id == oneTimeId);

			if (oneTime == null)
			{
				throw new NotFoundException("OneTime not found");
			}

			context.OneTimeTimeBlocks.RemoveRange(oneTime.OneTimeTimeBlocks);
			context.OneTimes.Remove(oneTime);
			context.SaveChanges();
		}

		public void UpdateOneTime(int doctorId, int oneTimeId, OneTimeDto oneTimeDtos)
		{
			var doctor = context.Doctors.Include(x => x.OneTimes).ThenInclude(a => a.OneTimeTimeBlocks).FirstOrDefault(a => a.Id == doctorId);

			if (doctor == null)
			{
				throw new NotFoundException("Doctor not found!");
			}

			var oneTimeToUpdate = context.OneTimes.FirstOrDefault(x => x.Id == oneTimeId);

			if (oneTimeToUpdate == null)
			{
				throw new NotFoundException("OneTime not found!");
			}

			oneTimeToUpdate.Day = new DateTime(oneTimeDtos.Day.Year, oneTimeDtos.Day.Month, oneTimeDtos.Day.Day);
			oneTimeToUpdate.IsOnLeave = oneTimeDtos.IsOnLeave;

			foreach (var existingOneTime in oneTimeToUpdate.OneTimeTimeBlocks.ToList())
			{
				context.OneTimeTimeBlocks.Remove(existingOneTime);
			}

			if (oneTimeDtos.OneTimeTimeBlocks != null)
			{
				foreach (var updatedOneTime in oneTimeDtos.OneTimeTimeBlocks)
				{
					var newOneTime = _mapper.Map<OneTimeTimeBlock>(updatedOneTime);
					oneTimeToUpdate.OneTimeTimeBlocks.Add(newOneTime);
				}
			}

			context.SaveChanges();
		}

		public virtual List<DateInfoDto> GetDoctorRoutinesAndOneTimes(int doctorId, DateOnly? startDate, DateOnly? endDate)
		{
			var doctor = context.Doctors
		.Include(d => d.Routines)
			.ThenInclude(r => r.TimeBlocks)
		.Include(d => d.OneTimes)
			.ThenInclude(ot => ot.OneTimeTimeBlocks)
		.FirstOrDefault(d => d.Id == doctorId);

			if (doctor == null)
			{
				throw new NotFoundException("Doctor not found!");
			}

			if (!startDate.HasValue || !endDate.HasValue)
			{
				startDate ??= DateOnly.FromDateTime(DateTime.Now);
				endDate ??= DateOnly.FromDateTime(DateTime.Now.AddMonths(1));
			}

			var routineAndOneTimeInfos = doctor.Routines
				.SelectMany(r => Enumerable.Range(0, endDate.Value.DayNumber + 1 - startDate.Value.DayNumber)
					.Select(i => startDate.Value.AddDays(i))
					.Where(d => (int)d.DayOfWeek == r.DayOfWeek)
					.Select(d => new DateInfoDto
					{
						DayOfWeek = d.DayOfWeek,
						IsOnLeave = r.IsOnLeave,
						Day = d,
						dateInfoTimeDtos = r.TimeBlocks.Select(tb => new DateInfoTimeDto
						{
							StartTime = tb.StartTime,
							EndTime = tb.EndTime
						}).ToList()
					}))
				.ToList();

			var lastFourWeeksOneTimes = doctor.OneTimes
				.Where(ot => DateOnly.FromDateTime(ot.Day) >= startDate && DateOnly.FromDateTime(ot.Day) <= endDate)
				.Select(ot => new DateInfoDto
				{
					DayOfWeek = ot.Day.DayOfWeek,
					IsOnLeave = ot.IsOnLeave,
					Day = DateOnly.FromDateTime(ot.Day),
					dateInfoTimeDtos = ot.OneTimeTimeBlocks.Select(otb => new DateInfoTimeDto
					{
						StartTime = otb.StartTime,
						EndTime = otb.EndTime
					}).ToList()
				})
				.ToList();

			foreach (var ot in lastFourWeeksOneTimes)
			{
				var existingRoutineInfo = routineAndOneTimeInfos.FirstOrDefault(r =>
				r.Day.Equals(ot.Day));

				if (existingRoutineInfo == null)
				{
					routineAndOneTimeInfos.Add(ot);
				}
				else if (ot.IsOnLeave)
				{
					existingRoutineInfo.IsOnLeave = true;

					var leaveDayInfo = lastFourWeeksOneTimes.FirstOrDefault(l => l.Day.Equals(ot.Day));
					if (leaveDayInfo != null)
					{
						existingRoutineInfo.dateInfoTimeDtos = leaveDayInfo.dateInfoTimeDtos;
					}
				}
			}

			return routineAndOneTimeInfos.OrderBy(info => info.Day).ToList();
		}

		public void AddPrescriptionToAppointment(int appointmentId, string prescription)
		{
			var appointment = context.Appointments.Include(a => a.Status).FirstOrDefault(a => a.Id == appointmentId && (a.Status.Id == 14));

            if (appointment == null)
            {
				throw new NotFoundException("Appointment not found!"); 
            }

            appointment.Prescription = prescription;
			appointment.StatusId = 14;
			context.SaveChanges();
		}

		public void MarkPatientAsNoShow(int appointmentId)
		{
			var appointment = context.Appointments.FirstOrDefault(a => a.Id == appointmentId && a.StatusId != 17 &&
			!(a.StatusId == 14 || a.StatusId == 16));

			if (appointment == null && DateTime.Now <= appointment.Date.AddMinutes(10))
			{
				throw new NotFoundException("Appointment could not be found or you cannot make a mark before the appointment time expires!");
			}

			appointment.StatusId = 17;
			context.SaveChanges();
		}
	}
}