using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareasApi.Data;
using TareasApi.Models;

namespace TareasApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TareasController(AppDbContext context)
        {
            _context = context;
        }

        // PREGUNTA 2: GET /api/tareas con filtros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarea>>> GetTareas(
            [FromQuery] EstadoTarea? estado, 
            [FromQuery] PrioridadTarea? prioridad,
            [FromQuery] DateTime? fechaInicio, 
            [FromQuery] DateTime? fechaFin)
        {
            var query = _context.Tareas.AsQueryable();

            // Filtro por Estado
            if (estado.HasValue)
            {
                query = query.Where(t => t.Estado == estado.Value);
            }

            // Filtro por Prioridad
            if (prioridad.HasValue)
            {
                query = query.Where(t => t.Prioridad == prioridad.Value);
            }

            // Filtro por Rango de fechas y Validación (Error 400)
            if (fechaInicio.HasValue && fechaFin.HasValue)
            {
                if (fechaInicio > fechaFin)
                {
                    return BadRequest("La fecha de inicio no puede ser mayor que la fecha de fin.");
                }
                query = query.Where(t => t.FechaVencimiento >= fechaInicio.Value && 
                                         t.FechaVencimiento <= fechaFin.Value);
            }

            return await query.ToListAsync();
        }

        // GET: api/tareas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tarea>> GetTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null) return NotFound();
            return tarea;
        }

        // POST: api/tareas
        [HttpPost]
        public async Task<ActionResult<Tarea>> PostTarea(Tarea tarea)
        {
            if (tarea.FechaVencimiento.Date < DateTime.Now.Date)
            {
                return BadRequest("La fecha de vencimiento no puede ser menor a la fecha actual.");
            }

            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTarea), new { id = tarea.Id }, tarea);
        }

        // PUT: api/tareas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTarea(int id, Tarea tarea)
        {
            if (id != tarea.Id) return BadRequest();

            if (tarea.FechaVencimiento.Date < DateTime.Now.Date)
            {
                return BadRequest("La fecha de vencimiento no puede ser menor a la fecha actual.");
            }

            _context.Entry(tarea).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tareas.Any(e => e.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/tareas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null) return NotFound();

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}