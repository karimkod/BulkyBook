using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {

            Category category = new Category();

            if(id == null)
            {
                return View(category);
            }

            category = _unitOfWork.Category.Get(id.GetValueOrDefault());
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.Category.GetAll() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if(ModelState.IsValid)
            {
                if(category.Id == 0)
                {
                    _unitOfWork.Category.Add(category);
                    
                }else
                {
                    _unitOfWork.Category.Update(category);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var category = _unitOfWork.Category.Get(id); 
            if(category == null)
            {
                return Json(new { success = false, message = "Problem while deleting" });
            }else
            {
                _unitOfWork.Category.Remove(category);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Deleted Successfully" });
            }
        }


    }
}