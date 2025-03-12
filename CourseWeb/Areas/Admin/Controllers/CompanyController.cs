using Course.DataAccess.Repository.IRepository;
using Course.Models;
using Course.Models.ViewModels;
using Course.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace CourseWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unityOfWork;
        public CompanyController(IUnitOfWork unityOfWork)
        {
            _unityOfWork = unityOfWork;
        }
        public IActionResult Index()
        {
            List<Company> objProductList = _unityOfWork.Company.GetAll().ToList();
            
            return View(objProductList);
        }

        //nell' upsert possiamo avere un id (nel caso dell'update), ma possiamo anche non averlo (nel caso dell' insert)
        public IActionResult Upsert(int? id)
        {

            if(id == null || id == 0)
            {
                return View(new Company());
            }
            else
            {
                //update
                var companyObj = _unityOfWork.Company.Get(u => u.Id == id);
                return View(companyObj);
            }

                
        }
        [HttpPost]
        public IActionResult Upsert(Company company)
        {

            if (ModelState.IsValid)
            {
                
                if(company.Id==0)
                {
                    _unityOfWork.Company.Add(company);
                    TempData["success"] = "Company created successfully";
                }
                else
                {
                    _unityOfWork.Company.Update(company);
                    TempData["success"] = "Company updated successfully";
                }
                    _unityOfWork.Save();
                
                return RedirectToAction("Index");
            }
            else
            {
                return View(company);
            }
        }
 

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanytList = _unityOfWork.Company.GetAll().ToList();
            return Json(new {data = objCompanytList});
        }
        [HttpDelete]
       public IActionResult Delete(int? id)
        {
            var companyToBeDeleted = _unityOfWork.Company.Get(u => u.Id == id);
            if (companyToBeDeleted == null)
            {
                return Json(new { success = false, massage = "Error while deleting" });
            }

            _unityOfWork.Company.Remove(companyToBeDeleted);
            _unityOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}