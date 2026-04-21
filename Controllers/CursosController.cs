using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace PortalAcademico.Controllers
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CursosController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(CatalogoIndexViewModel model)
        {

            var query = _context.Cursos.Where(c => c.Activo).AsQueryable();


            if (!ModelState.IsValid)
            {
                model.Cursos = await query.ToListAsync();
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.BuscarNombre))
            {
                query = query.Where(c => c.Nombre.Contains(model.BuscarNombre));
            }
            if (model.MinCreditos.HasValue)
            {
                query = query.Where(c => c.Creditos >= model.MinCreditos);
            }
            if (model.MaxCreditos.HasValue)
            {
                query = query.Where(c => c.Creditos <= model.MaxCreditos);
            }
            if (model.HorarioInicio.HasValue)
            {
                query = query.Where(c => c.HorarioInicio >= model.HorarioInicio);
            }
            if (model.HorarioFin.HasValue)
            {
                query = query.Where(c => c.HorarioFin <= model.HorarioFin);
            }


            model.Cursos = await query.ToListAsync();
            return View(model);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var curso = await _context.Cursos.FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (curso == null) return NotFound();

            return View(curso);
        }
    }
}