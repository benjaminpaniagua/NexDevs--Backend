using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NexDevs.Models;
using System.Net.Http.Headers;
using System.Net;

namespace NexDevs.Controllers
{
    [Authorize]// protect routes
    public class ReviewsController : Controller
    {
        private NetworkAPI networkAPI;
        private HttpClient client;

        public ReviewsController()
        {
            networkAPI = new NetworkAPI();
            client = networkAPI.Initial();
        }

        public async Task<IActionResult> Index(string search)
        {
            List<Review> listReviews = new List<Review>();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync("Reviews/Listado");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                listReviews = JsonConvert.DeserializeObject<List<Review>>(result);
            }

            if (!string.IsNullOrEmpty(search))
            {
                listReviews = listReviews
                    .Where(review => review.ReviewComment.Contains(search, StringComparison.OrdinalIgnoreCase))
                                .ToList();
            }

            return View(listReviews);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] Review review)
        {
            if (ModelState.IsValid)
            {
                var add = client.PostAsJsonAsync<Review>("Reviews/Agregar", review);
                await add;

                var result = add.Result;

                // if (ValidateSession(response.StatusCode) == false)
                // {
                //     return RedirectToAction("Logout", "Users");
                // }

                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Reviews");
                }
                else
                {
                    TempData["Mensaje"] = "No se logró registrar la skill";

                    return View(review);
                }
            }
            return View(review);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var review = new Review();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Reviews/Consultar?reviewId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                review = JsonConvert.DeserializeObject<Review>(result);
            }

            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] Review review)
        {
            if (ModelState.IsValid)
            {
                var result = await client.PutAsJsonAsync<Review>("Reviews/Editar", review);

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

                    return View(review);
                }
            }
            return View(review);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var review = new Review();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage mensaje = await client.GetAsync($"Reviews/Consultar?reviewId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (mensaje.IsSuccessStatusCode)
            {
                var result = mensaje.Content.ReadAsStringAsync().Result;

                //conversion json a obj
                review = JsonConvert.DeserializeObject<Review>(result);
            }

            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await client.DeleteAsync($"Reviews/Eliminar?reviewId=={id}");
            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var review = new Review();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Reviews/Consultar?reviewId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                review = JsonConvert.DeserializeObject<Review>(result);
            }

            return View(review);
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
