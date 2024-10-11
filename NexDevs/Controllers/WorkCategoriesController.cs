using NexDevs.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace NexDevs.Controllers
{
    [Authorize]// protect routes
    public class WorkCategoriesController : Controller
    {
        private NetworkAPI networkAPI;
        private HttpClient client;

        public WorkCategoriesController()
        {
            networkAPI = new NetworkAPI();
            client = networkAPI.Initial();
        }

        public async Task<IActionResult> Index()
        {
            List<WorkCategory> listWorkCategories = new List<WorkCategory>();


            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync("WorkCategories/Listado");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                listWorkCategories = JsonConvert.DeserializeObject<List<WorkCategory>>(result);
            }

            return View(listWorkCategories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] WorkCategory workCategory)
        {
            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            var add = client.PostAsJsonAsync<WorkCategory>("WorkCategories/Agregar", workCategory);
            await add;

            var result = add.Result;

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "WorkCategories");
            }
            else
            {
                TempData["Mensaje"] = "No se logró registrar la categoria";

                return View(workCategory);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var workCategory = new WorkCategory();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"WorkCategories/ConsultarId?id={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                workCategory = JsonConvert.DeserializeObject<WorkCategory>(result);
            }

            return View(workCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] WorkCategory workCategory)
        {
            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            var result = await client.PutAsJsonAsync<WorkCategory>("WorkCategories/Editar", workCategory);

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Mensaje"] = "Datos incorrectos";

                return View(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var workCategory = new WorkCategory();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage mensaje = await client.GetAsync($"WorkCategories/ConsultarId?id={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (mensaje.IsSuccessStatusCode)
            {
                var result = mensaje.Content.ReadAsStringAsync().Result;

                //conversion json a obj
                workCategory = JsonConvert.DeserializeObject<WorkCategory>(result);
            }

            return View(workCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            HttpResponseMessage response = await client.DeleteAsync($"WorkCategories/Eliminar?id={id}");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var workCategory = new WorkCategory();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"WorkCategories/ConsultarId?id={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                workCategory = JsonConvert.DeserializeObject<WorkCategory>(result);
            }

            return View(workCategory);
        }

        private AuthenticationHeaderValue AutorizacionToken()
        {
            var token = HttpContext.Session.GetString("token");

            AuthenticationHeaderValue autorizacion = null;

            if (token != null && token.Length != 0)
            {
                autorizacion = new AuthenticationHeaderValue("Bearer", token);
            }

            return autorizacion;
        }

        private bool ValidateSession(HttpStatusCode result)
        {
            //The token has expired so the session must be closed
            if (result == HttpStatusCode.Unauthorized)
            {
                TempData["MensajeSesion"] = "Su sesion ha expirado o no es válida";
                return false;
            }
            else
            {
                TempData["MensajeSesion"] = null;
                return true;
            }
        }
    }
}
