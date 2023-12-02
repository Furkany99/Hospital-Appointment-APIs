using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels;
using Common.Models.ResponseModels;
using Common.RequestModels;
using Common.ResponseModels;
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

		[HttpPost("Appointments")]
		public IActionResult Appointment(AppointmentRequestModel requestModel)
		{
			try
			{
				var appointments = _mapper.Map<AppointmentDto>(requestModel);
				_patientService.CreateAppointment(appointments);
				return Ok();
			}
			catch
			{
				return BadRequest();
			}

		}

		[HttpDelete("Appointments")]
		public IActionResult Delete(int id)
		{
			_patientService.DeleteAppointment(id);
			return Ok();
		}

		[HttpGet("Appointments")]
		public List<AppointmentListResponseModel> GetAppointments(int id)
		{
			var appointments = _patientService.GetPatientAppointments(id);
			var appointmentResponseList = appointments.Select(appointmentDto => _mapper.Map<AppointmentResponseModel>(appointmentDto)).ToList();

			var appointmentListResponse = new AppointmentListResponseModel
			{
				Count = appointmentResponseList.Count,
				appointmentResponseModels = appointmentResponseList
			};

			return new List<AppointmentListResponseModel> { appointmentListResponse };


		}
	}
}
