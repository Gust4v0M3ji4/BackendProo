using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using libreriawebpractica.Data; // Asegúrate del namespace correcto
using libreriawebpractica.Models; // Asegúrate del namespace correcto
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectList

namespace libreriawebpractica.Controllers
{
    public class AutoresController : Controller
    {
        private readonly AppDbContext _context; // Usamos el nombre correcto del DbContext

        public AutoresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Autores
        public async Task<IActionResult> Index(string searchString) // Añadido para búsqueda personalizada (Sesión 6)
        {
            // Consulta base (Sesión 5)
            var autores = _context.Autores.AsQueryable(); // AsQueryable() para poder aplicar Where

            if (!string.IsNullOrEmpty(searchString))
            {
                // Filtra por nombre del autor (Sesión 6)
                autores = autores.Where(a => a.Nombre.Contains(searchString));
            }

            return View(await autores.ToListAsync());
        }

        // GET: Autores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Sesión 5
            }

            var autor = await _context.Autores
                .FirstOrDefaultAsync(m => m.Id == id); // Sesión 5
            if (autor == null)
            {
                return NotFound(); // Sesión 5
            }

            return View(autor); // Sesión 5
        }

        // GET: Autores/Create
        public IActionResult Create()
        {
            // No necesitamos ViewData para autores en este caso simple (no tiene FKs)
            // Si tuviera, aquí se cargaría la lista para el dropdown
            return View();
        }

        // POST: Autores/Create
        // Para protegerse contra ataques de publicación excesiva, habilita las propiedades específicas a las que quieres enlazarte.
        // Para obtener más información, consulta https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] // Sesión 5
        [ValidateAntiForgeryToken] // Protección contra falsificación de solicitudes (Sesión 5)
        public async Task<IActionResult> Create([Bind("Nombre")] Autor autor) // Bind solo las propiedades necesarias (Sesión 5)
        {
            if (ModelState.IsValid) // Verificar si el modelo es válido (Sesión 5)
            {
                _context.Add(autor); // Agregar el autor al contexto (Sesión 5)
                await _context.SaveChangesAsync(); // Guardar cambios en la base de datos (Sesión 5)
                return RedirectToAction(nameof(Index)); // Redirigir a la lista de autores (Sesión 5)
            }
            return View(autor); // Si ModelState no es válido, vuelve a mostrar el formulario con errores (Sesión 5)
        }

        // GET: Autores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Sesión 5
            }

            var autor = await _context.Autores.FindAsync(id); // Sesión 5
            if (autor == null)
            {
                return NotFound(); // Sesión 5
            }
            return View(autor); // Sesión 5
        }

        // POST: Autores/Edit/5
        // Para protegerse contra ataques de publicación excesiva, habilita las propiedades específicas a las que quieres enlazarte.
        // Para obtener más información, consulta https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] // Sesión 5
        [ValidateAntiForgeryToken] // Protección contra falsificación de solicitudes (Sesión 5)
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] Autor autor) // Bind solo las propiedades necesarias (Sesión 5)
        {
            if (id != autor.Id) // Verificar que el ID del modelo coincida con el de la URL (Sesión 5)
            {
                return NotFound(); // Sesión 5
            }

            if (ModelState.IsValid) // Verificar si el modelo es válido (Sesión 5)
            {
                try
                {
                    _context.Update(autor); // Actualizar el autor en el contexto (Sesión 5)
                    await _context.SaveChangesAsync(); // Guardar cambios en la base de datos (Sesión 5)
                }
                catch (DbUpdateConcurrencyException) // Manejo de excepciones de concurrencia (Sesión 5)
                {
                    if (!AutorExists(autor.Id)) // Verificar si el autor aún existe (Sesión 5)
                    {
                        return NotFound(); // Sesión 5
                    }
                    else
                    {
                        throw; // Volver a lanzar la excepción si no es un problema de existencia (Sesión 5)
                    }
                }
                return RedirectToAction(nameof(Index)); // Redirigir a la lista de autores (Sesión 5)
            }
            return View(autor); // Si ModelState no es válido, vuelve a mostrar el formulario con errores (Sesión 5)
        }

        // GET: Autores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Sesión 5
            }

            var autor = await _context.Autores
                .FirstOrDefaultAsync(m => m.Id == id); // Sesión 5
            if (autor == null)
            {
                return NotFound(); // Sesión 5
            }

            return View(autor); // Sesión 5
        }

        // POST: Autores/Delete/5
        [HttpPost, ActionName("Delete")] // Usar ActionName para que la URL sea /Delete/5 pero el método sea DeleteConfirmed (Sesión 5)
        [ValidateAntiForgeryToken] // Protección contra falsificación de solicitudes (Sesión 5)
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var autor = await _context.Autores.FindAsync(id); // Buscar el autor por ID (Sesión 5)
            if (autor != null)
            {
                _context.Autores.Remove(autor); // Remover el autor del contexto (Sesión 5)
            }

            await _context.SaveChangesAsync(); // Guardar cambios en la base de datos (Sesión 5)
            return RedirectToAction(nameof(Index)); // Redirigir a la lista de autores (Sesión 5)
        }

        // Método privado para verificar si un autor existe (Sesión 5)
        private bool AutorExists(int id)
        {
            return _context.Autores.Any(e => e.Id == id);
        }
    }
}