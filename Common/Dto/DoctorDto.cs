using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto
{
	public class DoctorDto
	{
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public string Surname { get; set; } = string.Empty;

		public string Username { get; set; } = string.Empty;

		public string? Email { get; set; }

		public string Password { get; set; } = string.Empty;

		public int TitleId { get; set; }

		public List<int> DepartmentIds { get; set; } = new List<int>();


	}
}
