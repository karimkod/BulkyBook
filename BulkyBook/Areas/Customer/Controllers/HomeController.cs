using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using DataAccess.Repository.IRepository;
using Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Utility;
using Microsoft.AspNetCore.Http;

namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
          
            if(claim != null)
            {
                var count = _unitOfWork.ShoppingCart
                    .GetAll(u => u.ApplicationUserId == claim.Value)
                    .ToList()
                    .Count();
                HttpContext.Session.SetInt32(SD.Session_Cart_count, count);
            }
            

            return View(_unitOfWork.Product.GetAll());
        }

        public IActionResult Details(int id)
        {
            var productFromDb = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id, includeProperties:"Category,CoverType");
            var shoppingCart = new ShoppingCart()
            {
                ProductId = productFromDb.Id,
                Product = productFromDb
            };
            return View(shoppingCart);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;
            if(ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCart.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    c => c.ApplicationUserId == shoppingCart.ApplicationUserId && c.ProductId == shoppingCart.ProductId,
                    includeProperties:"Product");
                if(cartFromDb == null)
                {
                    _unitOfWork.ShoppingCart.Add(shoppingCart);
                }else
                {
                    cartFromDb.Count += shoppingCart.Count; 
                }

                _unitOfWork.Save();

                var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.Session_Cart_count, count);
                return RedirectToAction(nameof(Index));
            }else
            {
                var productFromDb = _unitOfWork.Product.GetFirstOrDefault(p => shoppingCart.ProductId == p.Id, includeProperties: "Category,CoverType");
                shoppingCart = new ShoppingCart()
                {
                    ProductId = productFromDb.Id,
                    Product = productFromDb
                };
                return View(shoppingCart);
            }
        }

    }
}
