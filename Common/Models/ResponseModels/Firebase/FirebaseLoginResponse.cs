using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.ResponseModels.Firebase
{
	public class FirebaseLoginResponse
	{
		public string idToken { get; set; } = null!;
		//public string UID { get; set; } = null!;
		public string Email { get; set; } = null!;
	}
}
