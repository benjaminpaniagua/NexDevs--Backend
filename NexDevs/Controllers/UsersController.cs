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

    public class UsersController : Controller
    {

        private NetworkAPI networkAPI;
        private HttpClient client;
        private readonly DbContextNetwork _context;
        public UsersController(DbContextNetwork context)
        {
            networkAPI = new NetworkAPI();
            client = networkAPI.Initial();
            _context = context;
        }

        [Authorize]// protect routes
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
        [Authorize]// protect routes
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Protege las rutas si es necesario
        public async Task<IActionResult> Create([Bind] UserImage user, IFormFile profilePictureUrl)
        {
            if (ModelState.IsValid)
            {
                user.Salt = "salt";

                using (var content = new MultipartFormDataContent())
                {
                    // Añade los campos de texto
                    content.Add(new StringContent(user.FirstName), "FirstName");
                    content.Add(new StringContent(user.LastName), "LastName");
                    content.Add(new StringContent(user.Email), "Email");
                    content.Add(new StringContent(user.Password), "Password");
                    content.Add(new StringContent(user.Province), "Province");
                    content.Add(new StringContent(user.City), "City");
                    content.Add(new StringContent(user.Bio), "Bio");
                    content.Add(new StringContent(user.ProfileType.ToString()), "ProfileType");
                    content.Add(new StringContent(user.Salt), "Salt");

                    // Añade el archivo si no es nulo
                    if (profilePictureUrl != null)
                    {
                        var fileStreamContent = new StreamContent(profilePictureUrl.OpenReadStream());
                        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // Ajusta el tipo de contenido según sea necesario
                        content.Add(fileStreamContent, "ProfilePictureUrl", profilePictureUrl.FileName);
                    }

                    var response = await client.PostAsync("Users/CrearCuenta", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Users");
                    }
                    else
                    {
                        TempData["Mensaje"] = "No se logró registrar el usuario";
                        return View(user);
                    }
                }
            }
            return View(user);
        }

        [HttpGet]
        [Authorize]// protect routes
        public async Task<IActionResult> Edit(string email)
        {
            var user = new User();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Users/BuscarEmail?email={email}");

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
        [Authorize]// protect routes
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
                    HttpResponseMessage response = await client.GetAsync($"Users/BuscarEmail?email={user.Email}");
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
        [Authorize]// protect routes
        public async Task<IActionResult> Delete(string email)
        {
            var user = new User();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage mensaje = await client.GetAsync($"Users/BuscarEmail?email={email}");

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
        [Authorize]// protect routes
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string email)
        {
            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.DeleteAsync($"Users/Eliminar?email={email}");

            // if (ValidateSession(response.StatusCode) == false)
            // {
            //     return RedirectToAction("Logout", "Users");
            // }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]// protect routes
        public async Task<IActionResult> Details(string email)
        {
            var user = new User();

            //client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await client.GetAsync($"Users/BuscarEmail?email={email}");

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


        // ***  INICIO DE SESIÓN  ***
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind] User temp)
        {
            if (!ModelState.IsValid) // Verifica si el modelo es válido
            {
                return View(temp); // Devuelve la vista con errores
            }

            User user = ValidateUser(temp); // Valida el usuario

            if (user != null)
            {
                var userClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, temp.Email), // Puedes agregar más claims si es necesario
            new Claim(ClaimTypes.NameIdentifier, temp.UserId.ToString()) // Agregar ID de usuario
        };

                var granIdentity = new ClaimsIdentity(userClaims, "User Identity");
                var userPrincipal = new ClaimsPrincipal(granIdentity);

                await HttpContext.SignInAsync(userPrincipal); // Inicia sesión
                return RedirectToAction("Index", "Home"); // se redirige a la vista principal
            }
            else
            {
                //message in case of invalid user
                TempData["Message"] = "Invalid User";
                return View(temp); //return the view with the user data
            }
        }

        private User ValidateUser(User temp)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email.Equals(temp.Email));

            User userAuth = null; //var to store the user to be authenticated

            if (user != null)
            {
                //password validation
                if (user.Password.Equals(temp.Password))
                {
                    userAuth = user; //store the user to be authenticated
                }
            }
            return userAuth;
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(); //close the session
            return RedirectToAction("Index", "Home"); //redirect to the main view
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount([Bind] User temp, string confirm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Verify all Claims are correct"; // Mostrar mensaje de error

                if (temp.FirstName == null)
                {
                    ModelState.AddModelError("FirstName", "FirstName is required");
                }
                if (temp.LastName == null)
                {
                    ModelState.AddModelError("LastName", "LastName is required");
                }
                if (temp.Email == null)
                {
                    ModelState.AddModelError("Email", "Email is required");
                }
                if (temp.Password == null)
                {
                    ModelState.AddModelError("Password", "Password is required");
                }
                if (temp.Province == null)
                {
                    ModelState.AddModelError("Province", "Province is required");
                }
                if (temp.City == null)
                {
                    ModelState.AddModelError("City", "City is required");
                }
                if (temp.Bio == null)
                {
                    ModelState.AddModelError("Bio", "Bio is required");
                }
                if (temp.ProfilePictureUrl == null)
                {
                    ModelState.AddModelError("ProfilePictureUrl", "ProfilePictureUrl is required");
                }
                if (temp.ProfileType == null)
                {
                    ModelState.AddModelError("ProfileType", "ProfileType is required");
                }

                return View(); // Devolver la vista si el modelo no es válido
            }

            if (temp.Password.Equals(confirm)) // Validar que la confirmación de la contraseña coincida con la contraseña
            {
                temp.Email = temp.Email.Replace(" ", ""); // Eliminar los espacios en blanco del correo electrónico
                _context.Users.Add(temp); // Agregar el usuario a la base de datos
                await _context.SaveChangesAsync(); // Guardar los cambios en la base de datos
                return RedirectToAction("Login"); // Redirigir a la vista de inicio de sesión
            }
            else
            {
                TempData["Message"] = "Password and Confirm Password must be the same"; // Mostrar mensaje de error
                ModelState.AddModelError("Password", "Password and Confirm Password must be the same"); // Agregar error al modelo
                return View(); // Devolver la vista si la confirmación de la contraseña falla
            }
        }
    }
}
