using Common.Dto;
using Common.Models.RequestModels.TimeBlock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.RequestModels.Routine
{
    public class RoutineUpdateRequestModel
    {

        public bool IsOnLeave { get; set; }

        public List<TimeBlockRequestModel>? TimeBlocks { get; set; }
    }
}
