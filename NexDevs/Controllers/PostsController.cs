using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NexDevs.Models;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Net.Http.Headers;

namespace NexDevs.Controllers
{
    [Authorize]// protect routes
    public class PostsController : Controller
    {
        private NetworkAPI networkAPI;
        private HttpClient client;

        public PostsController()
        {
            networkAPI = new NetworkAPI();
            client = networkAPI.Initial();
        }

        public async Task<IActionResult> Index()
        {
            List<Post> posts = new List<Post>();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync("Posts/ListadoGeneral");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");

            // }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                posts = JsonConvert.DeserializeObject<List<Post>>(result);
            }

            return View(posts);
        }

        public async Task<IActionResult> Approved()
        {
            List<Post> posts = new List<Post>();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync("Posts/Aprobados");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");

            // }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                posts = JsonConvert.DeserializeObject<List<Post>>(result);
            }

            return View(posts);
        }

        public async Task<IActionResult> ToBeApprove()
        {
            List<Post> posts = new List<Post>();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync("Posts/PorAprobar");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");

            // }

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                posts = JsonConvert.DeserializeObject<List<Post>>(result);
            }

            return View(posts);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var post = new Post();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Posts/Consultar?postId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                post = JsonConvert.DeserializeObject<Post>(result);
            }

            return View(post);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] Post post, IFormFile postImageUrl)
        {
            if (ModelState.IsValid)
            {
                // Asignar valores predeterminados al post
                post.PostId = 0; // Este valor puede ser asignado por la base de datos
                post.Approved = 0;
                post.CommentsCount = 0;
                post.LikesCount = 0;
                post.CreateAt = DateTime.UtcNow;

                using (var content = new MultipartFormDataContent())
                {
                    // Agregar el objeto Post al contenido
                    content.Add(new StringContent(post.WorkId.ToString()), "WorkId");
                    content.Add(new StringContent(post.ContentPost ?? ""), "ContentPost");

                    // Validar si hay imagen y agregarla al contenido
                    if (postImageUrl != null && postImageUrl.Length > 0)
                    {
                        var imageContent = new StreamContent(postImageUrl.OpenReadStream());
                        imageContent.Headers.ContentType = new MediaTypeHeaderValue(postImageUrl.ContentType);
                        content.Add(imageContent, "photo", postImageUrl.FileName); // Nombre debe coincidir con el que espera la API
                    }

                    // Hacer la solicitud POST al endpoint Agregar
                    var response = await client.PostAsync("Posts/Agregar", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Posts");
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        TempData["Mensaje"] = $"Error: {responseContent}"; // Capturar errores
                        return View(post);
                    }
                }
            }
            return View(post);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var post = new Post();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Posts/Consultar?postId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                post = JsonConvert.DeserializeObject<Post>(result);
            }
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] Post post, IFormFile postImageUrl)
        {
            if (ModelState.IsValid)
            {
                //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

                if (postImageUrl != null && postImageUrl.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/posts");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + postImageUrl.FileName.Replace(" ", "_");
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await postImageUrl.CopyToAsync(fileStream);
                    }

                    post.PostImageUrl = "/images/posts/" + uniqueFileName;
                }
                else
                {
                    HttpResponseMessage response = await client.GetAsync($"Posts/Consultar?postId={post.PostId}");

                    // if (ValidateSession(response.StatusCode) == false)
                    // {
                    //     return RedirectToAction("Logout", "Users");
                    // }

                    if (response.IsSuccessStatusCode)
                    {
                        var resultado = await response.Content.ReadAsStringAsync();
                        var postsDtos = JsonConvert.DeserializeObject<Post>(resultado);
                        post.PostImageUrl = postsDtos?.PostImageUrl ?? postsDtos.PostImageUrl;
                    }
                }

                var result = await client.PutAsJsonAsync<Post>("Posts/Editar", post);

                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Mensaje"] = "Datos incorrectos";

                    return View(post);
                }

            }

            return View(post);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var post = new Post();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Posts/Consultar?postId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                post = JsonConvert.DeserializeObject<Post>(result);
            }

            return View(post);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.DeleteAsync($"Posts/Eliminar?postId={id}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

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
