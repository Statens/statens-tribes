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
            if (!ModelState.IsValid)
            {
                return View(model: model);
            }

            await tribeRepository.SaveAsync(new Tribe
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.Name,
                Type = model.TribeType
            });
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var tribe = await tribeRepository.ReadAsync(id);
            var m = new UpdateTribeModel
            {
                Name = tribe.Name,
                TribeType = tribe.Type
            };
            return View(model: m);
        }

        [HttpPost]
        public async Task<IActionResult> Update(string id, UpdateTribeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model: model);
            }

            var tribe = await tribeRepository.ReadAsync(id);
            tribe.Name = model.Name;

            await tribeRepository.SaveAsync(tribe);

            return View(model: model);
        }
    }
}
