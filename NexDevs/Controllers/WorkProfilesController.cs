using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NexDevs.Context;
using NexDevs.Models;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using Humanizer;

namespace NexDevs.Controllers
{
    [Authorize]// protect routes
    public class WorkProfilesController : Controller
    {
        private NetworkAPI networkAPI;
        private HttpClient client;

        public WorkProfilesController()
        {
            networkAPI = new NetworkAPI();
            client = networkAPI.Initial();
        }


        public async Task<IActionResult> Index(string search)
        {
            List<WorkProfile> listWorkProfiles = new List<WorkProfile>();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync("WorkProfile/Listado");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                listWorkProfiles = JsonConvert.DeserializeObject<List<WorkProfile>>(result);
            }

            if (!string.IsNullOrEmpty(search))
            {
                listWorkProfiles = listWorkProfiles
                                    .Where(workProfile => workProfile.Name.Contains(search))
                                    .ToList();
            }

            return View(listWorkProfiles);

        }

        public async Task<IActionResult> Details(int? id)
        {
            var workProfile = new WorkProfile();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"WorkProfile/BuscarID?id={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                workProfile = JsonConvert.DeserializeObject<WorkProfile>(result);
            }

            return View(workProfile);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Protege las rutas si es necesario
        public async Task<IActionResult> Create([Bind] WorkProfileImage workProfile, IFormFile profilePictureUrl)
        {
            if (ModelState.IsValid)
            {

                using (var content = new MultipartFormDataContent())
                {
                    // Añade los campos de texto
                    content.Add(new StringContent(workProfile.Name), "Name");
                    content.Add(new StringContent(workProfile.Email), "Email");
                    content.Add(new StringContent(workProfile.Number), "Number");
                    content.Add(new StringContent(workProfile.Password), "Password");
                    content.Add(new StringContent(workProfile.Province), "Province");
                    content.Add(new StringContent(workProfile.City), "City");
                    content.Add(new StringContent(workProfile.WorkDescription), "WorkDescription");
                    content.Add(new StringContent(workProfile.ProfileType.ToString()), "ProfileType");

                    // Añade el archivo si no es nulo
                    if (profilePictureUrl != null)
                    {
                        var fileStreamContent = new StreamContent(profilePictureUrl.OpenReadStream());
                        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // Ajusta el tipo de contenido según sea necesario
                        content.Add(fileStreamContent, "ProfilePictureUrl", profilePictureUrl.FileName);
                    }

                    var response = await client.PostAsync("WorkProfile/CrearCuenta", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "WorkProfiles");
                    }
                    else
                    {
                        TempData["Mensaje"] = "No se logró registrar el WorkProfile";
                        return View(workProfile);
                    }
                }
            }
            return View(workProfile);
        }




        public async Task<IActionResult> Edit(int id)
        {
            var workProfile = new WorkProfile();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"WorkProfile/BuscarID?id={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                workProfile = JsonConvert.DeserializeObject<WorkProfile>(result);

                var workProfileImage = new WorkProfileImage
                {
                    WorkId = workProfile.WorkId,
                    Name = workProfile.Name,
                    Email = workProfile.Email,
                    Number = workProfile.Number,
                    Password = workProfile.Password,
                    Province = workProfile.Province,
                    City = workProfile.City,
                    WorkDescription = workProfile.WorkDescription,
                    ImageUrl = workProfile.ProfilePictureUrl,
                    ProfileType = workProfile.ProfileType,
                    Salt = workProfile.Salt
                };
                return View(workProfileImage);
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] WorkProfileImage workProfile, IFormFile profilePictureUrl)
        {
            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            if (ModelState.IsValid)
            {
                using (var content = new MultipartFormDataContent())
                {
                    // Añade los campos de texto
                    content.Add(new StringContent(workProfile.WorkId.ToString()), "WorkId");
                    content.Add(new StringContent(workProfile.Name), "Name");
                    content.Add(new StringContent(workProfile.Email), "Email");
                    content.Add(new StringContent(workProfile.Number), "Number");
                    content.Add(new StringContent(workProfile.Password), "Password");
                    content.Add(new StringContent(workProfile.Province), "Province");
                    content.Add(new StringContent(workProfile.City), "City");
                    content.Add(new StringContent(workProfile.WorkDescription), "WorkDescription");
                    content.Add(new StringContent(workProfile.ProfileType.ToString()), "ProfileType");

                    // Añade el archivo si no es nulo
                    if (profilePictureUrl != null)
                    {
                        var fileStreamContent = new StreamContent(profilePictureUrl.OpenReadStream());
                        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // Ajusta el tipo de contenido según sea necesario
                        content.Add(fileStreamContent, "ProfilePictureUrl", profilePictureUrl.FileName);
                    }
                    var result = await client.PutAsync("WorkProfile/Editar", content);

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

                        return View(workProfile);
                    }
                }

            }

            return View(workProfile);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var workProfile = new WorkProfile();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage profile = await client.GetAsync($"WorkProfile/BuscarID?id={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (profile.IsSuccessStatusCode)
            {
                var result = profile.Content.ReadAsStringAsync().Result;

                workProfile = JsonConvert.DeserializeObject<WorkProfile>(result);
            }

            return View(workProfile);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            var workProfile = new WorkProfile();

            HttpResponseMessage profile = await client.GetAsync($"WorkProfile/BuscarID?id={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (profile.IsSuccessStatusCode)
            {
                var result = profile.Content.ReadAsStringAsync().Result;

                workProfile = JsonConvert.DeserializeObject<WorkProfile>(result);

                if (workProfile != null)
                {
                    HttpResponseMessage response = await client.DeleteAsync($"WorkProfile/Eliminar?email={workProfile.Email}");

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Mensaje"] = "Error deleting the profile.";
                    }
                }
            }
            else
            {
                TempData["Mensaje"] = "Profile not found.";
            }

            return RedirectToAction("Index");
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
