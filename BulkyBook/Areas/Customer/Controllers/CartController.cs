using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Models;
using Models.ViewModels;
using Stripe;
using Utility;

namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(IUnitOfWork unitOfWork,
            IEmailSender emailSender,
            UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }

        public IActionResult Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCartVM = new ShoppingCartVM()
            {
                orderHeader = new OrderHeader(),
                shoppingCarts = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == claim.Value, includeProperties: "Product")
            };

            shoppingCartVM.orderHeader.OrderTotal = 0;
            shoppingCartVM.orderHeader.ApplicationUserId = claim.Value;
            shoppingCartVM.orderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "Company");


            foreach (ShoppingCart cart in shoppingCartVM.shoppingCarts)
            {
                cart.Price = SD.GetPriceDependingOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                shoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);
                cart.Product.Description = SD.ConvertToRawHtml(cart.Product.Description ?? "");
                if (cart.Product.Description.Length > 100)
                {
                    cart.Product.Description = cart.Product.Description.Substring(0, 99) + "...";
                }
            }



            return View(shoppingCartVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> ResendEmail()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email is empty!");
            }


            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return RedirectToAction("Index");
        }


        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(s => s.Id == cartId, includeProperties: "Product");
            cartFromDb.Count += 1;
            cartFromDb.Price = SD.GetPriceDependingOnQuantity(cartFromDb.Count, cartFromDb.Product.Price, cartFromDb.Product.Price50, cartFromDb.Product.Price100);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(s => s.Id == cartId, includeProperties: "Product");
            if (cartFromDb.Count == 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
                var currentCount = HttpContext.Session.GetInt32(SD.Session_Cart_count) - 1;
                HttpContext.Session.SetInt32(SD.Session_Cart_count, currentCount ?? 0);
                _unitOfWork.Save();


                if (currentCount == 0)
                {

                    return RedirectToAction(controllerName: "Home", actionName: "Index");

                }

            }
            else
            {
                cartFromDb.Count -= 1;
                cartFromDb.Price = SD.GetPriceDependingOnQuantity(cartFromDb.Count, cartFromDb.Product.Price, cartFromDb.Product.Price50, cartFromDb.Product.Price100);
                _unitOfWork.Save();

            }

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(s => s.Id == cartId, includeProperties: "Product");

            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            var currentCount = HttpContext.Session.GetInt32(SD.Session_Cart_count) - 1;
            HttpContext.Session.SetInt32(SD.Session_Cart_count, currentCount ?? 0);
            _unitOfWork.Save();

            if (currentCount == 0)
            {
                return RedirectToAction(controllerName: "Home", actionName: "Index");

            }

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Summary()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCartVM = new ShoppingCartVM()
            {
                orderHeader = new OrderHeader(),
                shoppingCarts = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == claim.Value, includeProperties: "Product")
            };

            shoppingCartVM.orderHeader.OrderTotal = 0;
            shoppingCartVM.orderHeader.ApplicationUserId = claim.Value;
            shoppingCartVM.orderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "Company");


            foreach (ShoppingCart cart in shoppingCartVM.shoppingCarts)
            {
                cart.Price = SD.GetPriceDependingOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                shoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);

            }

            shoppingCartVM.orderHeader.Name = shoppingCartVM.orderHeader.ApplicationUser.Name;
            shoppingCartVM.orderHeader.PhoneNumber = shoppingCartVM.orderHeader.ApplicationUser.PhoneNumber;

            shoppingCartVM.orderHeader.StreetAddress = shoppingCartVM.orderHeader.ApplicationUser.StreetAddress;
            shoppingCartVM.orderHeader.City = shoppingCartVM.orderHeader.ApplicationUser.City;
            shoppingCartVM.orderHeader.State = shoppingCartVM.orderHeader.ApplicationUser.State;
            shoppingCartVM.orderHeader.PostalCode = shoppingCartVM.orderHeader.ApplicationUser.PostalCode;

            return View(shoppingCartVM);


        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(string StripeToken)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCartVM.orderHeader.ApplicationUser = _unitOfWork.ApplicationUser.
                GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "Company");

            shoppingCartVM.shoppingCarts = _unitOfWork.ShoppingCart.
                GetAll(s => s.ApplicationUserId == claim.Value, includeProperties: "Product");

            shoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusPending;
            shoppingCartVM.orderHeader.OrderStatus = SD.StatusPending;
            shoppingCartVM.orderHeader.ApplicationUserId = claim.Value;
            shoppingCartVM.orderHeader.OrderDate = DateTime.Now;

            _unitOfWork.OrderHeader.Add(shoppingCartVM.orderHeader);
            _unitOfWork.Save();

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            foreach(var item in shoppingCartVM.shoppingCarts)
            {
                item.Price = SD.GetPriceDependingOnQuantity(item.Count, item.Product.Price,
                    item.Product.Price50, item.Product.Price100);
                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductId = item.ProductId, 
                    Cout = item.Count, 
                    Price = item.Price,
                    OrderId = shoppingCartVM.orderHeader.Id, 
                    
                };
                shoppingCartVM.orderHeader.OrderTotal += orderDetails.Cout * orderDetails.Price;
                _unitOfWork.OrderDetails.Add(orderDetails);
            }

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCartVM.shoppingCarts);
            HttpContext.Session.SetInt32(SD.Session_Cart_count, 0);



            if(StripeToken == null)
            {
                shoppingCartVM.orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
                shoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                shoppingCartVM.orderHeader.OrderStatus = SD.StatusApproved;
            }
            else
            {
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(shoppingCartVM.orderHeader.OrderTotal * 100),
                    Currency = "EUR",
                    Description = "Order ID : " + shoppingCartVM.orderHeader.Id,
                    Source = StripeToken
                };

                var service = new ChargeService();
                Charge charge = service.Create(options);

                if(charge.BalanceTransactionId == null)
                {
                    shoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusRejected;
                }
                else
                {
                    shoppingCartVM.orderHeader.TransactionId = charge.BalanceTransactionId;

                }

                if(charge.Status.ToLower() == "succeeded")
                {
                    shoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    shoppingCartVM.orderHeader.OrderStatus = SD.StatusApproved;
                    shoppingCartVM.orderHeader.PaymentDate = DateTime.Now;

                }
            }

            _unitOfWork.Save();

            return RedirectToAction("OrderConfirmation", "Cart", new { id = shoppingCartVM.orderHeader.Id });   

        }


        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }


    }
}