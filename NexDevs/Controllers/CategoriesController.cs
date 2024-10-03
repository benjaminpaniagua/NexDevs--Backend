using NexDevs.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;

namespace NexDevs.Controllers
{
    public class CategoriesController : Controller
    {
        private NetworkAPI networkAPI;
        private HttpClient client;

        public CategoriesController()
        {
            networkAPI = new NetworkAPI();
            client = networkAPI.Initial();
        }

        public async Task<IActionResult> Index(string search)
        {
            List<Category> listCategories = new List<Category>();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync("Categories/Listado");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                listCategories = JsonConvert.DeserializeObject<List<Category>>(result);
            }

            if (!string.IsNullOrEmpty(search))
            {
                listCategories = listCategories
                                .Where(category => category.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase))
                                .ToList();
            }

            return View(listCategories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] Category category, IFormFile categoryImageUrl)
        {
            if (ModelState.IsValid)
            {
                // Verifica si se ha subido una imagen
                if (categoryImageUrl != null && categoryImageUrl.Length > 0)
                {
                    // Define el directorio donde se guarda la imagen
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/categories");

                    // Verifica que el directorio exita y si no se crea
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Se genera un nombre único para la imagen
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + categoryImageUrl.FileName;

                    //Se quitan los espacios en blanco dentro del nombre de la imagen si tiene
                    uniqueFileName = uniqueFileName.Replace(" ", "_");

                    //Se indica la ruta fisca donde se almacena la foto
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Guarda la imagen en la carpeta del servidor
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        //se copia la imagen en nuestra app
                        await categoryImageUrl.CopyToAsync(fileStream);
                    }

                    // Se asigna la URL de la imagen de category
                    category.CategoryImageUrl = "/images/categories/" + uniqueFileName;
                }
                //se asigna 0 a la id para que no de problemas 
                category.CategoryId = 0;

                //En caso de que la imagen venga vacia le decimos que ponga N/D
                if (categoryImageUrl == null)
                {
                    category.CategoryImageUrl = "N/D";
                }

                //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

                var add = client.PostAsJsonAsync<Category>("Categories/Agregar", category);
                await add;

                var result = add.Result;

                // if (ValidateSession(response.StatusCode) == false)
                // {
                //     return RedirectToAction("Logout", "Users");
                // }

                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Categories");
                }
                else
                {
                    TempData["Mensaje"] = "No se logró registrar la categoria";

                    return View(category);
                }
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = new Category();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Categories/Consultar?categoryId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                category = JsonConvert.DeserializeObject<Category>(result);
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] Category category, IFormFile categoryImageUrl)
        {

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            if (ModelState.IsValid)
            {
                if (categoryImageUrl != null && categoryImageUrl.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/categories");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + categoryImageUrl.FileName;

                    uniqueFileName = uniqueFileName.Replace(" ", "_");

                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await categoryImageUrl.CopyToAsync(fileStream);
                    }

                    category.CategoryImageUrl = "/images/categories/" + uniqueFileName;
                }

                //Verificamos si el campo de la imagen viene vacio y de asignamos la misma imagen que tenía para que no de problemas
                if (categoryImageUrl == null)
                {
                    HttpResponseMessage response = await client.GetAsync($"Categories/Consultar?categoryId={category.CategoryId}");
                    var resultado = response.Content.ReadAsStringAsync().Result;
                    var categoryDtos = JsonConvert.DeserializeObject<Category>(resultado);
                    category.CategoryImageUrl = categoryDtos.CategoryImageUrl;
                }

                var result = await client.PutAsJsonAsync<Category>("Categories/Editar", category);

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

                    return View(category);
                }
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category = new Category();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage mensaje = await client.GetAsync($"Categories/Consultar?categoryId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (mensaje.IsSuccessStatusCode)
            {
                var result = mensaje.Content.ReadAsStringAsync().Result;

                //conversion json a obj
                category = JsonConvert.DeserializeObject<Category>(result);
            }

            return View(category);
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

            HttpResponseMessage response = await client.DeleteAsync($"Categories/Eliminar?categoryId={id}");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var category = new Category();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Categories/Consultar?categoryId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                category = JsonConvert.DeserializeObject<Category>(result);
            }

            return View(category);
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
