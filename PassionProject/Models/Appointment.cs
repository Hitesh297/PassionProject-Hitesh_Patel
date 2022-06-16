using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PassionProject.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        [Display(Name = "Customer Name:")]
        public string CustomerName { get; set; }
        [Display(Name = "Customer Email:")]
        public string CustomerEmail { get; set; }
        public int ServiceId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ServiceName { get; set; }
    }
}