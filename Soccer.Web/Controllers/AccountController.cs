using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Soccer.Common.Enums;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using Soccer.Web.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Soccer.Web.Controllers
{

    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IImageHelper _imageHelper;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration; //se usa cuando necesitamos leer datos de appsettings.json
        private readonly IMailHelper _mail;

        public AccountController(IUserHelper userHelper,
                                 ICombosHelper combosHelper,
                                 IImageHelper imageHelper,
                                 DataContext context,
                                 IConfiguration configuration,
                                 IMailHelper mail)
        {
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _imageHelper = imageHelper;
            _context = context;
            _configuration = configuration;
            _mail = mail;
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

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserEntity user = await _userHelper.GetUserAsync(model.Email); //busca el user
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email doesn't correspont to a registered user.");
                    return View(model);
                }

                //el user existe y generamos token
                string mytoken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                //generamos el link
                string link = Url.Action("ResetPassword",
                                         "Account",
                                         new { token = mytoken },
                                         protocol: HttpContext.Request.Scheme);

                _mail.SendMail(model.Email,
                               "Soccer Password Reset",
                               $"<h1>Soccer Password Reset" +
                               $"To reset the password click in this link: </br></br>" +
                               $"<a href=\"{link}\">Reset Password</a>");

                ViewBag.Message = "The instructions to recover your password has been sent to email.";
                return View();
            }

            return View(model);
        }

        //cuando el user haga click en reset password envìa el token y muestra el form vacìo
        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        //llena los datos del nuevo password
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            UserEntity user = await _userHelper.GetUserAsync(model.UserName);
            if (user != null)
            {
                IdentityResult result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Password reset successful.";
                    return View();
                }

                ViewBag.Message = "Error while resetting the password.";
                return View(model);
            }

            ViewBag.Message = "User not found.";
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userid, string token)
        {
            if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            UserEntity user = await _userHelper.GetUserAsync(new Guid(userid));
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserAsync(model.UserName); //busca el user
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(user, model.Password); //valida que el password es el correcto del user
                    if (result.Succeeded)
                    {
                        //agregamos clains al token
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email), //agregamos el email
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) //y un GUID para poder combinar
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"])); //encriptamos el key
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); //con un algoritmo
                        var token = new JwtSecurityToken(_configuration["Tokens:Issuer"], //buscamos los valores de Issuer
                                                         _configuration["Tokens:Audience"], //y de Audience
                                                         claims,
                                                         expires: DateTime.UtcNow.AddDays(99), //asigna expiraciòn al token
                                                         signingCredentials: credentials);

                        //devolvemos el valor del token y de la expiraciòn
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return Created(string.Empty, results); //devolvemos el token
                    }
                }
            }

            return BadRequest(); //si no funciona devuelve error 400
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
                //LoginViewModel loginViewModel = new LoginViewModel
                //{
                //    Password = model.Password,
                //    RememberMe = false,
                //    UserName = model.Username
                //};

                //var result2 = await _userHelper.LoginAsync(loginViewModel);
                //if (result2.Succeeded)
                //{
                //    return RedirectToAction("Index", "Home");
                //}
                var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user); //dame token para el user

                //creamos link para enviar al nuevo user
                //ConfirEmail es una acciòn
                var tokenlink = Url.Action("ConfirmEmail", "Account", new
                {
                    //enviamos el
                    userid = user.Id, //còdigo del user
                    token = myToken //y token
                }, protocol: HttpContext.Request.Scheme);

                var response = _mail.SendMail(model.Username,
                                              "Email Confirmation",
                                              $"<h1>Email Confirmation</h1>" +
                                              $"To allow the user. " +
                                              $"please click in this link:</br></br><a href=\"{tokenlink}\">Confirm Email</a>"); //tokenlink enviamos el link

                if (response.IsSuccess)
                {
                    ViewBag.Message = "The instructions to allow your user has been sent to email.";
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, response.Message);
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
