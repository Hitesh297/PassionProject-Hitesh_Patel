using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PassionProject.Models;

namespace PassionProject.Controllers
{
    public class AppointmentDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all appointments in the system
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all appointments in the database
        /// </returns>
        /// <example>
        /// GET: api/AppointmentData/ListAppointments
        /// </example>
        [HttpGet]
        public IEnumerable<AppointmentDto> ListAppointments()
        {
            List<Appointment> appointments = db.Appointments.ToList();
            List<AppointmentDto> appointmentDtos = new List<AppointmentDto>();

            appointments.ForEach(x => appointmentDtos.Add(new AppointmentDto()
            {
                AppointmentId = x.AppointmentId,
                ServiceId = x.ServiceId,
                StartTime = x.StartTime,
                CustomerEmail = x.CustomerEmail,
                CustomerName = x.CustomerName,
                EmployeeId = x.EmployeeId,
                EndTime = x.EndTime
            }));

            return appointmentDtos;
        }


        /// <summary>
        /// Returns details of the appointment by appointment id
        /// </summary>
        /// <param name="id">Appointment primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An appointment in the system matching up to the appointment ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// GET: api/AppointmentData/FindAppointmentsByEmployee/5
        /// </example>
        [ResponseType(typeof(Appointment))]
        [HttpGet]
        public IHttpActionResult FindAppointmentsByEmployee(int id)
        {
            List<Appointment> appointments = db.Appointments.Include(x=>x.Service).Where(x => x.EmployeeId == id).ToList();
            List<AppointmentDto> appointmentDtos = new List<AppointmentDto>();

            appointments.ForEach(x => appointmentDtos.Add(new AppointmentDto()
            {
                AppointmentId = x.AppointmentId,
                ServiceId = x.ServiceId,
                StartTime = x.StartTime,
                CustomerEmail = x.CustomerEmail,
                CustomerName = x.CustomerName,
                EmployeeId = x.EmployeeId,
                EndTime = x.EndTime,
                ServiceName = x.Service.Name
            }));

            return Ok(appointmentDtos);
        }


        /// <summary>
        /// Adds an appointment to the system
        /// </summary>
        /// <param name="appointment">JSON form data of appointment</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Appointment ID, Appointment Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/AppointmentData/AddAppointment
        /// </example>
        [ResponseType(typeof(Appointment))]
        [HttpPost]
        public IHttpActionResult AddAppointment(Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Appointments.Add(appointment);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = appointment.AppointmentId }, appointment);
        }


        /// <summary>
        /// Deletes an appointment from the system by it's ID.
        /// </summary>
        /// <param name="id">primary key of appointment</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// DELETE: api/AppointmentData/DeleteAppointment/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Appointment))]
        [HttpPost]
        public IHttpActionResult DeleteAppointment(int id)
        {
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return NotFound();
            }

            db.Appointments.Remove(appointment);
            db.SaveChanges();

            return Ok(appointment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AppointmentExists(int id)
        {
            return db.Appointments.Count(e => e.AppointmentId == id) > 0;
        }
    }
}