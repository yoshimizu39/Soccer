using Microsoft.AspNetCore.Mvc.Rendering;
using Soccer.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Models
{
    public class MatchViewModel : MatchEntity
    {
        public int GroupId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Local")]
        [Range(1, int.MaxValue, ErrorMessage = "Necesitas sellecionar un equipo")]
        public int LocalId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Visitor")]
        [Range(1, int.MaxValue, ErrorMessage = "Necesitas sellecionar un equipo")]
        public int VisitorlId { get; set; }

        public IEnumerable<SelectListItem> Teams { get; set; }
    }
}
