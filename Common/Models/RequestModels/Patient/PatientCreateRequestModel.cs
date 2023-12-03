namespace Common.Models.RequestModels.Patient
{
    public class PatientCreateRequestModel
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateTime Birthdate { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? Weight { get; set; }
        public double? Height { get; set; }
    }
}
