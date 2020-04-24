using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soccer.Common.Enums;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using Soccer.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Controllers
{

    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IImageHelper _imageHelper;
        private readonly DataContext _context;

        public AccountController(IUserHelper userHelper,
                                 ICombosHelper combosHelper,
                                 IImageHelper imageHelper,
                                 DataContext context)
        {
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _imageHelper = imageHelper;
            _context = context;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home"); //redireccionamos al index del home
            }

            return View(); //si nò se redirecciona a la vista
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserAsync(User.Identity.Name); //bucamos el user
                var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("ChangeUser");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description); //manda los errores hasta solucionarlo
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ChangeUser()
        {
            UserEntity userEntity = await _userHelper.GetUserAsync(User.Identity.Name); //buscame al user logueado

            EditUserViewModel editUserViewModel = new EditUserViewModel
            {
                Address = userEntity.Address,
                Document = userEntity.Document,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                PhoneNumber = userEntity.PhoneNumber,
                PicturePath = userEntity.PicturePath,
                Teams = _combosHelper.GetComboTeams(),
                TeamId = userEntity.Team.Id
            };

            return View(editUserViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {   
                string path = model.PicturePath;
                if (model.PictureFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.PictureFile, "Users");
                }

                UserEntity user = await _userHelper.GetUserAsync(User.Identity.Name); //traer el user

                //para actualizar los datos
                user.Document = model.Document;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                user.PicturePath = path;
                user.Team = await _context.Teams.FindAsync(model.TeamId); //se busca con teamid para actualizarlo

                await _userHelper.UpdateUserAsync(user);

                return RedirectToAction("Index", "Home");
            }

            model.Teams = _combosHelper.GetComboTeams(); //para que no se pierda los datos cuando usamos post
            return View(model);
        }

        public IActionResult Register()
        {
            AddUserViewModel model = new AddUserViewModel
            {
                Teams = _combosHelper.GetComboTeams()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = string.Empty;
                if (model.PictureFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.PictureFile, "Users"); //subimos la imàgen
                }

                UserEntity user = await _userHelper.AddUserAsync(model, path, UserType.User);
                if (user == null) //si devuelve nulo es porque ya existe
                {
                    ModelState.AddModelError(string.Empty, "This email is already used");
                    model.Teams = _combosHelper.GetComboTeams();

                    return View(model);
                }

                //logueamos por còdigo
                LoginViewModel loginViewModel = new LoginViewModel
                {
                    Password = model.Password,
                    RememberMe = false,
                    UserName = model.Username
                };

                var result2 = await _userHelper.LoginAsync(loginViewModel);
                if (result2.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            //y si falla mandamos a la vista con el combo
            model.Teams = _combosHelper.GetComboTeams();
            return View(model);
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
