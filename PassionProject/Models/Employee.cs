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
        public string Fname { get; set; }
        public string Lname { get; set; }
        [Column(TypeName = "Date")]
        public DateTime DOJ { get; set; }
        public string Bio { get; set; }
        public ICollection<Service> Services { get; set; }
        public ICollection<Appointment> Appointments { get; set; }

    }

    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public DateTime DOJ { get; set; }
        public string Bio { get; set; }
        public List<ServiceDto> Services { get; set; } = new List<ServiceDto>();
        public List<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();

    }

    public class EmployeeVM
    {
        public EmployeeDto Employee { get; set; }
        public IEnumerable<ServiceDto> ServicesAssigned { get; set; }
        public IEnumerable<ServiceDto> ServicesAvailable { get; set; }
    }
}