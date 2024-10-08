﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using PassionProject.Models;

namespace PassionProject.Controllers
{
    public class EmployeeDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns list of all employees in the system
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all employees in the database
        /// </returns>
        /// <example>
        /// GET: api/EmployeeData/ListEmployees
        /// </example>
        [HttpGet]
        public IHttpActionResult ListEmployees()
        {
            List<Employee> employees = db.Employees.ToList();
            List<EmployeeDto> employeeDtos = new List<EmployeeDto>();
            employees.ForEach(a => employeeDtos.Add(new EmployeeDto()
            {
                EmployeeId = a.EmployeeId,
                Fname = a.Fname,
                Lname = a.Lname,
                DOJ = a.DOJ,
                Bio = a.Bio,
                EmployeeHasPic = a.EmployeeHasPic,
                PicExtension = a.PicExtension
            }));
            return Ok(employeeDtos);
        }


        /// <summary>
        /// Returns Employee details based on id
        /// </summary>
        /// <param name="id">Employee primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An employee in the system matching up to the employee ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
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
                Bio = employee.Bio,
                EmployeeHasPic = employee.EmployeeHasPic,
                PicExtension = employee.PicExtension
            };

            return Ok(employee);
        }


        /// <summary>
        /// Gathers information about all Employees related to a particular ServiceId
        /// </summary>
        /// <param name="id">Service primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all employees in the database, including their associated sevices matched with a particular service ID
        /// </returns>
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


        /// <summary>
        /// Updates a particular employee in the system with POST Data input
        /// </summary>
        /// <param name="id">Employee primary key</param>
        /// <param name="employee">JSON form data of an Employee</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// PUT: api/EmployeeData/UpdateEmployee/5
        /// FORM DATA: Employee JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
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
        /// <param name="employeeId">Employee primary key</param>
        /// <param name="serviceId">Service primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/EmployeeData/unassociateservicewithemployee/1/2
        /// </example>
        [HttpPost]
        [Authorize]
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
        /// Associates a service to an employee
        /// </summary>
        /// <param name="employeeId">Employee primary key</param>
        /// <param name="serviceId">Service primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/EmployeeData/associateservicewithemployee/1/2
        /// </example>
        [HttpPost]
        [Authorize]
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

        /// <summary>
        /// Adds an employee to the system
        /// </summary>
        /// <param name="employee">JSON form data of Employee</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Employee ID, Employee Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/EmployeeData/AddEmployee
        /// FORM DATA: Employee JSON Object
        /// </example>
        [ResponseType(typeof(Employee))]
        [HttpPost]
        [Authorize]
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

        /// <summary>
        /// Deletes an employee from the system by it's id.
        /// </summary>
        /// <param name="id">Employee Primary Key</param>
        /// <returns>
        ///  HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/EmployeeData/DeleteEmployee/5
        /// FORM DATA: (empty)
        /// </example>
        [HttpPost]
        [ResponseType(typeof(Employee))]
        [Authorize]
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

        /// <summary>
        /// Receives employee picture data, uploads it to the webserver and updates the employee HasPic property
        /// </summary>
        /// <param name="id">the employee id</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// POST: api/animalData/UpdateanimalPic/3
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
        [HttpPost]
        public IHttpActionResult UploadEmployeePic(int id)
        {

            bool haspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {

                int numfiles = HttpContext.Current.Request.Files.Count;

                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var employeePic = HttpContext.Current.Request.Files[0];
                    if (employeePic.ContentLength > 0)
                    {
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(employeePic.FileName).Substring(1);
                        if (valtypes.Contains(extension, StringComparer.InvariantCultureIgnoreCase))
                        {
                            try
                            {
                                string fn = id + "." + extension;
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/Employees/"), fn);

                                employeePic.SaveAs(path);

                                haspic = true;
                                picextension = extension;

                                Employee Selectedemployee = db.Employees.Find(id);
                                Selectedemployee.EmployeeHasPic = haspic;
                                Selectedemployee.PicExtension = extension;
                                db.Entry(Selectedemployee).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                return BadRequest();
                            }
                        }
                    }

                }
                else
                {
                    Employee Selectedemployee = db.Employees.Find(id);
                    Selectedemployee.EmployeeHasPic = haspic;
                    Selectedemployee.PicExtension = null;
                    db.Entry(Selectedemployee).State = EntityState.Modified;

                    db.SaveChanges();
                }

                return Ok();
            }
            else
            {
                //not multipart form data
                return BadRequest();

            }

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