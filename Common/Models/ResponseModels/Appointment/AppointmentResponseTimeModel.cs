﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.ResponseModels.Appointment
{
	public class AppointmentResponseTimeModel
	{
		public TimeOnly StartTime { get; set; }

		public TimeOnly EndTime { get; set; }
	}
}
