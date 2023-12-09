
namespace Common.Models
{
	public class AppointmentQueryParameter
	{
		public int PatientId { get; set; }
		public int StatusId { get; set; }
		public int DoctorId { get; set; }
		public int DepartmentId { get; set; }
		public DateTime? startDate { get; set; }
		public DateTime? endDate { get; set; }
	}
}
