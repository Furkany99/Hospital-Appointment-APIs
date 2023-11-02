using Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.ResponseModels
{
	public class OneTimeResponseModel
	{
		public DateOnly Day { get; set; }

		public bool IsOnLeave { get; set; }

		public List<OneTimeTimeBlockDto>? OneTimeTimeBlocks { get; set; }
	}
}
