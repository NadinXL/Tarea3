using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tarea3.DTO;
using Tarea3.Integrations;
using System.Text;
using System.Text.Json;

namespace Tarea3.Controllers.UI
{
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly JsonplaceholderAPIIntegration _jsonplaceholder;

        public PostController(ILogger<PostController> logger, JsonplaceholderAPIIntegration jsonplaceholder)
        {
            _logger = logger;
            _jsonplaceholder = jsonplaceholder;
        }
public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(PostDTO newPost)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Serializa el objeto newPost a JSON
                    string postJson = JsonSerializer.Serialize(newPost);

                    // Configura la solicitud POST
                    var content = new StringContent(postJson, Encoding.UTF8, "application/json");
                    var createdPost = await _jsonplaceholder.CreatePost(newPost); // Debes usar el método CreatePost del Integration

                    if (createdPost != null)
                    {
                        // Si la creación fue exitosa, redirige al listado de posts
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "No se pudo crear el post.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Error en la creación del post: " + ex.Message);
                }
            }

            // Si hay errores de validación, muestra el formulario de creación nuevamente
            return View(newPost);
        }
        public async Task<IActionResult> Index()
        {

            List<PostDTO> posts = await _jsonplaceholder.GetAllPosts();




            return View(posts);
        }

         public async Task<IActionResult> Details(int id)
        {
            // Lógica para obtener los detalles del post con el ID especificado
            PostDTO post = await _jsonplaceholder.GetPostDetails(id);

            if (post == null)
            {
                return NotFound(); // Manejar el caso en el que no se encuentre el post con el ID proporcionado
            }

            return View(post);
        }

        public async Task<IActionResult> Edit(int id)
        {
            // Aquí debes obtener el post existente con el ID proporcionado
            var existingPost = await _jsonplaceholder.GetPostDetails(id);

            if (existingPost == null)
            {
                // Si no se encuentra el post, puedes mostrar un error o redirigir a una página de error.
                return NotFound();
            }

            return View(existingPost);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, PostDTO updatedPost)
        {
            if (id != updatedPost.id)
            {
                // El ID proporcionado en el cuerpo del formulario no coincide con el ID en la URL.
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var updated = await _jsonplaceholder.UpdatePost(updatedPost);

                    if (updated != null)
                    {
                        // La actualización fue exitosa, redirige al listado de posts o a la vista de detalles del post.
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "No se pudo actualizar el post.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Error en la actualización del post: " + ex.Message);
                }
            }

            // Si hay errores de validación o la actualización falla, muestra el formulario de edición nuevamente.
            return View(updatedPost);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
           // Realiza la eliminación llamando a la API de la siguiente manera
    var response = await _jsonplaceholder.DeletePostAsync(id);

    if (response.IsSuccessStatusCode)
    {
        // Si la eliminación fue exitosa, redirige a la lista de posts
        return RedirectToAction("Index");
    }
    else
    {
        // Si hubo un error, maneja la respuesta o muestra un mensaje de error
        var errorMessage = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError(string.Empty, "Error al eliminar el post: " + errorMessage);

        // Redirige de nuevo a la vista de detalles del post (o a donde consideres) en caso de error
        return RedirectToAction("Details", new { id = id });
    }
        }
    }
}