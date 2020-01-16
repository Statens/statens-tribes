using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Statens.Tribes.App.Domain.Interfaces;
using Statens.Tribes.App.Domain.Model;
using Statens.Tribes.App.Models;

namespace Statens.Tribes.App.Controllers
{
    [Authorize]
    public class TribeController : Controller
    {
        private readonly ITribeRepository tribeRepository;

        public TribeController(ITribeRepository tribeRepository)
        {
            this.tribeRepository = tribeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var tribes = await tribeRepository.ReadAllAsync();

            var model = tribes.Select(x => new TribeViewModel
                {
                    Name = x.Name,
                    Id = x.Id
                }).ToList();
            return View(model: model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var m = new CreateTribeModel();
            return View(model: m);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTribeModel model)
        {
            await tribeRepository.SaveAsync(new Tribe()
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.Name,
                Type = model.TribeType
            });
            return RedirectToAction(nameof(Index));
        }
    }
}
