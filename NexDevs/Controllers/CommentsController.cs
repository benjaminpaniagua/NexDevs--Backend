using NexDevs.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace NexDevs.Controllers
{
    [Authorize]// protect routes
    public class CommentsController : Controller
    {
        private NetworkAPI networkAPI;
        private HttpClient client;

        public CommentsController()
        {
            networkAPI = new NetworkAPI();
            client = networkAPI.Initial();
        }

        public async Task<IActionResult> Index(string search)
        {
            List<Comment> listComments = new List<Comment>();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync("Comments/Listado");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                listComments = JsonConvert.DeserializeObject<List<Comment>>(result);
            }

            if (!string.IsNullOrEmpty(search))
            {
                listComments = listComments
                                .Where(comment => comment.ContentComment.Contains(search, StringComparison.OrdinalIgnoreCase))
                                .ToList();
            }

            return View(listComments);
        }

        [HttpGet]
        public IActionResult Create() 
        { 
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.CreateAt = DateTime.Now;
                var add = client.PostAsJsonAsync<Comment>("Comments/Agregar", comment);
                await add;

                var result = add.Result;

                // if (ValidateSession(response.StatusCode) == false)
                // {
                //     return RedirectToAction("Logout", "Users");
                // }

                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Comments");
                }
                else
                {
                    TempData["Mensaje"] = "No se logró registrar el comentario";

                    return View(comment);
                }
            }
            return View(comment);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var comment = new Comment();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Comments/ConsultarId?commentId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                comment = JsonConvert.DeserializeObject<Comment>(result);
            }

            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var result = await client.PutAsJsonAsync<Comment>("Comments/Editar", comment);

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

                    return View(comment);
                }
            }
            return View(comment);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = new Comment();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage mensaje = await client.GetAsync($"Comments/ConsultarId?commentId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (mensaje.IsSuccessStatusCode)
            {
                var result = mensaje.Content.ReadAsStringAsync().Result;

                //conversion json a obj
                comment = JsonConvert.DeserializeObject<Comment>(result);
            }

            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await client.DeleteAsync($"Comments/Eliminar?commentId={id}");
            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var comment = new Comment();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Comments/ConsultarId?commentId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                comment = JsonConvert.DeserializeObject<Comment>(result);
            }

            return View(comment);
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
