using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NexDevs.Context;
using NexDevs.Models;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace NexDevs.Controllers
{
    [Authorize]// protect routes
    public class CategoriesController : Controller
    {
        private NetworkAPI networkAPI;
        private HttpClient client;
        private readonly DbContextNetwork _context;

        public CategoriesController(DbContextNetwork context)
        {
            networkAPI = new NetworkAPI();
            client = networkAPI.Initial();
            _context = context;
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
        [Authorize] // Protege las rutas si es necesario
        public async Task<IActionResult> Create([Bind] CategoryImage category, IFormFile categoryImageUrl)
        {
            if (ModelState.IsValid)
            {

                using (var content = new MultipartFormDataContent())
                {
                    // Añade los campos de texto
                    content.Add(new StringContent(category.CategoryName), "CategoryName");

                    // Añade el archivo si no es nulo
                    if (categoryImageUrl != null)
                    {
                        var fileStreamContent = new StreamContent(categoryImageUrl.OpenReadStream());
                        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // Ajusta el tipo de contenido según sea necesario
                        content.Add(fileStreamContent, "CategoryImageUrl", categoryImageUrl.FileName);
                    }

                    var response = await client.PostAsync("Categories/Agregar", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Categories");
                    }
                    else
                    {
                        TempData["Mensaje"] = "No se logró registrar la categoria";
                        return View(category);
                    }
                }
            }
            return View(category);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = new Category();

            HttpResponseMessage response = await client.GetAsync($"Categories/Consultar?categoryId={id}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                category = JsonConvert.DeserializeObject<Category>(result);
            }

            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit([Bind] Category category, IFormFile categoryImageUrl)
        {
            if (ModelState.IsValid)
            {
                using (var content = new MultipartFormDataContent())
                {
                    // Añadir los campos de texto
                    content.Add(new StringContent(category.CategoryId.ToString()), "CategoryId");
                    content.Add(new StringContent(category.CategoryName), "CategoryName");

                    // Si se proporciona una imagen
                    if (categoryImageUrl != null)
                    {
                        var fileStreamContent = new StreamContent(categoryImageUrl.OpenReadStream());
                        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(categoryImageUrl.ContentType); // Tipo de archivo dinámico
                        content.Add(fileStreamContent, "CategoryImageUrl", categoryImageUrl.FileName);
                    }

                    // Llamada a la API para editar la categoría
                    HttpResponseMessage response = await client.PutAsync("Categories/Editar", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Categories");
                    }
                    else
                    {
                        TempData["Mensaje"] = "Error al editar la categoría";
                        return View(category);
                    }
                }
            }

            return View(category);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit([Bind] Category category, IFormFile categoryImageUrl)
        //{

        //    //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

        //    if (ModelState.IsValid)
        //    {
        //        if (categoryImageUrl != null && categoryImageUrl.Length > 0)
        //        {
        //            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/categories");

        //            if (!Directory.Exists(uploadsFolder))
        //            {
        //                Directory.CreateDirectory(uploadsFolder);
        //            }

        //            var uniqueFileName = Guid.NewGuid().ToString() + "_" + categoryImageUrl.FileName;

        //            uniqueFileName = uniqueFileName.Replace(" ", "_");

        //            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //            using (var fileStream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await categoryImageUrl.CopyToAsync(fileStream);
        //            }

        //            category.CategoryImageUrl = "/images/categories/" + uniqueFileName;
        //        }

        //        //Verificamos si el campo de la imagen viene vacio y de asignamos la misma imagen que tenía para que no de problemas
        //        if (categoryImageUrl == null)
        //        {
        //            HttpResponseMessage response = await client.GetAsync($"Categories/Consultar?categoryId={category.CategoryId}");
        //            var resultado = response.Content.ReadAsStringAsync().Result;
        //            var categoryDtos = JsonConvert.DeserializeObject<Category>(resultado);
        //            category.CategoryImageUrl = categoryDtos.CategoryImageUrl;
        //        }

        //        var result = await client.PutAsJsonAsync<Category>("Categories/Editar", category);

        //        // if (ValidateSession(response.StatusCode) == false)
        //        // {
        //        //     return RedirectToAction("Logout", "Users");
        //        // }

        //        if (result.IsSuccessStatusCode)
        //        {
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            TempData["Mensaje"] = "Datos incorrectos";

        //            return View(category);
        //        }
        //    }
        //    return View(category);
        //}

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

            HttpResponseMessage response = await client.DeleteAsync($"Categories/Eliminar?categoryId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

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
