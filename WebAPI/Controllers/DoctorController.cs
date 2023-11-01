using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels;
using Common.Models.ResponseModels;
using Common.RequestModels;
using Common.ResponseModels;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DoctorController : Controller
	{
		private readonly DoctorService _doctorService;
		private readonly IMapper _mapper;

		public DoctorController(DoctorService doctorService,IMapper mapper)
        {
			_doctorService = doctorService;
			_mapper = mapper;
		}

		[HttpPost("Doctors")]
		public IActionResult CreateDoctor(DoctorRequestModel doctorRequest)
		{
			try
			{
				var doctors = _mapper.Map<DoctorDto>(doctorRequest);
				_doctorService.CreateDoctor(doctors);
				return Ok();
			}

			catch (Exception ex)
			{

				return BadRequest("Doktor oluşturulamadı: " + ex.Message);
			}

		}

		[HttpGet("Doctors")]
		public List<DoctorListResponseModel> GetDoctors()
		{
			var doctors = _doctorService.GetDoctors();
			var doctorResponseList = doctors.Select(doctorDto => _mapper.Map<DoctorResponseModel>(doctorDto)).ToList();

			var doctorListResponse = new DoctorListResponseModel
			{
				Count = doctorResponseList.Count,
				doctorResponseModels = doctorResponseList
			};

			return new List<DoctorListResponseModel> { doctorListResponse };


		}

		[HttpGet("{id}")]
		public DoctorResponseModel GetDoctorsByID(int id)
		{
			var doctorDto = _doctorService.GetDoctorById(id);
			var doctorResponseModel = _mapper.Map<DoctorResponseModel>(doctorDto);
			return doctorResponseModel;
		}

		[HttpPut("{id}")]
		public DoctorUpdateRequestModel UpdatePatient(int id, DoctorUpdateRequestModel doctorUpdate)
		{
			var updatedDoctor = _doctorService.UpdateDoctor(id, doctorUpdate);
			var doctorRequestModel = _mapper.Map<DoctorUpdateRequestModel>(updatedDoctor);
			if (updatedDoctor != null)
			{
				return doctorRequestModel;
			}
			else
			{
				return null;
			}

		}

		[HttpDelete("{id}")]
		public IActionResult DeleteDoctor(int id)
		{
			_doctorService.DeleteDoctor(id);
			return Ok();

		}

		[HttpPut("{id}/Departments")]
		public DoctorDepartmentUpdateRequest UpdateDoctorDepartment(int id, DoctorDepartmentUpdateRequest departmentUpdateRequest)
		{
			try
			{
				var existingDoctor = _doctorService.GetDoctorById(id);

				_mapper.Map(departmentUpdateRequest, existingDoctor);

				var doctorDto = _doctorService.UpdateDoctorDepartment(id, existingDoctor.DepartmentIds);

				return departmentUpdateRequest;
			}
			catch (KeyNotFoundException)
			{
				throw new KeyNotFoundException();
			}
		}

		[HttpPut("{id}/Titles")]
		public DoctorTitleUpdateRequest UpdateDoctorTitle(int id, DoctorTitleUpdateRequest doctorTitleUpdate)
		{
			try
			{
				var existingTitle = _doctorService.GetDoctorById(id);
				_mapper.Map(doctorTitleUpdate, existingTitle);
				var doctorDto = _doctorService.UpdateDoctorTitle(id, existingTitle.TitleId);
				return doctorTitleUpdate;
			}
			catch (KeyNotFoundException)
			{
				throw new KeyNotFoundException();
			}
		}

		[HttpPost("{doctorId}/routines")]
		public IActionResult CreateRoutineForDoctor(int doctorId, RoutineRequestModel routineRequest)
		{
			try
			{
				var doctor = _doctorService.GetDoctorById(doctorId);
				if (doctor == null)
				{
					return NotFound("Doktor bulunamadı.");
				}

				var routineDto = _mapper.Map<RoutineDto>(routineRequest);
				routineDto.DoctorId = doctorId;

				_doctorService.CreateRoutine(doctorId, routineDto);

				return Ok("Rutin başarıyla oluşturuldu.");
			}
			catch (Exception ex)
			{
				return BadRequest("Rutin oluşturulurken bir hata oluştu: " + ex.Message);
			}
		}

		[HttpPut("{doctorId}/routines")]
		public RoutineUpdateRequestModel UpdateRoutineAndTimeBlocks(int doctorId,int routineId ,RoutineUpdateRequestModel routineUpdateRequest)
		{
			try
			{
				var routineDto = _mapper.Map<RoutineDto>(routineUpdateRequest);
				routineDto.DoctorId = doctorId;
				routineDto.DayOfWeek = routineUpdateRequest.DayOfWeek;
				routineDto.Id = routineId;
				_doctorService.UpdateRoutineAndTimeBlocks(doctorId, routineDto);
				return routineUpdateRequest;
			}
			catch (KeyNotFoundException ex)
			{
				throw new KeyNotFoundException("Doktor veya rutin bulunamadı.", ex);
			}
		}

		[HttpDelete("{doctorId}/routines")]
		public IActionResult DeleteDoctorRoutine(int doctorId,int routineID)
		{
			try
			{
				_doctorService.DeleteRoutine(doctorId, routineID);
				return Ok("Doktorun rutinleri ve TimeBlock verileri silindi.");
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}
	}
}
