using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto
{
	public class OneTimeDto
	{
		public int Id { get; set; }

		public int DoctorId { get; set; }

		public DateOnly Day { get; set; }

		public bool IsOnLeave { get; set; }

		public List<OneTimeTimeBlockDto> OneTimeTimeBlocks { get; set; } = new List<OneTimeTimeBlockDto>();

	}
}
