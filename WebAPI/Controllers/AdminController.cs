using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels.Department;
using Common.Models.RequestModels.Patient;
using Common.Models.ResponseModels.Department;
using Common.Models.ResponseModels.Patient;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
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

		////[Authorize(Roles = "Admin")]
		//[HttpPost("Patients")]
		//public IActionResult CreatePatient(PatientCreateRequestModel patientCreate)
		//{
		//	try
		//	{
		//		var patients = _mapper.Map<PatientDto>(patientCreate);
		//		_patientService.CreatePatient(patients);
		//		return Ok();
		//	}
		//	catch 
		//	{
		//		return BadRequest();
		//	}

		//}

		


		

		[HttpPost("Department")]
		[Authorize(Roles = "Admin")]
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
		[Authorize(Roles = "Admin,Doctor,Patient")]
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
		[Authorize(Roles = "Admin")]
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
		[Authorize(Roles = "Admin")]
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
