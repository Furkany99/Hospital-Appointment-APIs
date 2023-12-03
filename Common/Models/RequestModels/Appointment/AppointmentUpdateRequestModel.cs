using Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.RequestModels.Appointment
{
    public class AppointmentUpdateRequestModel
    {

        public DateOnly Date { get; set; }

        public string? Detail { get; set; }

        public int DocId { get; set; }

        public List<AppointmentTimeDto>? appointmentTimes { get; set; }
    }
}
