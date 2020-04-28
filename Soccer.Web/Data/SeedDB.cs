using Soccer.Common.Enums;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Data
{
    public class SeedDB
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDB(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckTeamsAsync();
            await CheckTournamentsAsync();
            await CheckRolesAsync();
            await CheckUserAsync("1010", "Yoshi", "Shimizu", "yoshimizu39@outlook.es", "955221095", "Urb. Lima D-12", UserType.Admin); //crea usuario
            //await CheckUserAsync("1020", "Alanis", "Shimizu", "alanis@gmail.com", "988888888", "Urb. Lima D-12", UserType.User);
            //await CheckUserAsync("1030", "Karina", "Torres", "karina@hotmail.com", "977777777", "Urb. Lima D-12", UserType.User);
            await CheckUsersAsync();
            await CkeckPredictionsAsync();
        }

        private async Task CheckUsersAsync()
        {
            for (int i = 1; i <= 100; i++)
            {
                await CheckUserAsync($"100{i}", "User", $"{i}", $"user{i}@yopmail.com", "350 634 2747", "Calle Luna del Sol", UserType.User);
            }
        }

        private async Task CkeckPredictionsAsync()
        {
            if (!_context.Predictions.Any()) //si no hay predicciones
            {
                foreach (UserEntity user in _context.Users) //para cada user
                {
                    if (user.UserType == UserType.User) //si el user son iguales
                    {
                        AddPredictions(user); //realiza predicciones
                    }
                }

                await _context.SaveChangesAsync();
            }
        }

        private void AddPredictions(UserEntity user)
        {
            Random randon = new Random(); //aleatorio

            foreach (MatchEntity match in _context.Matches) //por cada partido en la colecciòn de partidos
            {
                _context.Predictions.Add(new PredictionEntity
                {
                    GoalsLocal = randon.Next(0, 5), //goles aleatorios entre 0-5
                    GoalsVisitor = randon.Next(0, 5), //goles aleatorios entre 0-5
                    Match = match,
                    User = user
                });
            }
        }

        private async Task<UserEntity> CheckUserAsync(string document, string firstName, string lastName, string email, string phone,
                                                       string address, UserType userType)
        {
            UserEntity user = await _userHelper.GetUserAsync(email); //busca email

            if (user == null)
            {
                user = new UserEntity //crea el user
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    Team = _context.Teams.FirstOrDefault(), //coge el primer equipo
                    UserType = userType
                };

                await _userHelper.AddUserAsync(user, "123456"); //crea el usuario
                await _userHelper.AddUserToRoleAsync(user, userType.ToString()); //añade el role indicado

                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task CheckTournamentsAsync()
        {
            if (!_context.Tournaments.Any())
            {
                var startDate = DateTime.Today.AddMonths(2).ToUniversalTime();
                var endDate = DateTime.Today.AddMonths(3).ToUniversalTime();

                _context.Tournaments.Add(new TournamentEntity
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    IsActive = true,
                    LogoPath = $"~/images/Tournaments/Copa America 2020.png",
                    Name = "Copa America 2020",
                    Groups = new List<GroupEntity>
                    {
                        new GroupEntity
                        {
                            Name = "A",
                            GroupDetails = new List<GroupDetailEntity>
                            {
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Colombia")},
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Ecuador")},
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Panama")},
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Canada")},
                            },
                            Matches = new List<MatchEntity>
                            {
                                new MatchEntity
                                {
                                    Date = startDate.AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Colombia"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Ecuador")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Panama"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Canada")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Colombia"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Panama")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Ecuador"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Canada")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Canada"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Colombia")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Ecuador"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Panama")
                                }
                            }
                        },
                        new GroupEntity
                        {
                            Name = "B",
                            GroupDetails = new List<GroupDetailEntity>
                            {
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Argentina")},
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Paraguay")},
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Mexico")},
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Chile")},
                            },
                            Matches = new List<MatchEntity>
                            {
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(1).AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Argentina"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Paraguay")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(1).AddHours(17),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Mexico"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Chile")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(5).AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Argentina"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Mexico")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(5).AddHours(17),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Paraguay"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Chile")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(10).AddHours(16),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Chile"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Argentina")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(10).AddHours(16),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Paraguay"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Mexico")
                                },
                                
                            }
                        },
                        new GroupEntity
                        {
                            Name = "C",
                            GroupDetails = new List<GroupDetailEntity>
                            {
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Brasil")},
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Venezuela")},
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "USA")},
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Peru")},
                            },
                            Matches = new List<MatchEntity>
                            {
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(2).AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Brasil"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Venezuela")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(2).AddHours(17),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "USA"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Peru")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(6).AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Brasil"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "USA")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(6).AddHours(17),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Venezuela"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Peru")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(11).AddHours(16),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Peru"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Brasil")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(11).AddHours(16),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Venezuela"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "USA")
                                }
                            }
                        },
                        new GroupEntity
                        {
                            Name = "D",
                            GroupDetails = new List<GroupDetailEntity>
                            {
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Uruguay")},
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Bolivia")},
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Costa Rica")},
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Honduras")},
                            },
                            Matches = new List<MatchEntity>
                            {
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(3).AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Uruguay"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Bolivia")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(3).AddHours(17),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Costa Rica"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Honduras")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(7).AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Uruguay"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Costa Rica")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(7).AddHours(17),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Bolivia"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Honduras")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(12).AddHours(16),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Honduras"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Uruguay")
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(12).AddHours(16),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Bolivia"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Costa Rica")
                                }
                            }
                        }
                    }
                });

                startDate = DateTime.Today.AddMonths(1).ToUniversalTime();
                endDate = DateTime.Today.AddMonths(4).ToUniversalTime();

                _context.Tournaments.Add(new TournamentEntity
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    IsActive = true,
                    LogoPath = $"~/images/Tournaments/Liga Aguila 2020-I.png",
                    Name = "Liga Aguila 2020-I",
                    Groups = new List<GroupEntity>
                    {
                        new GroupEntity
                        {
                            Name = "A",
                            GroupDetails = new List<GroupDetailEntity>
                            {
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "America")},
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Bucaramanga")},
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Junior")},
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Medellin")},
                            },
                            Matches = new List<MatchEntity>
                            {
                                new MatchEntity
                                {
                                    Date = startDate.AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "America"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Bucaramanga"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(4).AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "America"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Junior"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(4).AddHours(17),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Bucaramanga"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Medellin"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(9).AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Medellin"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "America"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(9).AddHours(17),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Bucaramanga"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Junior"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(15).AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Bucaramanga"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "America"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(15).AddHours(17),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Medellin"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Junior"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(19).AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Junior"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "America"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(19).AddHours(17),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Medellin"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Bucaramanga"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(19).AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "America"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Medellin"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(19).AddHours(17),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Junior"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Bucaramanga"),
                                }
                            }
                        },
                        new GroupEntity
                        {
                            Name = "B",
                            GroupDetails = new List<GroupDetailEntity>
                            {
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Millonarios")},
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Nacional")},
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Once Caldas")},
                                new GroupDetailEntity{Team = _context.Teams.FirstOrDefault(t => t.Name == "Santa Fe")},
                            },
                            Matches = new List<MatchEntity>
                            {
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(1).AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Millonarios"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Nacional"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(5).AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Millonarios"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Once Caldas"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(5).AddHours(17),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Nacional"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Santa Fe"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(10).AddHours(16),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Santa Fe"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Millonarios"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(10).AddHours(16),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Nacional"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Once Caldas"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(16).AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Nacional"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Millonarios"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(16).AddHours(17),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Santa Fe"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Once Caldas"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(20).AddHours(14),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Once Caldas"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Millonarios"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(20).AddHours(17),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Santa fe"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Nacional"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(35).AddHours(16),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Millonarios"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Santa Fe"),
                                },
                                new MatchEntity
                                {
                                    Date = startDate.AddDays(35).AddHours(16),
                                    Local = _context.Teams.FirstOrDefault(t => t.Name == "Once Caldas"),
                                    Visitor = _context.Teams.FirstOrDefault(t => t.Name == "Nacional"),
                                }
                            }
                        }
                    }
                });

                startDate = DateTime.Today.AddMonths(3).ToUniversalTime();
                endDate = DateTime.Today.AddMonths(5).ToUniversalTime();

                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckTeamsAsync()
        {
            if (!_context.Teams.Any())
            {
                AddTeam("America");
                AddTeam("Argentina");
                AddTeam("Bolivia");
                AddTeam("Brasil");
                AddTeam("Bucaramanga");
                AddTeam("Canada");
                AddTeam("Chile");
                AddTeam("Colombia");
                AddTeam("Costa Rica");
                AddTeam("Ecuador");
                AddTeam("Honduras");
                AddTeam("Junior");
                AddTeam("Medellin");
                AddTeam("Mexico");
                AddTeam("Millonarios");
                AddTeam("Nacional");
                AddTeam("Once Caldas");
                AddTeam("Panama");
                AddTeam("Paraguay");
                AddTeam("Peru");
                AddTeam("Santa Fe");
                AddTeam("Uruguay");
                AddTeam("USA");
                AddTeam("Venezuela");
                AddTeam_("Arsenal");
                AddTeam_("Barcelona");
                AddTeam_("Chelsea");
                AddTeam_("Inter Milan");
                AddTeam_("Leidzi");
                AddTeam_("Leverkusen");
                AddTeam_("Liverpool");
                AddTeam_("Real Madrid");
                AddTeam_("Roma");
                AddTeam_("Valencia");
                await _context.SaveChangesAsync();
            }
        }

        private void AddTeam(string name)
        {
            _context.Teams.Add(new TeamEntity { Name = name, LogoPath = $"~/images/Teams/{name}.jpg" });
        }

        private void AddTeam_(string name)
        {
            _context.Teams.Add(new TeamEntity { Name = name, LogoPath = $"~/images/Teams/{name}.png" });
        }
    }
}
