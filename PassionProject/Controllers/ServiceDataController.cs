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

        // GET: api/ServiceData/ListServices
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

        // GET: api/ServiceData/FindService/5
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

        // GET: api/ServiceData/ServicesAssignedToEmployee/2
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


        // GET: api/ServiceData/ServicesNotAssignedToEmployee/2
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

        // PUT: api/ServiceData/UpdateService/5
        [ResponseType(typeof(void))]
        [HttpPost]
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

        // POST: api/ServiceData/AddService
        [ResponseType(typeof(Service))]
        [HttpPost]
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

        // POST: api/ServiceData/DeleteService/5
        [ResponseType(typeof(Service))]
        [HttpPost]
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