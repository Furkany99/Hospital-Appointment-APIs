namespace Common.ResponseModels;

public class DoctorResponseModel
{
	public int Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public string Surname { get; set; } = string.Empty;

	public string? Email { get; set; }

	public int TitleId { get; set; }

	public List<int> DepartmentIds { get; set; } = new List<int>();
}
