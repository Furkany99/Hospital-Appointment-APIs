using Common.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.ResponseModels
{
	public class DoctorListResponseModel
	{
		public int Count { get; set; }
		public List<DoctorResponseModel> doctorResponseModels { get; set; } = new List<DoctorResponseModel>();
	}
}
