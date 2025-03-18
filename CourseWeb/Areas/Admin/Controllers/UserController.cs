using Course.DataAccess.Data;
using Course.DataAccess.Repository.IRepository;
using Course.Models;
using Course.Models.ViewModels;
using Course.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace CourseWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagment(string id)
        {
            RoleManagmentVM roleManagmentVM = new()
            {
                ApplicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id),
                RoleList = _db.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CompanyList = _db.Companies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            roleManagmentVM.ApplicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            return View(roleManagmentVM);
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUsertList = _db.ApplicationUsers.Include(u => u.Company).ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in objUsertList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

                if (user.Company == null)
                {
                    user.Company = new() { Name = "" };
                }
            }

            return Json(new { data = objUsertList });
        }
        [HttpPost]
        public IActionResult RoleManagement(RoleManagmentVM roleManagmentVM)
        {
            // Retrieve the application user from the database
            var appUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == roleManagmentVM.ApplicationUser.Id);
            appUser.Role = roleManagmentVM.ApplicationUser.Role;
            if (appUser == null)
            {
                return NotFound();
            }
            var roleComapny = _db.Roles.FirstOrDefault(u=>u.Name == SD.Role_Copany);
            // Update the role of the application user
            if (appUser.Role != null)
            {
                var userRolesToRemove = _db.UserRoles.FirstOrDefault(u => u.UserId == appUser.Id);
                if (roleManagmentVM.ApplicationUser.Role == roleComapny.Id)
                {
                    _db.UserRoles.Remove(userRolesToRemove);
                    _db.UserRoles.Add(new IdentityUserRole<string>
                    {
                        UserId = appUser.Id,
                        RoleId = roleManagmentVM.ApplicationUser.Role
                    });
                    _db.ApplicationUsers.FirstOrDefault(u => u.Id == appUser.Id).CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                }
                else
                {
                    _db.UserRoles.Remove(userRolesToRemove);
                    _db.ApplicationUsers.FirstOrDefault(u => u.Id == appUser.Id).CompanyId = null;
                    _db.UserRoles.Add(new IdentityUserRole<string>
                    {
                        UserId = appUser.Id,
                        RoleId = roleManagmentVM.ApplicationUser.Role
                    });
                }

            }
            else
            {
                _db.UserRoles.Add(new IdentityUserRole<string>
                {
                    UserId = appUser.Id,
                    RoleId = roleManagmentVM.ApplicationUser.Role
                });
            }

            // Save changes to the database
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string? id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while locking/unlocking" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();

            return Json(new { success = true, message = "Operation  Successful" });
        }

        #endregion
    }
}