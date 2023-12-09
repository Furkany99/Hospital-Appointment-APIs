using AutoMapper;
using Common.Dto;
using Common.Models;
using Common.Models.RequestModels.Appointment;
using Common.Models.ResponseModels.Appointment;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AppointmentController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly AppointmentService _appointmentService;

		public AppointmentController(AppointmentService appointmentService, IMapper mapper)
		{
			_appointmentService = appointmentService;
			_mapper = mapper;
		}

		[HttpPost("Appointments")]
		public IActionResult Appointment(AppointmentRequestModel requestModel)
		{
			try
			{
				var appointments = _mapper.Map<AppointmentDto>(requestModel);
				_appointmentService.CreateAppointment(appointments);
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
			_appointmentService.DeleteAppointment(id);
			return Ok();
		}

		[HttpGet("Appointments")]
		public List<AppointmentListResponseModel> GetAppointments([FromQuery] AppointmentQueryParameter queryParameter)
		{
			var appointments = _appointmentService.GetPatientAppointments(queryParameter);
			var appointmentResponseList = appointments.Select(appointmentDto => _mapper.Map<AppointmentResponseModel>(appointmentDto)).ToList();

			var appointmentListResponse = new AppointmentListResponseModel
			{
				Count = appointmentResponseList.Count,
				appointmentResponseModels = appointmentResponseList
			};

			return new List<AppointmentListResponseModel> { appointmentListResponse };
		}

		[HttpGet("DoctorAppointmentsAndSchedules")]
		public ActionResult<List<AppointmentAvailabilityDto>> GetDoctorAppointmentsAndSchedules(int departmentId, DateOnly? startDate, DateOnly? endDate)
		{
			var doctorAppointmentsAndSchedules = _appointmentService.GetDoctorAppointmentsAndSchedules(departmentId, startDate, endDate);
			return doctorAppointmentsAndSchedules;
		}
	}
}