using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.RequestModels
{
	public class DoctorDepartmentUpdateRequest
	{

		public List<int> DepartmentIds { get; set; } = new List<int>();
	}
}
