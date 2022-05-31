using PassionProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PassionProject.Controllers
{
    public class EmployeeController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        static EmployeeController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44364/api/");

        }
        // GET: Employee/List
        public ActionResult List()
        {
            string url = "employeedata/listemployees";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<EmployeeDto> employees = response.Content.ReadAsAsync<IEnumerable<EmployeeDto>>().Result;
            return View(employees);
        }

        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            string url = "employeedata/findemployee/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            EmployeeDto selectedEmployee = response.Content.ReadAsAsync<EmployeeDto>().Result;
            return View(selectedEmployee);
        }

        [HttpGet]
        public JsonResult GetEmployeesByServiceId(int id)
        {
            string url = "employeedata/GetEmployeesByServiceId/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<EmployeeDto> employees = response.Content.ReadAsAsync<IEnumerable<EmployeeDto>>().Result;
            return Json(employees);
        }

        // GET: Employee/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        public ActionResult Create(Employee employee)
        {
            try
            {
                string url = "employeedata/addemployee";


                string jsonpayload = jss.Serialize(employee);

                HttpContent content = new StringContent(jsonpayload);
                content.Headers.ContentType.MediaType = "application/json";

                HttpResponseMessage response = client.PostAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("List");
                }
                else
                {
                    return View("Error");
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            EmployeeVM viewModel = new EmployeeVM();

            string url = "employeedata/findemployee/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            EmployeeDto selectedEmployee = response.Content.ReadAsAsync<EmployeeDto>().Result;

            url = "ServiceData/ServicesAssignedToEmployee/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ServiceDto> servicesAssignedToEmployee = response.Content.ReadAsAsync<IEnumerable<ServiceDto>>().Result;

            url = "ServiceData/ServicesNotAssignedToEmployee/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ServiceDto> ServicesNotAssignedToEmployee = response.Content.ReadAsAsync<IEnumerable<ServiceDto>>().Result;

            viewModel.Employee = selectedEmployee;
            viewModel.ServicesAssigned = servicesAssignedToEmployee;
            viewModel.ServicesAvailable = ServicesNotAssignedToEmployee;


            return View(viewModel);
        }

        [HttpGet]
        public ActionResult UnassociateService(int id, int serviceId)
        {
            try
            {
                string url = "employeedata/unassociateservicewithemployee/" + id + "/" + serviceId;
                HttpContent content = new StringContent("");
                content.Headers.ContentType.MediaType = "application/json";
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Edit/" + id);
                }
                else
                {
                    return View("Error");
                }

            }
            catch
            {

                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult AssociateService(int id, int serviceId)
        {
            try
            {
                string url = "employeedata/associateservicewithemployee/" + id + "/" + serviceId;
                HttpContent content = new StringContent("");
                content.Headers.ContentType.MediaType = "application/json";
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Edit/" + id);
                }
                else
                {
                    return View("Error");
                }

            }
            catch
            {

                return View("Error");
            }
        }

        // POST: Employee/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Employee employee)
        {
            try
            {
                string url = "employeedata/updateemployee/" + id;
                string jsonpayload = jss.Serialize(employee);
                HttpContent content = new StringContent(jsonpayload);
                content.Headers.ContentType.MediaType = "application/json";
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("List");
                }
                else
                {
                    return View("Error");
                }
            }
            catch
            {
                return View("Error");
            }
        }

        // GET: Employee/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "employeedata/findemployee/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            EmployeeDto selectedEmployee = response.Content.ReadAsAsync<EmployeeDto>().Result;
            return View(selectedEmployee);
        }

        // POST: Employee/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                string url = "employeedata/deleteemployee/" + id;
                HttpContent content = new StringContent("");
                content.Headers.ContentType.MediaType = "application/json";
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("List");
                }
                else
                {
                    return View("Error");
                }
            }
            catch
            {
                return View("Error");
            }
        }
    }
}
