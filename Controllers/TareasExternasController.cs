using Microsoft.AspNetCore.Mvc;
using TareasApi.Models;
using System.Text.Json;

namespace TareasApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasExternasController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ExternalApiUrl = "https://jsonplaceholder.typicode.com/todos";

        public TareasExternasController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /api/tareas-externas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TareaExternalDto>>> GetTareasExternas()
        {
            var client = _httpClientFactory.CreateClient();
            
            try 
            {
                var response = await client.GetAsync(ExternalApiUrl);
                
                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Error al conectar con la API externa.");

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var rawTasks = JsonSerializer.Deserialize<List<JsonElement>>(content, options);

                // Mapeo al DTO propio requerido
                var mappedTasks = rawTasks?.Select(t => new TareaExternalDto
                {
                    ExternalId = t.GetProperty("id").GetInt32(),
                    Titulo = t.GetProperty("title").GetString() ?? "",
                    Completado = t.GetProperty("completed").GetBoolean()
                }).ToList();

                return Ok(mappedTasks);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error controlado: La API externa no responde.");
            }
        }

        // GET: /api/tareas-externas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TareaExternalDto>> GetTareaExterna(int id)
        {
            var client = _httpClientFactory.CreateClient();
            
            try 
            {
                var response = await client.GetAsync($"{ExternalApiUrl}/{id}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound($"La tarea externa con ID {id} no existe.");

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Error en la API externa.");

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var t = JsonSerializer.Deserialize<JsonElement>(content, options);

                return Ok(new TareaExternalDto
                {
                    ExternalId = t.GetProperty("id").GetInt32(),
                    Titulo = t.GetProperty("title").GetString() ?? "",
                    Completado = t.GetProperty("completed").GetBoolean()
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Error controlado: No se pudo procesar la solicitud externa.");
            }
        }
    }
}