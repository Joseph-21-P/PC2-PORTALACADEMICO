using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PortalAcademico.Models;

namespace PortalAcademico.ViewModels
{
    public class CatalogoIndexViewModel : IValidatableObject
    {
        [Display(Name = "Buscar por nombre")]
        public string? BuscarNombre { get; set; }

        [Display(Name = "Créditos Mínimos")]
        [Range(0, int.MaxValue, ErrorMessage = "Los créditos no pueden ser negativos.")]
        public int? MinCreditos { get; set; }

        [Display(Name = "Créditos Máximos")]
        [Range(0, int.MaxValue, ErrorMessage = "Los créditos no pueden ser negativos.")]
        public int? MaxCreditos { get; set; }

        [Display(Name = "Horario Inicio")]
        public TimeSpan? HorarioInicio { get; set; }

        [Display(Name = "Horario Fin")]
        public TimeSpan? HorarioFin { get; set; }


        public IEnumerable<Curso> Cursos { get; set; } = new List<Curso>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HorarioInicio.HasValue && HorarioFin.HasValue && HorarioInicio >= HorarioFin)
            {
                yield return new ValidationResult(
                    "El horario de fin no puede ser anterior o igual al horario de inicio.", 
                    new[] { nameof(HorarioFin) });
            }
        }
    }
}