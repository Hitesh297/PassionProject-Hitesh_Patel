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
    public class ServiceDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all the services in the system
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all services in the database
        /// </returns>
        /// <example>
        /// GET: api/ServiceData/ListServices
        /// </example>
        [HttpGet]
        public IEnumerable<ServiceDto> ListServices()
        {
            List<Service> services = db.Services.ToList();
            List<ServiceDto> serviceDtos = new List<ServiceDto>();

            services.ForEach(x => serviceDtos.Add(new ServiceDto()
            {
                ServiceId = x.ServiceId,
                Name = x.Name,
                Duration = x.Duration,
                Cost = x.Cost
            }));

            return serviceDtos;
        }

        /// <summary>
        /// Returns a service based on service id
        /// </summary>
        /// <param name="id">Service Primary Key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A service in the system matching up to the service id primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// GET: api/ServiceData/FindService/5
        /// </example>
        [ResponseType(typeof(Service))]
        [HttpGet]
        public IHttpActionResult FindService(int id)
        {
            Service service = db.Services.Find(id);
            if (service == null)
            {
                return NotFound();
            }

            ServiceDto serviceDto = new ServiceDto()
            {
                ServiceId = service.ServiceId,
                Name = service.Name,
                Duration = service.Duration,
                Cost = service.Cost
            };

            return Ok(serviceDto);
        }

        /// <summary>
        /// Gets service and related employees details based on employeeid
        /// </summary>
        /// <param name="id">Employee primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all services in the database, including their associated employees that match to a particular employee id
        /// </returns>
        /// <example>
        /// GET: api/ServiceData/ServicesAssignedToEmployee/2
        /// </example>
        [HttpGet]
        public IHttpActionResult ServicesAssignedToEmployee(int id)
        {
            List<Service> services = db.Services.Where(x => x.Employees.Any(y => y.EmployeeId == id)).ToList();
            List<ServiceDto> serviceDtos = new List<ServiceDto>();

            services.ForEach(x => serviceDtos.Add(new ServiceDto()
            {
                ServiceId = x.ServiceId,
                Name = x.Name,
                Duration = x.Duration,
                Cost = x.Cost
            }));
            return Ok(serviceDtos);
        }

        /// <summary>
        /// Returns list of services not assigned to an employee
        /// </summary>
        /// <param name="id">Employee primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Services in the database not assigned to an employee
        /// </returns>
        /// <example>
        /// GET: api/ServiceData/ServicesNotAssignedToEmployee/2
        /// </example>
        [HttpGet]
        public IHttpActionResult ServicesNotAssignedToEmployee(int id)
        {
            List<Service> services = db.Services.Where(x => !x.Employees.Any(y => y.EmployeeId == id)).ToList();
            List<ServiceDto> serviceDtos = new List<ServiceDto>();

            services.ForEach(x => serviceDtos.Add(new ServiceDto()
            {
                ServiceId = x.ServiceId,
                Name = x.Name,
                Duration = x.Duration,
                Cost = x.Cost
            }));
            return Ok(serviceDtos);
        }

        /// <summary>
        /// Updates a particular Service in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Service ID primary key</param>
        /// <param name="service">JSON FORM DATA of an Service</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// PUT: api/ServiceData/UpdateService/5
        /// FORM DATA: Service JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateService(int id, Service service)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != service.ServiceId)
            {
                return BadRequest();
            }

            db.Entry(service).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        /// <summary>
        /// Adds a Service to the system
        /// </summary>
        /// <param name="service">JSON FORM DATA of an Service</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Service ID, Service Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/ServiceData/AddService
        /// FORM DATA: Service JSON Object
        /// </example>
        [ResponseType(typeof(Service))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddService(Service service)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Services.Add(service);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = service.ServiceId }, service);
        }


        /// <summary>
        /// Deletes a Service from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the Service</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/ServiceData/DeleteService/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Service))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteService(int id)
        {
            Service service = db.Services.Find(id);
            if (service == null)
            {
                return NotFound();
            }

            db.Services.Remove(service);
            db.SaveChanges();

            return Ok(service);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ServiceExists(int id)
        {
            return db.Services.Count(e => e.ServiceId == id) > 0;
        }
    }
}