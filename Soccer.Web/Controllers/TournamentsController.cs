using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using Soccer.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TournamentsController : Controller
    {
        private readonly DataContext _context;
        private readonly ICoverterHelper _converter;
        private readonly IImageHelper _image;
        private readonly ICombosHelper _combo;
        private readonly IMatchHelper _match;

        public TournamentsController(DataContext context,
                                     ICoverterHelper converter,
                                     IImageHelper image, 
                                     ICombosHelper combo,
                                     IMatchHelper match)
        {
            _context = context;
            _converter = converter;
            _image = image;
            _combo = combo;
            _match = match;
        }

        #region Tournament
        public async Task<ActionResult> Index()
        {
            return View(await _context.Tournaments.Include(t => t.Groups)
                                                  .OrderBy(t => t.StartDate)
                                                  .ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TournametViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = string.Empty;

                if (model.LogoFile != null)
                {
                    path = await _image.UploadImageAsync(model.LogoFile, "Tournaments");
                }

                TournamentEntity tournament = _converter.ToTournamentEntity(model, path, true);
                _context.Add(tournament);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TournamentEntity tournament = await _context.Tournaments.FindAsync(id);

            if (tournament == null)
            {
                return NotFound();
            }

            TournametViewModel model = _converter.ToTournamentViewModel(tournament);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TournametViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = model.LogoPath;

                if (model.LogoFile != null)
                {
                    path = await _image.UploadImageAsync(model.LogoFile, "Tournaments");
                }

                TournamentEntity tournament = _converter.ToTournamentEntity(model, path, false);
                _context.Update(tournament);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TournamentEntity tournament = await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null)
            {
                return NotFound();
            }

            _context.Tournaments.Remove(tournament);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TournamentEntity tournament = await _context.Tournaments.Include(t => t.Groups)
                                                                    .ThenInclude(t => t.Matches)
                                                                    .ThenInclude(t => t.Local)
                                                                    .Include(t => t.Groups)
                                                                    .ThenInclude(t => t.Matches)
                                                                    .ThenInclude(t => t.Visitor)
                                                                    .Include(t => t.Groups)
                                                                    .ThenInclude(t => t.GroupDetails)
                                                                    .FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null)
            {
                return NotFound();
            }

            return View(tournament);
        }
        #endregion

        #region Groups
        public async Task<IActionResult> AddGroup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TournamentEntity tournament = await _context.Tournaments.FindAsync(id);

            if (tournament == null)
            {
                return NotFound();
            }

            GroupViewModel model = new GroupViewModel
            {
                Tournament = tournament,
                TournamentId = tournament.Id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGroup(GroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                GroupEntity entity = await _converter.ToGroupEntityAsync(model, true);
                _context.Add(entity);
                await _context.SaveChangesAsync();

                return RedirectToAction($"{ nameof(Details)}/{ model.TournamentId}");
            }

            return View(model);
        }

        public async Task<IActionResult> EditGroup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GroupEntity entity = await _context.Groups.Include(g => g.Tournament)
                                                      .FirstOrDefaultAsync(g => g.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            GroupViewModel model = _converter.ToGroupViewModel(entity);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGroup(GroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                GroupEntity entity = await _converter.ToGroupEntityAsync(model, false);
                _context.Update(entity);
                await _context.SaveChangesAsync();

                return RedirectToAction($"{nameof(Details)}/{model.TournamentId}");
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteGroup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GroupEntity entity = await _context.Groups.Include(g => g.Tournament)
                                                      .FirstOrDefaultAsync(g => g.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            _context.Remove(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction($"{nameof(Details)}/{entity.Tournament.Id}");
        }

        public async Task<IActionResult> DetailsGroup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GroupEntity entity = await _context.Groups.Include(g => g.Matches)
                                                      .ThenInclude(g => g.Local)
                                                      .Include(g => g.Matches)
                                                      .ThenInclude(g => g.Visitor)
                                                      .Include(g => g.Tournament)
                                                      .Include(g => g.GroupDetails)
                                                      .ThenInclude(g => g.Team)
                                                      .FirstOrDefaultAsync(g => g.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }
        #endregion

        #region GroupDetail
        public async Task<IActionResult> AddGroupDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GroupEntity entity = await _context.Groups.FindAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            GroupDetailViewModel model = new GroupDetailViewModel
            {
                Group = entity,
                GroupId = entity.Id,
                Teams = _combo.GetComboTeams()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGroupDetail(GroupDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                GroupDetailEntity entity = await _converter.ToGroupDetailEntityAsync(model, true);
                _context.Add(entity);
                await _context.SaveChangesAsync();

                return RedirectToAction($"{nameof(DetailsGroup)}/{model.GroupId}");
            }

            model.Group = await _context.Groups.FindAsync(model.GroupId);
            model.Teams = _combo.GetComboTeams();

            return View(model);
        }

        public async Task<IActionResult> AddMatch(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GroupEntity entity = await _context.Groups.FindAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            MatchViewModel model = new MatchViewModel
            {
                Date = DateTime.Today,
                Group = entity,
                GroupId = entity.Id,
                Teams = _combo.GetComboTeams(entity.Id)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMatch(MatchViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.LocalId != model.VisitorlId)
                {
                    MatchEntity entity = await _converter.ToMatchEntityAsync(model, true);
                    _context.Add(entity);
                    await _context.SaveChangesAsync();

                    return RedirectToAction($"{nameof(DetailsGroup)}/{model.GroupId}");
                }

                ModelState.AddModelError(string.Empty, "El local y el visitantes estan en diferentes equipos");
            }

            model.Group = await _context.Groups.FindAsync(model.GroupId);
            model.Teams = _combo.GetComboTeams(model.GroupId);

            return View(model);
        }

        public async Task<IActionResult> EditMatch(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MatchEntity entity = await _context.Matches.Include(t => t.Group)
                                                       .Include(t => t.Local)
                                                       .Include(t => t.Visitor)
                                                       .FirstOrDefaultAsync(t => t.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            MatchViewModel model = _converter.ToMatchViewModelAsync(entity);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMatch(MatchViewModel model)
        {
            if (ModelState.IsValid)
            {
                MatchEntity entity = await _converter.ToMatchEntityAsync(model, false);
                _context.Update(entity);
                await _context.SaveChangesAsync();

                return RedirectToAction($"{nameof(DetailsGroup)}/{model.GroupId}");
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteMatch(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MatchEntity entity = await _context.Matches.Include(t => t.Group)
                                                                  .FirstOrDefaultAsync(t => t.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            _context.Matches.Remove(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction($"{nameof(DetailsGroup)}/{entity.Group.Id}");
        }

        public async Task<IActionResult> EditGroupDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GroupDetailEntity entity = await _context.GroupDetails.Include(t => t.Group)
                                                                  .Include(t => t.Team)
                                                                  .FirstOrDefaultAsync(t => t.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            GroupDetailViewModel model = _converter.ToGroupDetailViewModelAsync(entity);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGroupDetail(GroupDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                GroupDetailEntity entity = await _converter.ToGroupDetailEntityAsync(model, false);
                _context.Update(entity);
                await _context.SaveChangesAsync();

                return RedirectToAction($"{nameof(DetailsGroup)}/{model.GroupId}");
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteGroupDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GroupDetailEntity entity = await _context.GroupDetails.Include(t => t.Group)
                                                                  .FirstOrDefaultAsync(t => t.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            _context.GroupDetails.Remove(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction($"{nameof(DetailsGroup)}/{entity.Group.Id}");
        }
        #endregion

        #region Match
        public async Task<IActionResult> CloseMatch(int? id) //valida id al cerrar
        {
            if (id == null)
            {
                return NotFound();
            }

            //buscamos el partido
            var matchentity = await _context.Matches.Include(m => m.Group)
                                                    .Include(m => m.Local)
                                                    .Include(m => m.Visitor)
                                                    .FirstOrDefaultAsync(m => m.Id == id);

            if (matchentity == null)
            {
                return NotFound();
            }

            //armamos el modelo
            ClosMatchViewModel model = new ClosMatchViewModel
            {
                Group = matchentity.Group,
                GroupId = matchentity.Group.Id,
                Local = matchentity.Local,
                LocalId = matchentity.Local.Id,
                MatchId = matchentity.Id,
                Visitor = matchentity.Visitor,
                VisitorId = matchentity.Visitor.Id
            };

            return View(model); //el model pide cuales fueron los goles del loca y visitor y una opciòn de close
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseMatch(ClosMatchViewModel model)
        {
            if (ModelState.IsValid)
            {
                //envìa el partido y cantidad de goles realizados por el local y el visitor
                await _match.CloseMatchAsync(model.MatchId, model.GoalsLocal.Value, model.GoalsVisitor.Value);

                //direcciona al detailgroup de ese grupo
                return RedirectToAction($"{nameof(DetailsGroup)}/{model.GroupId}");
            }

            //si hay error vuelve a armar
            model.Group = await _context.Groups.FindAsync(model.GroupId);
            model.Local = await _context.Teams.FindAsync(model.LocalId);
            model.Visitor = await _context.Teams.FindAsync(model.VisitorId);

            return View(model);
        }
        #endregion
    }
}
