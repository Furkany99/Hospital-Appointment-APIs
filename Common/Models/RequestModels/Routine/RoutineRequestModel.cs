using Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.RequestModels.Routine
{
    public class RoutineRequestModel
    {
        public DayOfWeek DayOfWeek { get; set; }

        public bool IsOnLeave { get; set; }

        public List<TimeBlockDto>? TimeBlocks { get; set; }
    }
}
