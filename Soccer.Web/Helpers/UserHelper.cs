using Microsoft.AspNetCore.Identity;
using Soccer.Web.Data.Entities;
using Soccer.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Soccer.Web.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<UserEntity> _signInManager; //para llamar a login y logout

        public UserHelper(UserManager<UserEntity> userManager, RoleManager<IdentityRole> roleManager, SignInManager<UserEntity> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public async Task<IdentityResult> AddUserAsync(UserEntity user, string password)
        {
            return await _userManager.CreateAsync(user, password); //crea usuario
        }

        public async Task AddUserToRoleAsync(UserEntity user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName); //adicional el usuario al role
        }

        public async Task CheckRoleAsync(string roleName)
        {
            bool exitsRole = await _roleManager.RoleExistsAsync(roleName); //evalua el rol
            if (!exitsRole) //si no existe
            {
                await _roleManager.CreateAsync(new IdentityRole //crea el role
                {
                    Name = roleName
                });
            }
        }

        public async Task<UserEntity> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> IsUserInRoleAsync(UserEntity user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync( //pasamos paràmetros
                model.UserName,
                model.Password,
                model.RememberMe,
                false); //si esta en true bloque de acuerdo a los intentos
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync(); //deslogueate
        }
    }
}
