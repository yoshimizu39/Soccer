using Microsoft.AspNetCore.Identity;
using Soccer.Common.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Soccer.Web.Data.Entities
{
    public class UserEntity : IdentityUser
    {
        [Display(Name = "Document")]
        [MaxLength(20, ErrorMessage = "El atributo {0} no puede tener mas de {1} caracteres")]
        [Required(ErrorMessage = "El atributo {0} es requerido")]
        public string Document { get; set; }

        [Display(Name = "Fsirst Name")]
        [MaxLength(50, ErrorMessage = "El atributo {0} no puede tener mas de {1} caracteres")]
        [Required(ErrorMessage = "El atributo {0} es requerido")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [MaxLength(50, ErrorMessage = "El atributo {0} no puede tener mas de {1} caracteres")]
        [Required(ErrorMessage = "El atributo {0} es requerido")]
        public string LastName { get; set; }

        [MaxLength(500, ErrorMessage = "El atributo {0} no puede tener mas de {1} caracteres")]
        public string Address { get; set; }

        [Display(Name = "Picture")]
        public string PicturePath { get; set; }

        [Display(Name = "Picture")]
        public UserType UserType { get; set; }

        [Display(Name = "Favorite Team")]
        public TeamEntity Team { get; set; }

        [Display(Name = "User")]
        public string FullName => $"{FirstName}{LastName}";

        [Display(Name = "User")]
        public string FullNameWithDocument => $"{FirstName}{LastName}-{Document}";

        public int Points => Predictions == null ? 0 : Predictions.Sum(p => p.Points);

        public ICollection<PredictionEntity> Predictions { get; set; }
    }
}
