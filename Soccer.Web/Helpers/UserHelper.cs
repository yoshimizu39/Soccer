using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Soccer.Common.Enums;
using Soccer.Web.Data;
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
        private readonly DataContext _context;

        public UserHelper(UserManager<UserEntity> userManager, 
                          RoleManager<IdentityRole> roleManager,
                          SignInManager<UserEntity> signInManager,
                          DataContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
        }
        public async Task<IdentityResult> AddUserAsync(UserEntity user, string password)
        {
            return await _userManager.CreateAsync(user, password); //crea usuario
        }

        public async Task<UserEntity> AddUserAsync(AddUserViewModel model, string path, UserType userType)
        {
            UserEntity user = new UserEntity
            {
                Address = model.Address,
                Document = model.Document,
                Email = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PicturePath = path,
                PhoneNumber = model.PhoneNumber,
                Team = await _context.Teams.FindAsync(model.TeamId), //buscamos el equipo favorito
                UserName = model.Username,
                UserType = userType
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (result != IdentityResult.Success)
            {
                return null;
            }

            UserEntity newUser = await GetUserAsync(model.Username);
            await AddUserToRoleAsync(newUser, user.UserType.ToString()); //asignamos el role useer

            return newUser;
        }

        public async Task AddUserToRoleAsync(UserEntity user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName); //adicional el usuario al role
        }

        public async Task<IdentityResult> ChangePasswordAsync(UserEntity user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
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

        public async Task<IdentityResult> ConfirmEmailAsync(UserEntity userEntity, string token)
        {
            return await _userManager.ConfirmEmailAsync(userEntity, token);
        }

        //string se almacena todo el token se envìa al user
        public async Task<string> GenerateEmailConfirmationTokenAsync(UserEntity userEntity)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(userEntity);
        }

        //public async Task<UserEntity> GetUserByEmailAsync(string email)
        //{
        //    return await _userManager.FindByEmailAsync(email);
        //}

        public async Task<UserEntity> GetUserAsync(string email)
        {
            return await _context.Users.Include(u => u.Team)
                                       .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserEntity> GetUserAsync(Guid userId)
        {
            return await _context.Users.Include(u => u.Team)
                                       .FirstOrDefaultAsync(u => u.Id == userId.ToString());
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
                false); //si esta en true bloquea de acuerdo a los intentos
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync(); //deslogueate
        }

        public async Task<IdentityResult> UpdateUserAsync(UserEntity user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<SignInResult> ValidatePasswordAsync(UserEntity userEntity, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(userEntity, password, false); //false no permite que se bloquee el logueo de varios intentos
        }
    }
}
