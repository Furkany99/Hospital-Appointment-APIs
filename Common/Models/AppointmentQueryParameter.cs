using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
	public class AppointmentQueryParameter
	{
		public int StatusId { get; set; }
		public int DoctorId { get; set; }
		public int DepartmentId { get; set; }
		public DateTime? startDate { get; set; }
		public DateTime? endDate { get; set; }
	}
}
