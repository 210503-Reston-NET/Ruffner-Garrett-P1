using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service;
using StoreModels;

namespace WebUI.Controllers
{
    public class OrdersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private IAuthorizationService _AuthorizationService;
   
        private IServices _service;
        public OrdersController(IServices service, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IAuthorizationService AuthorizationService)
        {
          this._service = service;
          this._userManager = userManager;
          this._signInManager = signInManager;
          this._AuthorizationService = AuthorizationService;
        }

        [Authorize]
        public IActionResult Index(string sortOrder)
        {
            ViewBag.TotalSortParm = String.IsNullOrEmpty(sortOrder) ? "total_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            //get orders from customer
            Guid UserId = Guid.Parse(_userManager.GetUserId(User));
            IEnumerable<Order> orders = _service.GetOrdersByCustomerId(UserId);
            //var students = from s in db.Students
            //               select s;
            switch (sortOrder)
            {
                case "total_desc":
                    orders = orders.OrderByDescending(s => s.Total);
                    break;
                case "Date":
                    orders = orders.OrderBy(s => s.Date);
                    break;
                case "date_desc":
                    orders = orders.OrderByDescending(s => s.Date);
                    break;
                default:
                    orders = orders.OrderBy(s => s.Total);
                    break;
            }
            return View(orders.ToList());
           
           // return View("Orders", orders);
        }

        [Authorize]
        [Route("Orders/Details/{OrderID}")]
        public ActionResult Details(int OrderID)
        {
            return View();
        }
    }
}