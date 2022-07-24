using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
    public class AppointmentController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static AppointmentController()
        {
            client = new HttpClient();
            //client.BaseAddress = new Uri("https://localhost:44364/api/");
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["apiServer"]);
        }
        // GET: Appointment
        public ActionResult Index()
        {
            return View();
        }

        // GET: Appointment/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Appointment/Create
        public ActionResult Create()
        {
            string url = "servicedata/listservices";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<ServiceDto> services = response.Content.ReadAsAsync<IEnumerable<ServiceDto>>().Result;
            return View(services);
        }

        // POST: Appointment/Create
        [HttpPost]
        public ActionResult Create(Appointment appointment)
        {
            try
            {
                string url = "servicedata/findservice/" + appointment.ServiceId;
                HttpResponseMessage response = client.GetAsync(url).Result;
                ServiceDto selectedService = response.Content.ReadAsAsync<ServiceDto>().Result;

                appointment.EndTime = appointment.StartTime.AddMinutes(selectedService.Duration);
                url = "appointmentdata/addappointment";

                // Javascript serializer has an issue with date coversion, the receiver deserializes
                // a different time then what is sent, Newtonsoft.Json doesn't have that problem
                string jsonpayload = JsonConvert.SerializeObject(appointment);

                HttpContent content = new StringContent(jsonpayload);
                content.Headers.ContentType.MediaType = "application/json";

                response = client.PostAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Home");
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

        // GET: Appointment/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Appointment/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Appointment/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Appointment/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
