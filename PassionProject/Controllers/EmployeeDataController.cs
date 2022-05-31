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
    public class EmployeeDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/EmployeeData/ListEmployees
        [HttpGet]
        public IEnumerable<EmployeeDto> ListEmployees()
        {
            List<Employee> employees = db.Employees.ToList();
            List<EmployeeDto> employeeDtos = new List<EmployeeDto>();
            employees.ForEach(a => employeeDtos.Add(new EmployeeDto()
            {
                EmployeeId = a.EmployeeId,
                Fname = a.Fname,
                Lname = a.Lname,
                DOJ = a.DOJ,
                Bio = a.Bio
            }));
            return employeeDtos;
        }

        // GET: api/EmployeeData/FindEmployee/5
        [ResponseType(typeof(Employee))]
        [HttpGet]
        public IHttpActionResult FindEmployee(int id)
        {
            Employee employee = db.Employees.Find(id);

            if (employee == null)
            {
                return NotFound();
            }

            EmployeeDto employeedto = new EmployeeDto()
            {
                EmployeeId = employee.EmployeeId,
                Fname = employee.Fname,
                Lname = employee.Lname,
                DOJ = employee.DOJ,
                Bio = employee.Bio
            };

            return Ok(employee);
        }

        // GET: api/EmployeeData/GetEmployeesByServiceId/2
        [HttpGet]
        [ResponseType(typeof(List<EmployeeDto>))]
        public IHttpActionResult GetEmployeesByServiceId(int id)
        {
            List<Employee> employees = db.Employees.Where(x=>x.Services.Any(y=>y.ServiceId == id)).ToList();
            List<EmployeeDto> employeesDto = new List<EmployeeDto>();

            employees.ForEach(a => employeesDto.Add(new EmployeeDto()
            {
                EmployeeId = a.EmployeeId,
                Fname = a.Fname,
                Lname = a.Lname,
                DOJ = a.DOJ,
                Bio = a.Bio
            }));

            return Ok(employeesDto);
        }

        // PUT: api/EmployeeData/UpdateEmployee5
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateEmployee(int id, Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employee.EmployeeId)
            {
                return BadRequest();
            }

            db.Entry(employee).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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
        /// Used to Unassociate service from the Employee
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/EmployeeData/unassociateservicewithemployee/{employeeId}/{serviceId}")]
        public IHttpActionResult UnassociateServiceWithEmployee(int employeeId, int serviceId)
        {
            Employee employee = db.Employees.Include(x => x.Services).Where(y => y.EmployeeId == employeeId).FirstOrDefault();
            Service service = db.Services.Find(serviceId);

            if (employee == null || service == null)
            {
                return NotFound();
            }

            employee.Services.Remove(service);
            db.SaveChanges();

            return Ok();
        }


        /// <summary>
        /// Used to associate service to an employee
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/EmployeeData/associateservicewithemployee/{employeeId}/{serviceId}")]
        public IHttpActionResult AssociateServiceWithEmployee(int employeeId, int serviceId)
        {
            Employee employee = db.Employees.Include(x => x.Services).Where(y => y.EmployeeId == employeeId).FirstOrDefault();
            Service service = db.Services.Find(serviceId);

            if (employee == null || service == null)
            {
                return NotFound();
            }

            employee.Services.Add(service);
            db.SaveChanges();

            return Ok();
        }


        // POST: api/EmployeeData/AddEmployee
        [ResponseType(typeof(Employee))]
        [HttpPost]
        public IHttpActionResult AddEmployee(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Employees.Add(employee);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = employee.EmployeeId }, employee);
        }

        // POST: api/EmployeeData/DeleteEmployee/5
        [HttpPost]
        [ResponseType(typeof(Employee))]
        public IHttpActionResult DeleteEmployee(int id)
        {
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            db.Employees.Remove(employee);
            db.SaveChanges();

            return Ok(employee);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EmployeeExists(int id)
        {
            return db.Employees.Count(e => e.EmployeeId == id) > 0;
        }
    }
}