using System.Security.Cryptography;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Service;
using WebUI.Models;
namespace WebUI.Controllers
{
    public class CustomerController : Controller
    {
        private IServices _service;
        public CustomerController(IServices service)
        {
          this._service = service;
        }

        public ActionResult Index()
        {
          return View(_service.GetAllCustomers().Select(c => new CustomerVM(c)).ToList());
        }
        public IActionResult Create()
        {
          return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CustomerVM c)
        {
          try
          {
            if(ModelState.IsValid)
            {
              _service.AddCustomer(c.Name,c.Address,c.Email);
              return RedirectToAction(nameof(Index));
            }
            return View();
          }catch{
            return View();
          }
          
        }
    }
}