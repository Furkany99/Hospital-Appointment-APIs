using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels.Doctor;
using Common.Models.RequestModels.OneTime;
using Common.Models.RequestModels.Routine;
using Common.Models.ResponseModels.Doctor;
using Common.Models.ResponseModels.OneTime;
using Common.Models.ResponseModels.Routine;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
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
		private readonly ILogger<DoctorController> _logger;

		public DoctorController(DoctorService doctorService, IMapper mapper, ILogger<DoctorController> logger)
		{
			_doctorService = doctorService;
			_mapper = mapper;
			_logger = logger;
		}

		[HttpPost("Doctors")]
		[Authorize(Roles = "Admin,Doctor")]
		public IActionResult CreateDoctor(DoctorRequestModel doctorRequest)
		{
				var doctors = _mapper.Map<DoctorDto>(doctorRequest);
				_doctorService.CreateDoctor(doctors);
				return Ok();
		}

		[HttpGet("Doctors")]
		[Authorize(Roles = "Admin,Doctor,Patient")]
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
		[Authorize(Roles = "Admin,Doctor,Patient")]
		public DoctorResponseModel GetDoctorsByID(int id)
		{
			var doctorDto = _doctorService.GetDoctorById(id);
			var doctorResponseModel = _mapper.Map<DoctorResponseModel>(doctorDto);
			return doctorResponseModel;
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Admin,Doctor")]
		public DoctorUpdateRequestModel UpdateDoctor(int id, DoctorUpdateRequestModel doctorUpdate)
		{
			var updatedDoctor = _doctorService.UpdateDoctor(id, doctorUpdate);
			var doctorRequestModel = _mapper.Map<DoctorUpdateRequestModel>(updatedDoctor);
			return doctorRequestModel;
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public IActionResult DeleteDoctor(int id)
		{
			_doctorService.DeleteDoctor(id);
			return Ok();
		}

		[HttpPut("{id}/Departments")]
		[Authorize(Roles = "Admin")]
		public ActionResult<DoctorDepartmentUpdateRequest> UpdateDoctorDepartment(int id, DoctorDepartmentUpdateRequest departmentUpdateRequest)
		{
			
			var existingDoctor = _doctorService.GetDoctorById(id);
			_mapper.Map(departmentUpdateRequest, existingDoctor);
			var updateDoc = _doctorService.UpdateDoctorDepartment(id, existingDoctor.DepartmentIds);
			var updatedDoctorRequest = _mapper.Map<DoctorDepartmentUpdateRequest>(updateDoc);
			return updatedDoctorRequest;
			
		}

		[HttpPut("{id}/Titles")]
		[Authorize(Roles = "Admin")]
		public DoctorTitleUpdateRequest UpdateDoctorTitle(int id, DoctorTitleUpdateRequest doctorTitleUpdate)
		{
			
			var existingDoctorTitle = _doctorService.GetDoctorById(id);
			_mapper.Map(doctorTitleUpdate, existingDoctorTitle);
			var UpdatedoctorDto = _doctorService.UpdateDoctorTitle(id, existingDoctorTitle.TitleId);
			var updateDoctorTitle = _mapper.Map<DoctorTitleUpdateRequest>(UpdatedoctorDto);
			return updateDoctorTitle;
			
			
		}

		[HttpPost("{doctorId}/routines")]
		[Authorize(Roles = "Admin")]
		public IActionResult CreateRoutineForDoctor(int doctorId, RoutineRequestModel routineRequest)
		{
			try
			{
				var doctor = _doctorService.GetDoctorById(doctorId);

				var routineDto = _mapper.Map<RoutineDto>(routineRequest);
				routineDto.DoctorId = doctorId;

				_doctorService.CreateRoutine(doctorId, routineDto);

				return Ok("The routine has been created successfully.");
			}
			catch (Exception ex)
			{
				return BadRequest("An error occurred while creating the routine: " + ex.Message);
			}
		}

		[HttpGet("{doctorId}/routines")]
		[Authorize(Roles = "Admin,Doctor,Patient")]
		public RoutineListResponseModel GetDoctorRoutines(int doctorId)
		{
			var doctorRoutines = _doctorService.GetDoctorRoutines(doctorId);
			var routineResponseModels = _mapper.Map<List<RoutineResponseModel>>(doctorRoutines);

			var routineListResponse = new RoutineListResponseModel
			{
				Count = routineResponseModels.Count,
				routineResponseModels = routineResponseModels
			};

			return routineListResponse;
		}

		[HttpPut("{doctorId}/routines")]
		[Authorize(Roles = "Admin")]
		public RoutineUpdateRequestModel UpdateRoutineAndTimeBlocks(int doctorId, int routineId, RoutineUpdateRequestModel routineUpdateRequest)
		{

			var routineDto = _mapper.Map<RoutineDto>(routineUpdateRequest);
			routineDto.DoctorId = doctorId;
			routineDto.Id = routineId;
			_doctorService.UpdateRoutineAndTimeBlocks(doctorId, routineDto);
			return routineUpdateRequest;
			
		}

		[HttpDelete("{doctorId}/routines")]
		[Authorize(Roles = "Admin")]
		public IActionResult DeleteDoctorRoutine(int doctorId, int routineID)
		{
			_doctorService.DeleteRoutine(doctorId, routineID);
			return Ok("The doctor's routines and TimeBlock data have been deleted.");
			
		}

		[HttpPost("{doctorId}/Onetime")]
		[Authorize(Roles = "Admin,Doctor")]
		public IActionResult CreateOneTime(int doctorId, OneTimeRequestModel requestModel)
		{
			
			_doctorService.GetDoctorById(doctorId);
			var onetimeDto = _mapper.Map<OneTimeDto>(requestModel);
			onetimeDto.DoctorId = doctorId;
			_doctorService.CreateOneTime(doctorId, onetimeDto);
			return Ok();
			
		}

		[HttpGet("{doctorId}/onetimes")]
		[Authorize(Roles = "Admin,Doctor,Patient")]
		public OneTimeListResponseModel GetDoctorOneTimes(int doctorId, DateOnly? startDate = null, DateOnly? endDate = null)
		{
			var responseModel = new OneTimeListResponseModel();
			List<OneTime> doctorOnetimes;

			if (!startDate.HasValue || !endDate.HasValue)
			{
				DateOnly fourWeeksAgo = DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-1));
				startDate = fourWeeksAgo;
				endDate = DateOnly.FromDateTime(DateTime.Now.Date);
			}

			doctorOnetimes = _doctorService.GetDoctorOneTimes(doctorId, startDate.Value, endDate.Value);

			if (doctorOnetimes == null)
			{
				responseModel.Count = 0;
				responseModel.oneTimeResponseModels = new List<OneTimeResponseModel>();
			}
			else
			{
				var oneTimeResponseModels = doctorOnetimes.Select(ot => _mapper.Map<OneTimeResponseModel>(ot)).ToList();
				responseModel.Count = oneTimeResponseModels.Count;
				responseModel.oneTimeResponseModels = oneTimeResponseModels;
			}

			return responseModel;
		}

		[HttpPut("{doctorId}/onetimes/{oneTimeId}")]
		[Authorize(Roles = "Admin,Doctor")]
		public OneTimeUpdateRequest UpdateOneTime(int doctorId, int oneTimeId, OneTimeUpdateRequest oneTimeUpdate)
		{
			var oneTimeDtos = _mapper.Map<OneTimeDto>(oneTimeUpdate);
			oneTimeDtos.DoctorId = doctorId;
			oneTimeDtos.Id = oneTimeId;
			_doctorService.UpdateOneTime(doctorId, oneTimeId, oneTimeDtos);
			return oneTimeUpdate;
			
		}

		[HttpDelete("{doctorId}/onetimes/{oneTimeId}")]
		[Authorize(Roles = "Admin,Doctor")]
		public IActionResult DeleteOneTime(int doctorId, int oneTimeId)
		{

			_doctorService.DeleteOneTime(doctorId, oneTimeId);
			return NoContent();
			
		}

		[HttpGet("{doctorId}/routines-and-onetimes")]
		[Authorize(Roles = "Admin,Doctor,Patient")]
		public DoctorRoutinesAndOneTimesResponseModel GetDoctorRoutinesAndOneTimes(int doctorId, DateOnly? startDate, DateOnly? endDate)
		{

			var doctorInfo = _doctorService.GetDoctorRoutinesAndOneTimes(doctorId, startDate, endDate);
			var doctorInfoResponseModels = _mapper.Map<List<DateInfoDto>>(doctorInfo);

			var responseModel = new DoctorRoutinesAndOneTimesResponseModel
			{
				Count = doctorInfoResponseModels.Count,
				DoctorInfo = doctorInfoResponseModels
			};

			return responseModel;
		}

		[HttpPost("{appointmentId}/prescription")]
		[Authorize(Roles = "Admin,Doctor")]
		public IActionResult AddPrescriptionToAppointment(int appointmentId, [FromBody] string prescription)
		{
			_doctorService.AddPrescriptionToAppointment(appointmentId, prescription);
			return Ok("The recipe has been added successfully.");
		}

		[HttpPost("{appointmentId}/mark-as-no-show")]
		[Authorize(Roles = "Admin,Doctor")]
		public IActionResult MarkAsNoShow(int appointmentId)
		{
			_doctorService.MarkPatientAsNoShow(appointmentId);
			return Ok("The appointment is marked as 'No Patient Showed'.");	
		}
	}
}