using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PortalAcademico.Models
{
    public class Matricula
    {
        public int Id { get; set; }

        [Required]
        public int CursoId { get; set; }
        public Curso Curso { get; set; } = null!;

        [Required]
        public string UsuarioId { get; set; } = string.Empty;
        public IdentityUser Usuario { get; set; } = null!;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public EstadoMatricula Estado { get; set; } = EstadoMatricula.Pendiente;
    }
}