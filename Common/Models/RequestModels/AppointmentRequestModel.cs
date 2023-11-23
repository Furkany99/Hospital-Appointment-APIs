using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.RequestModels
{
	public class AppointmentRequestModel
	{
		public DateTime Date { get; set; }

		public string? Detail { get; set; }

		public TimeOnly Duration { get; set; }

		public int DocId { get; set; }

		public int DepartmentId { get; set; }

		public int PatientId { get; set; }
	}
}
