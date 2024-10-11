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
        public async Task<IActionResult> Create(WorkProfile workProfile, IFormFile profilePictureUrl)
        {
            if (ModelState.IsValid)
            {
                if (profilePictureUrl != null && profilePictureUrl.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/workProfile");

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

                    workProfile.ProfilePictureUrl = "/images/workProfile/" + uniqueFileName;
                }

                workProfile.WorkId = 0;

                if (profilePictureUrl == null)
                {
                    workProfile.ProfilePictureUrl = "ND";
                }

                //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

                var add = client.PostAsJsonAsync<WorkProfile>("WorkProfile/CrearCuenta", workProfile);
                await add;

                var result = add.Result;

                // if (ValidateSession(response.StatusCode) == false)
                // {
                //     return RedirectToAction("Logout", "Users");
                // }

                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "WorkProfiles");
                }
                else
                {
                    TempData["Mensaje"] = "No se logró registrar el usuario";

                    return View(workProfile);
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
            }
            return View(workProfile);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] WorkProfile workProfile, IFormFile profilePictureUrl)
        {
            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            if (ModelState.IsValid)
            {
                if (profilePictureUrl != null && profilePictureUrl.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/workProfile");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + profilePictureUrl.FileName.Replace(" ", "_");
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePictureUrl.CopyToAsync(fileStream);
                    }

                    workProfile.ProfilePictureUrl = "/images/workProfile/" + uniqueFileName;
                }
                else
                {
                    HttpResponseMessage response = await client.GetAsync($"WorkProfile/BuscarID?id={workProfile.WorkId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var resultado = await response.Content.ReadAsStringAsync();
                        var profileDtos = JsonConvert.DeserializeObject<WorkProfile>(resultado);
                        workProfile.ProfilePictureUrl = profileDtos?.ProfilePictureUrl ?? workProfile.ProfilePictureUrl;
                    }
                }

                var result = await client.PutAsJsonAsync<WorkProfile>("WorkProfile/Editar", workProfile);

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
