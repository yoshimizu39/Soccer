﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Soccer.Web.Data.Entities
{
    public class TeamEntity
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "The file {0} cant not have more than {1} characters.")]
        [Required(ErrorMessage = "the file {0} is mandatory")]
        public string Name { get; set; }

        [Display(Name = "Logo")]
        public string LogoPath { get; set; }

        //public string LogoFullPath => string.IsNullOrEmpty(LogoPath)
        //    ? "https://SoccerWeb4.azurewebsites.net//images/noimage.png"
        //    : $"https://SoccerWeb4.azurewebsites.net{LogoPath.Substring(1)}";

        [Display(Name = "Logo")]
        public string LogoFullPath => string.IsNullOrEmpty(LogoPath)
        ? "https://SoccerWeb4.azurewebsites.net//images/noimage.png"
        : $"https://zulusoccer.blob.core.windows.net/teams/{LogoPath}"; // LogoPath graba el nombre del logo


        public ICollection<UserEntity> Users { get; set; }
    }
}
