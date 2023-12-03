using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.ResponseModels.Routine
{
    public class RoutineListResponseModel
    {
        public int Count { get; set; }
        public List<RoutineResponseModel> routineResponseModels { get; set; } = new List<RoutineResponseModel>();
    }
}
