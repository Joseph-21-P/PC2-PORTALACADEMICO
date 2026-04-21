using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalAcademico.Models
{
    public class Curso : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Los créditos deben ser mayores a 0.")]
        public int Creditos { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El cupo máximo debe ser mayor a 0.")]
        public int CupoMaximo { get; set; }

        [Required]
        public TimeSpan HorarioInicio { get; set; }

        [Required]
        public TimeSpan HorarioFin { get; set; }

        public bool Activo { get; set; } = true;

        // Propiedad de navegación
        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

        // Validación a nivel de modelo para el horario
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HorarioInicio >= HorarioFin)
            {
                yield return new ValidationResult(
                    "El horario de fin debe ser posterior al horario de inicio.",
                    new[] { nameof(HorarioInicio), nameof(HorarioFin) }
                );
            }
        }
    }
}