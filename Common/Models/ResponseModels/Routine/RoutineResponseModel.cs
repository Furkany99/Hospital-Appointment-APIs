using Common.Dto;
using Common.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.ResponseModels.Routine
{
    public class RoutineResponseModel
    {
        //public int DoctorId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public bool IsOnLeave { get; set; }

        public List<TimeBlockDto>? TimeBlocks { get; set; }
    }
}
