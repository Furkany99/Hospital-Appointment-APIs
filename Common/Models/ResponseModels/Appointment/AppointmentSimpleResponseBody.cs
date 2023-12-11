using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.ResponseModels.Appointment
{
	public class AppointmentSimpleResponseBody
	{
		public int AppointmentId { get; set; }
		public DateOnly AppointmentDate { get; set; }
		public List<AppointmentResponseTimeModel> TimeSlots { get; set; }
	}
}
