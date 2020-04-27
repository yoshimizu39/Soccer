using Microsoft.AspNetCore.Identity;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Soccer.Common.Enums;
using Soccer.Web.Data.Entities;
using Soccer.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Helpers
{
    public interface IUserHelper
    {
        Task<string> GenerateEmailConfirmationTokenAsync(UserEntity userEntity); //se genera token y se envìa al correo del nuevo user
        Task<IdentityResult> ConfirmEmailAsync(UserEntity userEntity, string token); //mètodo que se ejecuta cuando el user nuevo confirma el mesage enviado a su cuenta
        Task<SignInResult> ValidatePasswordAsync(UserEntity userEntity, string password);
        Task<IdentityResult> ChangePasswordAsync(UserEntity user, string oldPassword, string newPassword);
        Task<IdentityResult> UpdateUserAsync(UserEntity user);
        Task<UserEntity> AddUserAsync(AddUserViewModel model, string path, UserType userType);
        Task<UserEntity> GetUserAsync(string email);
        Task<UserEntity> GetUserAsync(Guid userId);
        Task<IdentityResult> AddUserAsync(UserEntity user, string password);
        Task CheckRoleAsync(string roleName);
        Task AddUserToRoleAsync(UserEntity user, string roleName);
        Task<bool> IsUserInRoleAsync(UserEntity user, string roleName);
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
    }
}
