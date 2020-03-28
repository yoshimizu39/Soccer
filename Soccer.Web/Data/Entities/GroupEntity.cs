using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Data.Entities
{
    public class GroupEntity
    {
        public int Id { get; set; }

        [MaxLength(30, ErrorMessage = "El Atributo {0} debe tener como mìnimo {1} caracteres.")]
        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        public string Name { get; set; }

        public TournamentEntity Tournament { get; set; }

        public ICollection<GroupDetailEntity> GroupDetails { get; set; }

        public ICollection<MatchEntity> Matches { get; set; }
    }
}
