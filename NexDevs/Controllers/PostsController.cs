using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NexDevs.Models;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.Extensions.Hosting;

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

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync("Posts/ListadoGeneral");

            if (ValidateSession(response.StatusCode) == false)
            {
                return RedirectToAction("Logout", "Users");
            }

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

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync("Posts/Aprobados");

            if (ValidateSession(response.StatusCode) == false)
            {
                return RedirectToAction("Logout", "Users");
            }

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

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync("Posts/PorAprobar");

            if (ValidateSession(response.StatusCode) == false)
            {
                return RedirectToAction("Logout", "Users");
            }

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

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Posts/Consultar?postId={id}");

            if (ValidateSession(response.StatusCode) == false)
            {
                return RedirectToAction("Logout", "Users");
            }

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
        [Authorize] // Protege las rutas si es necesario
        public async Task<IActionResult> Create([Bind] PostImage post, IFormFile postImageUrl)
        {
            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

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
                    content.Add(new StringContent(post.WorkId.ToString()), "WorkId");
                    content.Add(new StringContent(post.ContentPost ?? ""), "ContentPost");
                    content.Add(new StringContent(post.PaymentReceipt.ToString()), "PaymentReceipt");

                    // Añade el archivo si no es nulo
                    if (postImageUrl != null)
                    {
                        var fileStreamContent = new StreamContent(postImageUrl.OpenReadStream());
                        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(postImageUrl.ContentType); // Ajusta el tipo de contenido según sea necesario
                        content.Add(fileStreamContent, "PostImageUrl", postImageUrl.FileName);
                    }

                    var response = await client.PostAsync("Posts/Agregar", content);

                    if (ValidateSession(response.StatusCode) == false)
                    {
                        return RedirectToAction("Logout", "Users");
                    }


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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = new Post();

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Posts/Consultar?postId={id}");

            if (ValidateSession(response.StatusCode) == false)
            {
                return RedirectToAction("Logout", "Users");
            }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                post = JsonConvert.DeserializeObject<Post>(result);

                var postImage = new PostImage
                {
                    PostId = post.PostId,
                    WorkId = post.WorkId,
                    ContentPost = post.ContentPost,
                    PaymentReceipt = post.PaymentReceipt,
                    ImageUrl = post.PostImageUrl,
                    CreateAt = post.CreateAt,
                    LikesCount = post.LikesCount,
                    CommentsCount = post.CommentsCount,
                    Approved = post.Approved
                };
                return View(postImage);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] PostImage post, IFormFile postImageUrl)
        {
            if (ModelState.IsValid)
            {
                client.DefaultRequestHeaders.Authorization = AutorizacionToken();

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(post.PostId.ToString()), "PostId");
                    content.Add(new StringContent(post.WorkId.ToString()), "WorkId");
                    content.Add(new StringContent(post.ContentPost), "ContentPost");
                    content.Add(new StringContent(post.PaymentReceipt.ToString()), "PaymentReceipt");
                    content.Add(new StringContent(post.CreateAt.ToString()), "CreateAt");
                    content.Add(new StringContent(post.LikesCount.ToString()), "LikesCount");
                    content.Add(new StringContent(post.CommentsCount.ToString()), "CommentsCount");
                    content.Add(new StringContent(post.Approved.ToString()), "Approved");

                    // Añade el archivo si no es nulo
                    if (postImageUrl != null)
                    {
                        var fileStreamContent = new StreamContent(postImageUrl.OpenReadStream());
                        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                        content.Add(fileStreamContent, "PostImageUrl", postImageUrl.FileName);
                    }

                    var result = await client.PutAsync("Posts/Editar", content);

                    if (ValidateSession(result.StatusCode) == false)
                    {
                        return RedirectToAction("Logout", "Users");
                    }

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
            }

            return View(post);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var post = new Post();

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Posts/Consultar?postId={id}");

            if (ValidateSession(response.StatusCode) == false)
            {
                return RedirectToAction("Logout", "Users");
            }

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
            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.DeleteAsync($"Posts/Eliminar?postId={id}");

            if (ValidateSession(response.StatusCode) == false)
            {
                return RedirectToAction("Logout", "Users");
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
