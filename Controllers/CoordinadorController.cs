using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PortalAcademico.Data;
using PortalAcademico.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PortalAcademico.Controllers
{
    [Authorize(Roles = "Coordinador")] 
    public class CoordinadorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public CoordinadorController(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }


        public async Task<IActionResult> Index()
        {
            var cursos = await _context.Cursos.ToListAsync();
            return View(cursos);
        }


        public IActionResult Create() => View();


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Curso curso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(curso);
                await _context.SaveChangesAsync();
                await _cache.RemoveAsync("cursos_activos"); 
                TempData["Mensaje"] = "Curso creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();
            return View(curso);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Curso curso)
        {
            if (id != curso.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(curso);
                await _context.SaveChangesAsync();
                await _cache.RemoveAsync("cursos_activos"); 
                TempData["Mensaje"] = "Curso actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso != null)
            {
                curso.Activo = !curso.Activo;
                _context.Update(curso);
                await _context.SaveChangesAsync();
                await _cache.RemoveAsync("cursos_activos"); 
                TempData["Mensaje"] = $"El curso ha sido {(curso.Activo ? "activado" : "desactivado")}.";
            }
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Matriculas(int cursoId)
        {
            var curso = await _context.Cursos
                .Include(c => c.Matriculas)
                .ThenInclude(m => m.Usuario)
                .FirstOrDefaultAsync(c => c.Id == cursoId);

            if (curso == null) return NotFound();

            return View(curso);
        }


        [HttpPost]
        public async Task<IActionResult> CambiarEstadoMatricula(int matriculaId, EstadoMatricula nuevoEstado, int cursoId)
        {
            var matricula = await _context.Matriculas.FindAsync(matriculaId);
            if (matricula != null)
            {
                matricula.Estado = nuevoEstado;
                _context.Update(matricula);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = $"Matrícula actualizada a {nuevoEstado}.";
            }
            return RedirectToAction(nameof(Matriculas), new { cursoId = cursoId });
        }
    }
}