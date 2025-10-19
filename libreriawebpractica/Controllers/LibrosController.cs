using System.IO;
using libreriawebpractica.Data;
using libreriawebpractica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace libreriawebpractica.Controllers
{
    public class LibrosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public LibrosController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var librosQuery = _context.Libros.Include(l => l.Autor);

            if (!string.IsNullOrEmpty(searchString))
            {
                librosQuery = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Libro, Autor?>)librosQuery
                    .Where(l => l.Titulo.Contains(searchString) || l.Autor.Nombre.Contains(searchString));
            }

            var libros = await librosQuery.ToListAsync();
            return View(libros);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var libro = await _context.Libros
                .Include(l => l.Autor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (libro == null)
                return NotFound();

            return View(libro);
        }

        public IActionResult Create()
        {
            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titulo,AutorId,ImagenArchivo")] Libro libro)
        {
            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Nombre", libro.AutorId);

            if (ModelState.IsValid)
            {
                try
                {
                    Console.WriteLine("Antes de guardar imagen");
                    if (libro.ImagenArchivo != null && libro.ImagenArchivo.Length > 0)
                    {
                        Console.WriteLine("Intentando guardar imagen...");
                        var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(libro.ImagenArchivo.FileName);
                        var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagenes", nombreArchivo);

                        using (var stream = new FileStream(ruta, FileMode.Create))
                        {
                            await libro.ImagenArchivo.CopyToAsync(stream);
                        }

                        libro.ImagenRuta = "/imagenes/" + nombreArchivo;
                        Console.WriteLine("Imagen guardada en: " + ruta);
                    }
                    Console.WriteLine("Después de guardar imagen");

                    _context.Add(libro);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR AL GUARDAR IMAGEN: " + ex.Message);
                    ModelState.AddModelError("", "Error al guardar la imagen: " + ex.Message);
                }
            }
            return View(libro);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var libro = await _context.Libros.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
            if (libro == null)
                return NotFound();

            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Nombre", libro.AutorId);
            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,AutorId,ImagenRuta,ImagenArchivo")] Libro libro)
        {
            if (id != libro.Id)
                return NotFound();

            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Nombre", libro.AutorId);

            if (ModelState.IsValid)
            {
                try
                {
                    var libroExistente = await _context.Libros.FindAsync(id);
                    if (libroExistente == null)
                        return NotFound();

                    libroExistente.Titulo = libro.Titulo;
                    libroExistente.AutorId = libro.AutorId;

                    // Subida de imagen (opcional)
                    if (libro.ImagenArchivo != null && libro.ImagenArchivo.Length > 0)
                    {
                        var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(libro.ImagenArchivo.FileName);
                        var ruta = Path.Combine(_environment.WebRootPath, "imagenes", nombreArchivo);

                        using (var stream = new FileStream(ruta, FileMode.Create))
                        {
                            await libro.ImagenArchivo.CopyToAsync(stream);
                        }

                        // Elimina la imagen anterior si existe
                        if (!string.IsNullOrEmpty(libroExistente.ImagenRuta))
                        {
                            var rutaAnterior = Path.Combine(_environment.WebRootPath, libroExistente.ImagenRuta.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                            if (System.IO.File.Exists(rutaAnterior))
                                System.IO.File.Delete(rutaAnterior);
                        }

                        libroExistente.ImagenRuta = "/imagenes/" + nombreArchivo;
                    }

                    _context.Update(libroExistente);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibroExists(libro.Id))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(libro);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var libro = await _context.Libros
                .Include(l => l.Autor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (libro == null)
                return NotFound();

            return View(libro);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro != null)
            {
                if (!string.IsNullOrEmpty(libro.ImagenRuta))
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, libro.ImagenRuta.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);
                }

                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LibroExists(int id)
        {
            return _context.Libros.Any(e => e.Id == id);
        }
    }
}