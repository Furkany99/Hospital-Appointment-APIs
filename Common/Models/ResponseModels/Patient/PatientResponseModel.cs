namespace Common.Models.ResponseModels.Patient
{
    public class PatientResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateTime Birthdate { get; set; }
        public string? Email { get; set; }
    }
}
