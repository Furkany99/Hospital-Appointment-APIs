namespace Common.Models.RequestModels.Doctor
{
    public class DoctorRequestModel
    {

        public string Name { get; set; } = string.Empty;

        public string Surname { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string? Email { get; set; }

        public int TitleId { get; set; }

        public List<int> DepartmentId { get; set; } = new List<int>();
    }
}
