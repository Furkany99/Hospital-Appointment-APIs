using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto
{
	public class PatientDto
	{
		public PatientDto()
		{
			RoleId = 3;
		}

		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Surname { get; set; } = string.Empty;
		public DateTime Birthdate { get; set; }
		public string? PhoneNumber { get; set; }
		public string Email { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty; 
		public string Password { get; set; } = string.Empty;
		public string FirebaseUID { get; set; }
		public int RoleId { get; private set; }
		public int? Weight { get; set; }
		public double? Height { get; set; }
	}
}
