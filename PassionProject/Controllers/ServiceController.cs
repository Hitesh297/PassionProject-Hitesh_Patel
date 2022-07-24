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
    public class ServiceController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        static ServiceController()
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
        // GET: Service/List
        public ActionResult List()
        {
            string url = "servicedata/listservices";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<ServiceDto> services = response.Content.ReadAsAsync<IEnumerable<ServiceDto>>().Result;
            return View(services);
        }

        // GET: Service/Details/5
        public ActionResult Details(int id)
        {
            string url = "servicedata/findservice/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            ServiceDto selectedService = response.Content.ReadAsAsync<ServiceDto>().Result;
            return View(selectedService);
        }

        // GET: Service/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Service/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Service service)
        {
            try
            {
                GetApplicationCookie();
                string url = "servicedata/addservice";

                string jsonpayload = jss.Serialize(service);

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

        // GET: Service/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            string url = "servicedata/findservice/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ServiceDto selectedService = response.Content.ReadAsAsync<ServiceDto>().Result;
            return View(selectedService);
        }

        // POST: Service/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Edit(int id, Service service)
        {
            try
            {
                GetApplicationCookie();
                string url = "servicedata/updateservice/" + id;
                string jsonpayload = jss.Serialize(service);
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

        // GET: Service/Delete/5
        [HttpGet]
        [Authorize]
        public ActionResult ConfirmDelete(int id)
        {
            string url = "servicedata/findservice/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ServiceDto selectedService = response.Content.ReadAsAsync<ServiceDto>().Result;
            return View(selectedService);
        }

        // POST: Service/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            try
            {
                GetApplicationCookie();
                string url = "servicedata/deleteservice/" + id;
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
                return View();
            }
        }
    }
}
