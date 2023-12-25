using AutoMapper;
using Common.Dto;
using Common.Models;
using Common.Models.RequestModels.Appointment;
using Common.Models.RequestModels.Patient;
using Common.Models.ResponseModels.Appointment;
using Common.Models.ResponseModels.Patient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace WebAPI.Controllers
{
    [ApiController]
	[Route("[controller]")]
	public class PatientController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly PatientService _patientService;

		public PatientController(PatientService patientService, IMapper mapper)
        {
			_patientService = patientService;
			_mapper = mapper;
		}

		[HttpGet("Patients")]
		[Authorize(Roles = "Admin,Doctor")]
		public List<PatientListResponseModel> GetPatients()
		{
			var patients = _patientService.GetPatients();
			var patientResponseList = patients.Select(patientDto => _mapper.Map<PatientResponseModel>(patientDto)).ToList();

			var patientListResponse = new PatientListResponseModel
			{
				Count = patientResponseList.Count,
				patientResponseModels = patientResponseList
			};

			return new List<PatientListResponseModel> { patientListResponse };


		}

		[HttpGet("Patients/{id}")]
		[Authorize(Roles = "Admin")]
		public PatientResponseModel GetPatientByID(int id)
		{
			var patientDto = _patientService.GetPatientById(id);
			var patientResponseModel = _mapper.Map<PatientResponseModel>(patientDto);
			return patientResponseModel;
		}

		[HttpPut("Patients/{id}")]
		[Authorize(Roles = "Admin,Patient")]
		public PatientUpdateRequestModel UpdatePatient(int id, PatientUpdateRequestModel patientUpdate)
		{
			var updatedPatient = _patientService.Update(id, patientUpdate);
			var PatientRequestModel = _mapper.Map<PatientUpdateRequestModel>(updatedPatient);
			if (updatedPatient != null)
			{
				return PatientRequestModel;
			}
			else
			{
				return null;
			}

		}

		[HttpDelete("Patient/{id}")]
		[Authorize(Roles = "Admin")]
		public IActionResult DeletePatient(int id)
		{
			_patientService.Delete(id);
			return Ok();

		}


	}
}
