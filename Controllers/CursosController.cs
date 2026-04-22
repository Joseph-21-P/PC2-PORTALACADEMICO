using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PortalAcademico.Data;
using PortalAcademico.Models;
using PortalAcademico.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; 
using System;

namespace PortalAcademico.Controllers
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;


        public CursosController(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }


        public async Task<IActionResult> Index(CatalogoIndexViewModel model)
        {
            const string cacheKey = "cursos_activos";
            List<Curso> cursosBase;


            var cachedCursos = await _cache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(cachedCursos))
            {

                cursosBase = await _context.Cursos.Where(c => c.Activo).ToListAsync();
                

                var cacheOptions = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(cursosBase), cacheOptions);
            }
            else
            {

                cursosBase = JsonSerializer.Deserialize<List<Curso>>(cachedCursos) ?? new List<Curso>();
            }


            var query = cursosBase.AsEnumerable();

            if (!ModelState.IsValid)
            {
                model.Cursos = query;
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.BuscarNombre))
            {
                query = query.Where(c => c.Nombre.Contains(model.BuscarNombre, StringComparison.OrdinalIgnoreCase));
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

            model.Cursos = query.ToList();
            return View(model);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var curso = await _context.Cursos.FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (curso == null) return NotFound();

            // Guardar el último curso visitado en Sesión
            HttpContext.Session.SetString("UltimoCursoId", curso.Id.ToString());
            HttpContext.Session.SetString("UltimoCursoNombre", curso.Nombre);

            return View(curso);
        }
    }
}