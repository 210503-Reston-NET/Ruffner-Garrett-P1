using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service;
using StoreModels;
using Serilog;
using Microsoft.AspNetCore.Authorization.Infrastructure;

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
            try{
                ViewBag.TotalSortParm = String.IsNullOrEmpty(sortOrder) ? "total_desc" : "";
                ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

                //get orders from customer
                Guid UserId = Guid.Parse(_userManager.GetUserId(User));
                List<Order> ordersl = _service.GetOrdersByCustomerId(UserId);

                //check if their are no orders,
                if(ordersl.Count ==0){
                    Log.Information("No orders from customer {0}",UserId);
                }
                var orders = ordersl.AsEnumerable();
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
                return View(orders);
            }catch(Exception ex){
                Log.Error("Could not get orders From customer\n{0}\n{1}", ex.Message, ex.StackTrace);
                return new LocationController(_service, _userManager, _signInManager, _AuthorizationService).Index();
            }
        }

        [Authorize]
        [Route("Orders/OrderDetails/{OrderID}")]
        [Route("Orders/OrderDetails/{OrderID}/{LocationID}")]
        public ActionResult OrderDetails(int OrderID, int? LocationID)
        {
            try{
                if(LocationID != null){
                    ViewBag.LocationID = LocationID;
                }
                var order = _service.GetOrder(OrderID);
            return View(order);
            }catch(Exception ex){
                Log.Warning("Could not get order with ID: {0}\n {1}", OrderID, ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize]
        [Route("/Orders/{sortOrder}/{LocationID}")]
        public async Task<IActionResult> LocationOrders(string sortOrder, int LocationID){

            try{
                var item = _service.GetAllLocations().Select(location => location).Where(location => location.LocationID == LocationID).FirstOrDefault();
                if ((await _AuthorizationService.AuthorizeAsync(User, item, new ClaimsAuthorizationRequirement("Owner", new List<string>{item.LocationID.ToString()}))).Succeeded)
                {
                    ViewBag.TotalSortParm = String.IsNullOrEmpty(sortOrder) ? "total_desc" : "";
                    ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

                    //get orders from customer
                    // Guid UserId = Guid.Parse(_userManager.GetUserId(User));
                    List<Order> ordersl = _service.GetOrdersByLocationId(LocationID);

                    //check if their are no orders,
                    if(ordersl.Count ==0){
                        Log.Information("No orders from Location {0}",LocationID);
                    }
                    var orders = ordersl.AsEnumerable();
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
                    ViewBag.LocationID= LocationID;
                    return View(orders);
                }
                return View("/Views/Home/index.cshtml");
                //return new LocationController(_service, _userManager, _signInManager, _AuthorizationService).Index();         
            }catch(Exception ex){
                Log.Error("Could not get orders From Location\n{0}\n{1}", ex.Message, ex.StackTrace);
                //return new LocationController(_service, _userManager, _signInManager, _AuthorizationService).Index();
                return View("/Views/Home/index.cshtml");
            }
        }


    }
}