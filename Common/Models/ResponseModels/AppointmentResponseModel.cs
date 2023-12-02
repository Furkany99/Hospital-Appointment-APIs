using Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.ResponseModels
{
	public class AppointmentResponseModel
	{
		public DateOnly Date { get; set; }

		public int DepartmentId { get; set; }

		public string? Detail { get; set; }

		public int DocId { get; set; }

		public int StatusId { get; set; }

		public List<AppointmentTimeDto>? appointmentTimes { get; set; }
	}
}
