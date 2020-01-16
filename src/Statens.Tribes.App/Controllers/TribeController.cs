using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Statens.Tribes.App.Domain.Interfaces;
using Statens.Tribes.App.Domain.Model;
using Statens.Tribes.App.Models;

namespace Statens.Tribes.App.Controllers
{
    public class TribeController : Controller
    {
        private readonly ITribeRepository tribeRepository;

        public TribeController(ITribeRepository tribeRepository)
        {
            this.tribeRepository = tribeRepository;
        }

        public IActionResult Index()
        {
            var tribes = tribeRepository.ReadAll()
                .Select(x => new TribeViewModel
                {
                    Name = x.Name
                }).ToList();
            return View(model: tribes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var m = new CreateTribeModel();
            return View(model: m);
        }

        [HttpPost]
        public IActionResult Create(CreateTribeModel model)
        {
            tribeRepository.Save(new Tribe()
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.Name,
                Type = model.TribeType
            });
            return RedirectToAction(nameof(Index));
        }
    }
}
