using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.RequestModels
{
	public class PatientUpdateRequestModel
	{
		public string Name { get; set; } = string.Empty;
		public string Surname { get; set; } = string.Empty;
	}
}
