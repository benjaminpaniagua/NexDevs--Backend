using NexDevs.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;

namespace NexDevs.Controllers
{
    public class SkillsController : Controller
    {

        private NetworkAPI networkAPI;
        private HttpClient client;

        public SkillsController()
        {
            networkAPI = new NetworkAPI();
            client = networkAPI.Initial();
        }

        public async Task<IActionResult> Index(string search)
        {
            List<Skill> listSkills = new List<Skill>();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync("Skills/Listado");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                listSkills = JsonConvert.DeserializeObject<List<Skill>>(result);
            }

            if (!String.IsNullOrEmpty(search))
            {
                listSkills = listSkills
                    .Where(skill => skill.SkillName.Contains(search, StringComparison.OrdinalIgnoreCase))
                                .ToList();
            }

            return View(listSkills);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] Skill skill)
        {
            if (ModelState.IsValid)
            {
                var add = client.PostAsJsonAsync<Skill>("Skills/Agregar", skill);
                await add;

                var result = add.Result;

                // if (ValidateSession(response.StatusCode) == false)
                // {
                //     return RedirectToAction("Logout", "Users");
                // }

                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Skills");
                }
                else
                {
                    TempData["Mensaje"] = "No se logró registrar la skill";

                    return View(skill);
                }
            }
            return View(skill);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var skill = new Skill();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Skills/Consultar?Id={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                skill = JsonConvert.DeserializeObject<Skill>(result);
            }

            return View(skill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] Skill skill)
        {
            if (ModelState.IsValid)
            {
                var result = await client.PutAsJsonAsync<Skill>("Skills/Editar", skill);

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

                    return View(skill);
                }
            }
            return View(skill);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var skill = new Skill();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage mensaje = await client.GetAsync($"Skills/Consultar?Id={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (mensaje.IsSuccessStatusCode)
            {
                var result = mensaje.Content.ReadAsStringAsync().Result;

                //conversion json a obj
                skill = JsonConvert.DeserializeObject<Skill>(result);
            }

            return View(skill);
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

            HttpResponseMessage response = await client.DeleteAsync($"Skills/Eliminar?Id={id}");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var skill = new Skill();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Skills/Consultar?Id={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                skill = JsonConvert.DeserializeObject<Skill>(result);
            }

            return View(skill);
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
