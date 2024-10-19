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
    public class CollectionsController : Controller
    {
        private NetworkAPI networkAPI;
        private HttpClient client;
        private readonly DbContextNetwork _context;

        public CollectionsController(DbContextNetwork context)
        {
            networkAPI = new NetworkAPI();
            client = networkAPI.Initial();
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Collection> listCollections = new List<Collection>();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync("Collections/Listado");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                listCollections = JsonConvert.DeserializeObject<List<Collection>>(result);
            }

            return View(listCollections);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Protege las rutas si es necesario
        public async Task<IActionResult> Create([Bind] CollectionImage collection, IFormFile collectionImageUrl)
        {
            if (ModelState.IsValid)
            {
                using (var content = new MultipartFormDataContent())
                {
                    // Añade los campos de texto
                    content.Add(new StringContent(collection.WorkId.ToString()), "WorkId");

                    // Añade el archivo si no es nulo
                    if (collectionImageUrl != null)
                    {
                        var fileStreamContent = new StreamContent(collectionImageUrl.OpenReadStream());
                        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(collectionImageUrl.ContentType); // Ajusta el tipo de contenido según sea necesario
                        content.Add(fileStreamContent, "CollectionImageUrl", collectionImageUrl.FileName);
                    }

                    var response = await client.PostAsync("Collections/Agregar", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Collections");
                    }
                    else
                    {
                        TempData["Mensaje"] = "No se logró registrar la colección";
                        return View(collection);
                    }
                }
            }
            return View(collection);
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var collection = new Collection();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Collections/ConsultarId?collectionId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                collection = JsonConvert.DeserializeObject<Collection>(result);
            }

            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] Collection collection, IFormFile collectionImageUrl)
        {

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            if (ModelState.IsValid)
            {
                if (collectionImageUrl != null && collectionImageUrl.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/collections");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + collectionImageUrl.FileName;

                    uniqueFileName = uniqueFileName.Replace(" ", "_");

                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await collectionImageUrl.CopyToAsync(fileStream);
                    }

                    collection.CollectionImageUrl = "/images/collections/" + uniqueFileName;
                }

                if (collectionImageUrl == null)
                {
                    HttpResponseMessage response = await client.GetAsync($"Collections/ConsultarId?collectionId={collection.CollectionId}");
                    var resultado = response.Content.ReadAsStringAsync().Result;
                    var collectionDtos = JsonConvert.DeserializeObject<Collection>(resultado);
                    collection.CollectionImageUrl = collectionDtos.CollectionImageUrl;
                }

                var result = await client.PutAsJsonAsync<Collection>("Collections/Editar", collection);

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

                    return View(collection);
                }
            }
            return View(collection);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var collection = new Collection();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage mensaje = await client.GetAsync($"Collections/ConsultarId?collectionId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (mensaje.IsSuccessStatusCode)
            {
                var result = mensaje.Content.ReadAsStringAsync().Result;

                //conversion json a obj
                collection = JsonConvert.DeserializeObject<Collection>(result);
            }

            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.DeleteAsync($"Collections/Eliminar?id={id}");
           
            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var collection = new Collection();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Collections/ConsultarId?collectionId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                collection = JsonConvert.DeserializeObject<Collection>(result);
            }

            return View(collection);
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
