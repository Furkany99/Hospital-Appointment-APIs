using Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.RequestModels
{
	public class RoutineUpdateRequestModel
	{

		public bool IsOnLeave { get; set; }

		public List<TimeBlockRequestModel>? TimeBlocks { get; set; }
	}
}
