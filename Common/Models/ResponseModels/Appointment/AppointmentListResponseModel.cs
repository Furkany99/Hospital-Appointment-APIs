using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.ResponseModels.Appointment
{
    public class AppointmentListResponseModel
    {
        public int Count { get; set; }
        public List<AppointmentResponseModel> appointmentResponseModels { get; set; } = new List<AppointmentResponseModel>();
    }
}
