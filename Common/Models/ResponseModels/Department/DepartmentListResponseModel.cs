using Common.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.ResponseModels.Department
{
    public class DepartmentListResponseModel
    {
        public int Count { get; set; }
        public List<DepartmentResponseModel> departmentResponseModels { get; set; } = new List<DepartmentResponseModel>();
    }
}
