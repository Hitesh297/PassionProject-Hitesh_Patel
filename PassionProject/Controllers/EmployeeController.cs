using PassionProject.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };
            client = new HttpClient(handler);
            //client.BaseAddress = new Uri("https://localhost:44364/api/");
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["apiServer"]);

        }

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
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

            url = "appointmentdata/FindAppointmentsByEmployee/" + id;
            response = client.GetAsync(url).Result;
            selectedEmployee.Appointments = response.Content.ReadAsAsync<List<AppointmentDto>>().Result;

            //IEnumerable<IGrouping<DateTime,AppointmentDto>> appointmentsGroup = selectedEmployee.Appointments.GroupBy(x => x.StartTime.Date);
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
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Employee employee)
        {
            try
            {
                GetApplicationCookie();
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
        [Authorize]
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

        //GET: /Employee/UnassociateService?id={EmployeeId}&serviceId={ServiceId}
        [HttpGet]
        [Authorize]
        public ActionResult UnassociateService(int id, int serviceId)
        {
            try
            {
                GetApplicationCookie();
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

        //POST: Employee/AssociateService/{employeeId}
        [HttpPost]
        [Authorize]
        public ActionResult AssociateService(int id, int serviceId)
        {
            try
            {
                GetApplicationCookie();
                string url = "employeedata/associateservicewithemployee/" + id + "/" + serviceId;
                HttpContent content = new StringContent("");
                content.Headers.ContentType.MediaType = "application/json";
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Edit/" + id);
                    //return Redirect($"{Url.Action("Edit", new { id = id })}#associate-form");
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
        [Authorize]
        public ActionResult Edit(int id, Employee employee, HttpPostedFileBase EmployeePic)
        {
            try
            {
                GetApplicationCookie();
                string url = "employeedata/updateemployee/" + id;
                string jsonpayload = jss.Serialize(employee);
                HttpContent content = new StringContent(jsonpayload);
                content.Headers.ContentType.MediaType = "application/json";
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                if (response.IsSuccessStatusCode && EmployeePic != null)
                {
                    url = "EmployeeData/UploadEmployeePic/" + id;
                    MultipartFormDataContent requestcontent = new MultipartFormDataContent();

                    HttpContent imagecontent = new StreamContent(EmployeePic.InputStream);
                    requestcontent.Add(imagecontent, "EmployeePic", EmployeePic.FileName);

                    response = client.PostAsync(url, requestcontent).Result;

                    return RedirectToAction("List");
                }
                else if (response.IsSuccessStatusCode)
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
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "employeedata/findemployee/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            EmployeeDto selectedEmployee = response.Content.ReadAsAsync<EmployeeDto>().Result;
            return View(selectedEmployee);
        }

        // POST: Employee/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            try
            {
                GetApplicationCookie();
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
