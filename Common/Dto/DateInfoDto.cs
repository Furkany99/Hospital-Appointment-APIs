using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto
{
	public class DateInfoDto
	{
		public List<RoutineDto>? Routines { get; set; }
		public List<OneTimeDto>? OneTimes { get; set; }
	}
}
