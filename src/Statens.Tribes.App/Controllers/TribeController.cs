using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Statens.Tribes.App.Domain.Interfaces;
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
            var tribes = tribeRepository.ReadAll();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
