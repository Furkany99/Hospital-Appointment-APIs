using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto
{
	public class AppointmentDto
	{
		public int Id { get; set; }

		public DateTime Date { get; set; }

		public int DepartmentId { get; set; }

		public string? Detail { get; set; }

		public TimeOnly Duration { get; set; }

		public int DocId { get; set; }

		public int PatientId { get; set; }

		public int StatusId { get; set; }

	}
}
