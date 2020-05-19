using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BulkyBook.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Models;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {

        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAll()
        {

            var users = _db.ApplicationUsers.Include(u => u.Company).ToList();
            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (ApplicationUser user in users)
            {
                var userRole = userRoles.FirstOrDefault(r => r.UserId == user.Id);
                user.Role = roles.FirstOrDefault(r => r.Id == userRole.RoleId).Name;

                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    
                    };
                }

            }

            return Json(new { data = users });
        }


        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var userFromDb = _db.ApplicationUsers.Find(id); 
            if(userFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }else
            {
                if(userFromDb.LockoutEnd != null && userFromDb.LockoutEnd > DateTime.Now)
                {
                    userFromDb.LockoutEnd = DateTime.Now; 
                }else
                {
                    userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
                }
                _db.SaveChanges();
                return Json(new { success = true, message = "Operation successful" });

            }

        }




    }
}