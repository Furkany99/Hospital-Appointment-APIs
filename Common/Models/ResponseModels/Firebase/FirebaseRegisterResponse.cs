using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.ResponseModels.Firebase
{
	public class FirebaseRegisterResponse
	{
		public string IdToken { get; set; }
		public string Email { get; set; }
		public string? FirebaseUid { get; set; }
	}
}
