using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PassionProject.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        [Display(Name = "First Name:")]
        public string Fname { get; set; }
        [Display(Name = "Last Name:")]
        public string Lname { get; set; }
        [Display(Name = "Date of Joining:")]
        [Column(TypeName = "Date")]
        public DateTime DOJ { get; set; }
        [Display(Name = "Bio:")]
        public string Bio { get; set; }
        public ICollection<Service> Services { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public bool EmployeeHasPic { get; set; }
        public string PicExtension { get; set; }
    }

    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        [Display(Name = "First Name:")]
        public string Fname { get; set; }
        [Display(Name = "Last Name:")]
        public string Lname { get; set; }
        [Display(Name = "Date of Joining:")]
        public DateTime DOJ { get; set; }
        [Display(Name = "Bio:")]
        public string Bio { get; set; }
        public List<ServiceDto> Services { get; set; } = new List<ServiceDto>();
        public List<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();
        public bool EmployeeHasPic { get; set; }
        public string PicExtension { get; set; }
        public IEnumerable<IGrouping<DateTime, AppointmentDto>> AppointmentGroup
        {
            get
            {
                return Appointments.GroupBy(x => x.StartTime.Date);
            }
        }

    }

    public class EmployeeVM
    {
        public EmployeeDto Employee { get; set; }
        public IEnumerable<ServiceDto> ServicesAssigned { get; set; }
        public IEnumerable<ServiceDto> ServicesAvailable { get; set; }
    }
}