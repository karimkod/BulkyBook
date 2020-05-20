using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using BulkyBook.Models;
using Dapper;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

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

            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@Id", id);
            coverType = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Usp_CoverType_Get, parameter);

            if (coverType == null)
            {
                return NotFound();
            }

            return View(coverType);


        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.SP_Call.List<CoverType>(SD.Usp_CoverType_GetAll,null) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Name", coverType.Name);

                if (coverType.Id == 0)
                {
                    _unitOfWork.SP_Call.Execute(SD.Usp_CoverType_Create, parameters);
                    
                }
                else
                {
                    parameters.Add("@Id", coverType.Id);
                    _unitOfWork.SP_Call.Execute(SD.Usp_CoverType_Update, parameters);
                }

                return RedirectToAction(nameof(Index));

            }
            return View(coverType);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@Id", id);
            CoverType coverTypefromBD = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Usp_CoverType_Get, parameter);
            if(coverTypefromBD == null)
            {
                return Json(new { success = false, message="Deletion failed" });
            }else
            {
                _unitOfWork.SP_Call.Execute(SD.Usp_CoverType_Delete, parameter);
                return Json(new { success = true, message = "Deletion Successeded" });

            }
        }

    }
}