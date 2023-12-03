using Common.Dto;

namespace Common.Models.ResponseModels.Doctor
{
    public class DoctorRoutinesAndOneTimesResponseModel
    {
        public int Count { get; set; }
        public List<DateInfoDto>? DoctorInfo { get; set; }
    }
}