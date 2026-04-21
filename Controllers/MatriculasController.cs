using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PortalAcademico.Controllers
{
    [Authorize] 
    public class MatriculasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MatriculasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Inscripcion(int? cursoId)
        {
            if (cursoId == null) return NotFound();

            var curso = await _context.Cursos.FirstOrDefaultAsync(c => c.Id == cursoId && c.Activo);
            if (curso == null) return NotFound();

            return View(curso);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InscripcionConfirmada(int cursoId)
        {
            var curso = await _context.Cursos.FirstOrDefaultAsync(c => c.Id == cursoId && c.Activo);
            if (curso == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var yaMatriculado = await _context.Matriculas
                .AnyAsync(m => m.CursoId == curso.Id && m.UsuarioId == user.Id && m.Estado != EstadoMatricula.Cancelada);
            
            if (yaMatriculado)
            {
                TempData["Error"] = "Ya te encuentras matriculado o con solicitud pendiente para este curso.";
                return View("Inscripcion", curso);
            }


            var inscritosActuales = await _context.Matriculas
                .CountAsync(m => m.CursoId == curso.Id && m.Estado != EstadoMatricula.Cancelada);
            
            if (inscritosActuales >= curso.CupoMaximo)
            {
                TempData["Error"] = "El curso ya ha alcanzado su cupo máximo.";
                return View("Inscripcion", curso);
            }

            var cursosDelUsuario = await _context.Matriculas
                .Include(m => m.Curso)
                .Where(m => m.UsuarioId == user.Id && m.Estado != EstadoMatricula.Cancelada)
                .Select(m => m.Curso)
                .ToListAsync();

            bool haySolapamiento = cursosDelUsuario.Any(c => 
                (curso.HorarioInicio < c.HorarioFin) && (curso.HorarioFin > c.HorarioInicio)
            );

            if (haySolapamiento)
            {
                TempData["Error"] = "El horario de este curso se cruza con otro curso en el que ya estás matriculado.";
                return View("Inscripcion", curso);
            }


            var nuevaMatricula = new Matricula
            {
                CursoId = curso.Id,
                UsuarioId = user.Id,
                Estado = EstadoMatricula.Pendiente
            };

            _context.Matriculas.Add(nuevaMatricula);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "¡Inscripción solicitada con éxito! Tu estado es Pendiente.";
            return RedirectToAction("Index", "Cursos"); 
        }
    }
}