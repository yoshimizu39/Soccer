using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress] //valida que UserName tenga formato de correo
        public string UserName { get; set; }

        [Required]
        [MinLength(6)] //Valida que tenga 6 dìgitos, como fuè declarado en el startup
        public string Password { get; set; }

        public bool RememberMe { get; set; } //guarda contraseña y password
    }
}
