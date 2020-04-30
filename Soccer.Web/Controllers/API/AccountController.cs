using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Soccer.Common.Enums;
using Soccer.Common.Models;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using System.Globalization;
using Soccer.Web.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Pages.Internal.Account;

namespace Soccer.Web.Controllers.API
{
    [Route("api/[Controller]")]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IImageHelper _imageHelper;

        public AccountController(DataContext context,
                                 IUserHelper userHelper,
                                 IMailHelper mailHelper,
                                 IImageHelper imageHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _imageHelper = imageHelper;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request",
                    Result = ModelState
                });
            }

            CultureInfo cultureInfo = new CultureInfo(request.CultureInfo);
            Resource.Culture = cultureInfo;

            UserEntity user = await _userHelper.GetUserAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = Resource.UserDoesntExists
                });
            }

            IdentityResult result = await _userHelper.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                string message = result.Errors.FirstOrDefault().Description;

                //si Contains tiene la palabra "password" entonces mensaje incorrecto, 
                //caso contrario nos devuelve un mensaje del sistema(BadRequest)
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = message.Contains("password") ? Resource.IncorrectCurrentPassword : message
                });
            }

            return Ok(new Response
            {
                IsSuccess = true,
                Message = Resource.PasswordChangedSuccess
            });

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        public async Task<IActionResult> PutUser([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CultureInfo ci = new CultureInfo(request.CultureInfo);
            Resource.Culture = ci;

            UserEntity userentity = await _userHelper.GetUserAsync(request.Email);
            if (userentity == null)
            {
                return BadRequest(Resource.UserDoesntExists);
            }

            //subimos la foto desde el mòvil
            string picturepath = userentity.PicturePath;
            if (request.PictureArray != null && request.PictureArray.Length > 0)
            {
                picturepath = _imageHelper.UploadImage(request.PictureArray, "Users");
            }

            //actualizamos los campos
            userentity.FirstName = request.FirstName;
            userentity.LastName = request.LastName;
            userentity.Address = request.Address;
            userentity.PhoneNumber = request.Phone;
            userentity.Document = request.Phone;
            userentity.Team = await _context.Teams.FindAsync(request.TeamId);
            userentity.PicturePath = picturepath;

            //devolvemos el user
            IdentityResult result = await _userHelper.UpdateUserAsync(userentity);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault().Description);
            }

            return NoContent(); //no devuleve nada, pero dice si pudo realizarlo o nó
        }

        [HttpPost]
        [Route("RecoverPassword")]
        public async Task<IActionResult> RecoverPassword([FromBody] EmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request."
                });
            }

            CultureInfo ci = new CultureInfo(request.CultureInfo);
            Resource.Culture = ci;

            UserEntity user = await _userHelper.GetUserAsync(request.Email); //buscamos el email del user
            if (user == null) //si no existe
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = Resource.UserDoesntExists //email no existe
                });
            }

            string mytoken = await _userHelper.GeneratePasswordResetTokenAsync(user);
            string link = Url.Action("ResetPassword",
                                     "Account",
                                     new
                                     {
                                         token = mytoken
                                     }, protocol: HttpContext.Request.Scheme);

            _mailHelper.SendMail(request.Email,
                                 Resource.RecoverPassword,
                                 $"<h1>{Resource.RecoverPassword}</h1>"+
                                 $"{Resource.RecoverPasswordSubject}:</br></br>"+
                                 $"<a href=\"{link}\">{Resource.RecoverPassword}</a>");

            return Ok(new Response
            {
                IsSuccess = true,
                Message = Resource.RecoverPasswordMessage
            });
        }

        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request.",
                    Result = ModelState
                });
            }

            CultureInfo ci = new CultureInfo(request.CultureInfo);
            Resource.Culture = ci;

            UserEntity user = await _userHelper.GetUserAsync(request.Email);
            if (user != null)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = Resource.UserAlreadyExists
                });
            }

            string picturepath = string.Empty;
            if (request.PictureArray != null && request.PictureArray.Length > 0)
            {
                //subimos la foto al folder user
                picturepath = _imageHelper.UploadImage(request.PictureArray, "Users");
            }

            user = new UserEntity
            {
                Address = request.Address,
                Document = request.Document,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.Phone,
                UserName = request.Email,
                PicturePath = picturepath,
                UserType = UserType.User,
                Team = await _context.Teams.FindAsync(request.TeamId) //busca el id del equipo
            };

            //cramos una acciòn para el user
            IdentityResult result = await _userHelper.AddUserAsync(user, request.Password);
            if (result != IdentityResult.Success) //si no lo crea
            {
                //devuelve error
                return BadRequest(result.Errors.FirstOrDefault().Description);
            }

            UserEntity newuser = await _userHelper.GetUserAsync(request.Email); //obtenemos el user
            await _userHelper.AddUserToRoleAsync(newuser, user.UserType.ToString());//creamos el rol

            string mytoken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            string tokenlink = Url.Action("ConfirmEmail",
                                          "Account",
                                          new
                                          {
                                              userid = user.Id,
                                              token = mytoken
                                          }, protocol: HttpContext.Request.Scheme);

            _mailHelper.SendMail(request.Email,
                                 Resource.ConfirmEmail,
                                 $"<h1>{Resource.ConfirmEmail}</h1>"+
                                 $"{Resource.ConfirmEmailSubject}</br></br><a href=\"{tokenlink}\">{Resource.ConfirmEmail}</a>");

            return Ok(new Response
            {
                IsSuccess = true,
                Message = Resource.ConfirmEmailMessage
            });
        }
    }
}
