using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soccer.Web.Helpers;
using Soccer.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Controllers
{

    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;

        public AccountController(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home"); //redireccionamos al index del home
            }

            return View(); //si nò se redirecciona a la vista
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _userHelper.LoginAsync(model); //devuelve un identity result
                if (result.Succeeded) //si se logueo
                {
                    if (Request.Query.Keys.Contains("ReturnUrl")) //verifica si tiene direcciòn de retorno
                    {
                        return Redirect(Request.Query["ReturnUrl"].First()); //si es sì, redirecciona
                    }

                    return RedirectToAction("Index", "Home"); //si no logue redirecciona
                }

                ModelState.AddModelError(string.Empty, "Email o Password incorrectos"); //si no se puede loguear
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home"); //redirecciona
        }
    }
}
