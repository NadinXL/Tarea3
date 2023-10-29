using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using Tarea3.DTO;
using System.Text;
using System.Text.Json;


namespace Tarea3.Integrations
{
    public class JsonplaceholderAPIIntegration
    {
        private readonly ILogger<JsonplaceholderAPIIntegration> _logger;
        private const string API_URL = "https://jsonplaceholder.typicode.com/posts/";
        private readonly HttpClient httpClient;

        public JsonplaceholderAPIIntegration(ILogger<JsonplaceholderAPIIntegration> logger)
        {
            _logger = logger;
            httpClient = new HttpClient();
        }

        public async Task<List<PostDTO>> GetAllPosts()
        {
            string requestUrl = $"{API_URL}";
            List<PostDTO> listado = new List<PostDTO>();
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    listado = await response.Content.ReadFromJsonAsync<List<PostDTO>>() ?? new List<PostDTO>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Error al llamar a la API: {ex.Message}");
            }
            return listado;
        }

       public async Task<PostDTO> GetPostDetails(int id)
        {
            string requestUrl = $"{API_URL}/{id}"; // URL para obtener un post específico por su ID
            PostDTO post = null;
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    post = await response.Content.ReadFromJsonAsync<PostDTO>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Error al llamar a la API: {ex.Message}");
            }
            return post;
        }
        public async Task<PostDTO> CreatePost(PostDTO newPost)
        {
            string requestUrl = API_URL;
            PostDTO createdPost = null;
            try
            {
                // Realiza una solicitud POST para crear un nuevo post
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(requestUrl, newPost);
                if (response.IsSuccessStatusCode)
                {
                    createdPost = await response.Content.ReadFromJsonAsync<PostDTO>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Error al crear el post: {ex.Message}");
            }
            return createdPost;
        }

        public async Task<PostDTO> UpdatePost(PostDTO updatedPost)
        {
            string requestUrl = $"{API_URL}/{updatedPost.id}"; // URL para actualizar un post específico por su ID
            PostDTO updated = null;

            try
            {
                // Serializa el objeto updatedPost a JSON
                string updatedPostJson = JsonSerializer.Serialize(updatedPost);

                // Configura la solicitud PUT
                var content = new StringContent(updatedPostJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PutAsync(requestUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    updated = await response.Content.ReadFromJsonAsync<PostDTO>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Error al actualizar el post: {ex.Message}");
            }

            return updated;
        }
        public async Task<HttpResponseMessage> DeletePostAsync(int id)
{
    string requestUrl = $"{API_URL}/{id}"; // URL del recurso a eliminar

    try
    {
        HttpResponseMessage response = await httpClient.DeleteAsync(requestUrl);

        return response;
    }
    catch (Exception ex)
    {
        // Manejar errores o excepciones
        _logger.LogError($"Error al eliminar el post: {ex.Message}");
        throw; // Puedes relanzar la excepción o manejarla según tu necesidad
    }
}
    }
}