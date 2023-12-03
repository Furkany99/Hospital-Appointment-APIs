using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels;
using Common.Models.RequestModels.Department;
using Common.Models.RequestModels.Patient;
using Common.Models.ResponseModels;
using Common.ResponseModels;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Services;

namespace WebAPI.Controllers
{
    [ApiController]
	[Route("[controller]")]
	public class AdminController : ControllerBase
	{
		private readonly PatientService _patientService;
		private readonly DepartmentService _departmentService;
		private readonly IMapper _mapper;
		

		public AdminController(PatientService patientService, IMapper mapper,DepartmentService departmentService)
		{
			_patientService = patientService;
			_mapper = mapper;
			_departmentService = departmentService;
		}


		[HttpPost("Patients")]
		public IActionResult CreatePatient(PatientCreateRequestModel patientCreate)
		{
			try
			{
				var patients = _mapper.Map<PatientDto>(patientCreate);
				_patientService.CreatePatient(patients);
				return Ok();
			}
			catch 
			{
				return BadRequest();
			}

		}

		[HttpGet("Patients")]
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
		public PatientResponseModel GetPatientByID(int id)
		{
			var patientDto = _patientService.GetPatientById(id);
			var patientResponseModel = _mapper.Map<PatientResponseModel>(patientDto);
			return patientResponseModel;
		}
		
		[HttpPut("Patients/{id}")]
		public PatientUpdateRequestModel UpdatePatient(int id,PatientUpdateRequestModel patientUpdate)
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
		public IActionResult DeletePatient(int id)
		{
			_patientService.Delete(id);
			return Ok();

		}


		

		[HttpPost("Department")]
		public IActionResult CreateDepartment(DepartmentRequestModel departmentRequest)
		{
			try
			{
				var departments = _mapper.Map<DepartmentDto>(departmentRequest);
				_departmentService.CreateDepartments(departments);
				return Ok();
			}

			catch
			{

				return BadRequest();
			}

		}

		[HttpGet("Departments")]
		public List<DepartmentListResponseModel> GetDepartments()
		{
			var departments = _departmentService.GetDepartments();
			var departmentResponseList = departments.Select(departmentsDto => _mapper.Map<DepartmentResponseModel>(departmentsDto)).ToList();

			var departmentListResponse = new DepartmentListResponseModel
			{
				Count = departmentResponseList.Count,
				departmentResponseModels = departmentResponseList
				
			};

			return new List<DepartmentListResponseModel> { departmentListResponse };


		}

		[HttpPut("Department/{id}")]
		public DepartmentUpdateRequestModel UpdateDepartment(int id, DepartmentUpdateRequestModel departmentUpdate)
		{
			var updatedDepartment = _departmentService.UpdateDepartment(id, departmentUpdate);
			var departmentRequestModel = _mapper.Map<DepartmentUpdateRequestModel>(updatedDepartment);
			if (departmentUpdate != null)
			{
				return departmentRequestModel;
			}
			else
			{
				return null;
			}

		}

		



		[HttpDelete("Department/{id}")]
		public IActionResult DeleteDepartment(int id)
		{
			try 
			{
				_departmentService.DeleteDepartments(id);
				return Ok();
			}
			catch 
			{
				return BadRequest();
			}
			

		}



	} 
}
