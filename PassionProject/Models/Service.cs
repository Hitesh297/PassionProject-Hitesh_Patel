using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PassionProject.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public decimal Cost { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }

    public class ServiceDto
    {
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public decimal Cost { get; set; }
    }
}