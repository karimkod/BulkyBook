using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.Models;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();

            if (id == null)
            {
                return View(coverType);
            }

            coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault());

            if (coverType == null)
            {
                return NotFound();
            }

            return View(coverType);


        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.CoverType.GetAll() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                if (coverType.Id == 0)
                {
                    _unitOfWork.CoverType.Add(coverType);
                    
                }
                else
                {
                    _unitOfWork.CoverType.Update(coverType);
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));

            }
            return View(coverType);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            CoverType coverTypefromBD = _unitOfWork.CoverType.Get(id);
            if(coverTypefromBD == null)
            {
                return Json(new { success = false, message="Deletion failed" });
            }else
            {
                _unitOfWork.CoverType.Remove(coverTypefromBD);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Deletion Successeded" });

            }
        }

    }
}