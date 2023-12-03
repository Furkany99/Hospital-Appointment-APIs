using Common.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.ResponseModels.OneTime
{
    public class OneTimeListResponseModel
    {
        public int Count { get; set; }
        public List<OneTimeResponseModel> oneTimeResponseModels { get; set; } = new List<OneTimeResponseModel>();
    }
}
