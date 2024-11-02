using NexDevs.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis;

namespace NexDevs.Controllers
{
    [Authorize]// protect routes
    public class WorkSkillsController : Controller
    {
        private NetworkAPI networkAPI;
        private HttpClient client;

        public WorkSkillsController()
        {
            networkAPI = new NetworkAPI();
            client = networkAPI.Initial();
        }

        public async Task<IActionResult> Index()
        {
            List<WorkSkill> listWorkSkills = new List<WorkSkill>();


            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync("WorkSkills/Listado");

            if (ValidateSession(response.StatusCode) == false)
            {
                return RedirectToAction("Logout", "Users");
            }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                listWorkSkills = JsonConvert.DeserializeObject<List<WorkSkill>>(result);
            }

            return View(listWorkSkills);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] WorkSkill workSkill)
        {
            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            var add = client.PostAsJsonAsync<WorkSkill>("WorkSkills/Agregar", workSkill);
            await add;

            var result = add.Result;

            if (ValidateSession(result.StatusCode) == false)
            {
                return RedirectToAction("Logout", "Users");
            }

            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "WorkSkills");
            }
            else
            {
                TempData["Mensaje"] = "No se logró registrar la categoria";

                return View(workSkill);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var workSkill = new WorkSkill();

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"WorkSkills/ConsultarId?workSkillId={id}");

            if (ValidateSession(response.StatusCode) == false)
            {
                return RedirectToAction("Logout", "Users");
            }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                workSkill = JsonConvert.DeserializeObject<WorkSkill>(result);
            }

            return View(workSkill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] WorkSkill workSkill)
        {
            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            var result = await client.PutAsJsonAsync<WorkSkill>("WorkSkills/Editar", workSkill);

            if (ValidateSession(result.StatusCode) == false)
            {
                return RedirectToAction("Logout", "Users");
            }

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
            var workSkill = new WorkSkill();
            
            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage mensaje = await client.GetAsync($"WorkSkills/ConsultarId?workSkillId={id}");

            if (ValidateSession(mensaje.StatusCode) == false)
            {
                return RedirectToAction("Logout", "Users");
            }

            if (mensaje.IsSuccessStatusCode)
            {
                var result = mensaje.Content.ReadAsStringAsync().Result;

                //conversion json a obj
                workSkill = JsonConvert.DeserializeObject<WorkSkill>(result);
            }

            return View(workSkill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.DeleteAsync($"WorkSkills/Eliminar?id={id}");
          
            if (ValidateSession(response.StatusCode) == false)
            {
                return RedirectToAction("Logout", "Users");
            
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var workSkill = new WorkSkill();

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"WorkSkills/ConsultarId?workSkillId={id}");

            if (ValidateSession(response.StatusCode) == false)
            {
                return RedirectToAction("Logout", "Users");
            }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                workSkill = JsonConvert.DeserializeObject<WorkSkill>(result);
            }

            return View(workSkill);
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
