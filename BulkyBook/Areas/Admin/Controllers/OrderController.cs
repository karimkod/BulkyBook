using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using Stripe;
using Utility;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderDetailsVM OrderDetailsVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Details(int id)
        {
            OrderDetailsVM = new OrderDetailsVM();
            OrderDetailsVM.OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == id, includeProperties: "ApplicationUser");
            OrderDetailsVM.OrderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderId == OrderDetailsVM.OrderHeader.Id, includeProperties: "Product");

            return View(OrderDetailsVM  );
        }


        [Authorize(Roles =SD.Role_Admin +"," + SD.Role_Employee)]
        public IActionResult StartProcessing(int id)
        {
            var orderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == id);
            orderFromDb.OrderStatus = SD.StatusInProcess;
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == OrderDetailsVM.OrderHeader.Id);
            orderFromDb.OrderStatus = SD.StatusShipped;
            orderFromDb.ShippingDate = DateTime.Now;
            orderFromDb.Carrier = OrderDetailsVM.OrderHeader.Carrier;
            orderFromDb.TrackingNumber = OrderDetailsVM.OrderHeader.TrackingNumber;
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder(int id)
        {
            var orderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == id);
            if (orderFromDb.PaymentStatus == SD.StatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Amount = Convert.ToInt32(orderFromDb.OrderTotal * 100),
                    Reason = RefundReasons.RequestedByCustomer,
                    Charge = orderFromDb.TransactionId

                };
                var service = new RefundService();
                Refund refund = service.Create(options);

                orderFromDb.OrderStatus = SD.StatusRefunded;
                orderFromDb.PaymentStatus = SD.StatusRefunded;
            }
            else
            {
                orderFromDb.OrderStatus = SD.StatusCancelled;
                orderFromDb.PaymentStatus = SD.StatusCancelled;
            }

            _unitOfWork.Save();


            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Details")]
        public IActionResult Details(string stripeToken)
        {

            var orderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == OrderDetailsVM.OrderHeader.Id);
            if (stripeToken != null)
            {
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(orderFromDb.OrderTotal * 100),
                    Currency = "EUR",
                    Description = "Order ID : " + orderFromDb.Id,
                    Source = stripeToken
                };

                var service = new ChargeService();
                Charge charge = service.Create(options);

                if (charge.BalanceTransactionId == null)
                {
                    orderFromDb.PaymentStatus = SD.PaymentStatusRejected;
                }
                else
                {
                    orderFromDb.TransactionId = charge.BalanceTransactionId;

                }

                if (charge.Status.ToLower() == "succeeded")
                {
                    orderFromDb.PaymentStatus = SD.PaymentStatusApproved;
                    orderFromDb.PaymentDate = DateTime.Now;

                }
            }

            _unitOfWork.Save();

            return RedirectToAction("Details", "Order", new { id = orderFromDb.Id });
        }

        [HttpGet]
        public IActionResult GetOrderList(string status)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> orderHeaderList;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                orderHeaderList = _unitOfWork.OrderHeader.GetAll(
                                        u => u.ApplicationUserId == claim.Value,
                                        includeProperties: "ApplicationUser");
            }

            switch (status)
            {
                case "pending":
                    orderHeaderList = orderHeaderList.Where(o => o.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.StatusApproved ||
                                                            o.OrderStatus == SD.StatusInProcess ||
                                                            o.OrderStatus == SD.StatusPending);
                    break;
                case "completed":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.StatusShipped);
                    break;
                case "rejected":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.StatusCancelled ||
                                                            o.OrderStatus == SD.StatusRefunded ||
                                                            o.OrderStatus == SD.PaymentStatusRejected);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaderList });
        }



    }
}