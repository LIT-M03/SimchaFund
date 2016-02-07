using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication12.Models;
using SimchaFund.Data;

namespace MvcApplication12.Controllers
{
    public class SimchasController : Controller
    {
        public ActionResult Index()
        {
            if (TempData["newSimchaId"] != null)
            {
                ViewBag.Message = "New Simcha Created! Id: " + TempData["newSimchaId"];
            }
            var mgr = new SimchaFundManager(Properties.Settings.Default.ConStr);
            var viewModel = new SimchaIndexViewModel();
            viewModel.TotalContributors = mgr.GetContributorCount();
            viewModel.Simchas = mgr.GetAllSimchas();
            return View(viewModel);
        }

        public ActionResult New(string name, DateTime date)
        {
            var mgr = new SimchaFundManager(Properties.Settings.Default.ConStr);
            Simcha simcha = new Simcha {Name = name, Date = date};
            mgr.AddSimcha(simcha);
            TempData["newSimchaId"] = simcha.Id;
            return RedirectToAction("index");
        }

    }
}
