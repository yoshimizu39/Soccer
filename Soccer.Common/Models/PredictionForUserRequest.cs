using System;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Common.Models
{
    public class PredictionForUserRequest
    {
        //pide las predicciones
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public Guid UserId { get; set; }

        //predicciones de un torneo específico
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public int TournamentId { get; set; }

        [Required]
        public string CultureInfo { get; set; }

    }
}
