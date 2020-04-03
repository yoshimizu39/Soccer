using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Soccer.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Helpers
{
    public class CombosHelper : ICombosHelper
    {
        private readonly DataContext _context;

        public CombosHelper(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetComboTeams()
        {
            List<SelectListItem> list = _context.Teams.Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = $"{t.Id}"
            }).OrderBy(t => t.Text)
              .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Select a Team]",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboTeams(int id)
        {
            List<SelectListItem> list = _context.GroupDetails.Include(t => t.Team)
                                                             .Where(t => t.Group.Id == id)
                                                             .Select(t => new SelectListItem
                                                             {
                                                                 Text = t.Team.Name,
                                                                 Value = $"{t.Team.Id}"
                                                             })
                                                             .OrderBy(t => t.Text)
                                                             .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Select a team...]",
                Value = "0"
            });

            return list;
        }
    }
}
