using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NexDevs.Context;
using NexDevs.Models;
using System.Net.Http.Headers;
using System.Net;

namespace NexDevs.Controllers
{
    public class UsersController : Controller
    {

        private NetworkAPI networkAPI;
        private HttpClient client;

        public UsersController()
        {
            networkAPI = new NetworkAPI();
            client = networkAPI.Initial();
        }

        public async Task<IActionResult> Index(string search)
        {
            List<User> listUsers = new List<User>();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync("Users/Listado");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                listUsers = JsonConvert.DeserializeObject<List<User>>(result);
            }

            if (!string.IsNullOrEmpty(search))
            {
                listUsers = listUsers
                                .Where(user => user.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase))
                                .ToList();
            }

            return View(listUsers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] User user, IFormFile profilePictureUrl)
        {
            if (ModelState.IsValid)
            {
                // Verifica si se ha subido una imagen
                if (profilePictureUrl != null && profilePictureUrl.Length > 0)
                {
                    // Define el directorio donde se guarda la imagen
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users");

                    // Verifica que el directorio exita y si no se crea
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Se genera un nombre único para la imagen
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + profilePictureUrl.FileName;

                    //Se quitan los espacios en blanco dentro del nombre de la imagen si tiene
                    uniqueFileName = uniqueFileName.Replace(" ", "_");

                    //Se indica la ruta fisca donde se almacena la foto
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Guarda la imagen en la carpeta del servidor
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        //se copia la imagen en nuestra app
                        await profilePictureUrl.CopyToAsync(fileStream);
                    }

                    // Se asigna la URL de la imagen de user
                    user.ProfilePictureUrl = "/images/users/" + uniqueFileName;
                }
                //se asigna 0 a la id para que no de problemas 
                user.UserId = 0;

                //En caso de que la imagen venga vacia le decimos que ponga N/D
                if (profilePictureUrl == null)
                {
                    user.ProfilePictureUrl = "N/D";
                }

                //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

                var add = client.PostAsJsonAsync<User>("Users/Agregar", user);
                await add;

                var result = add.Result;

                // if (ValidateSession(response.StatusCode) == false)
                // {
                //     return RedirectToAction("Logout", "Users");
                // }

                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    TempData["Mensaje"] = "No se logró registrar el usuario";

                    return View(user);
                }
            }
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = new User();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Users/Consultar?UserId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                user = JsonConvert.DeserializeObject<User>(result);
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] User user, IFormFile profilePictureUrl)
        {
            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            if (ModelState.IsValid)
            {
                if (profilePictureUrl != null && profilePictureUrl.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + profilePictureUrl.FileName;

                    uniqueFileName = uniqueFileName.Replace(" ", "_");

                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePictureUrl.CopyToAsync(fileStream);
                    }

                    user.ProfilePictureUrl = "/images/users/" + uniqueFileName;
                }

                //Verificamos si el campo de la imagen viene vacio y de asignamos la misma imagen que tenía para que no de problemas
                if (profilePictureUrl == null)
                {
                    HttpResponseMessage response = await client.GetAsync($"Users/Consultar?UserId={user.UserId}");
                    var resultado = response.Content.ReadAsStringAsync().Result;
                    var userDtos = JsonConvert.DeserializeObject<User>(resultado);
                    user.ProfilePictureUrl = userDtos.ProfilePictureUrl;
                }

                var result = await client.PutAsJsonAsync<User>("Users/Editar", user);

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

                    return View(user);
                }
            }
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = new User();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage mensaje = await client.GetAsync($"Users/Consultar?UserId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (mensaje.IsSuccessStatusCode)
            {
                var result = mensaje.Content.ReadAsStringAsync().Result;

                //conversion json a obj
                user = JsonConvert.DeserializeObject<User>(result);
            }

            return View(user);
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

            HttpResponseMessage response = await client.DeleteAsync($"Users/Eliminar?UserId={id}");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = new User();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Users/Consultar?UserId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                user = JsonConvert.DeserializeObject<User>(result);
            }

            return View(user);
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
