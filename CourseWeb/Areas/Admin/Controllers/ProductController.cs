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

    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unityOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unityOfWork,IWebHostEnvironment webHostEnvironment)
        {
            _unityOfWork = unityOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unityOfWork.Product.GetAll(includeProperties:"Category").ToList();
            
            return View(objProductList);
        }

        //nell' upsert possiamo avere un id (nel caso dell'update), ma possiamo anche non averlo (nel caso dell' insert)
        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> CategoryList = _unityOfWork.Category.GetAll().Select(u => new SelectListItem
            //{
            //    Text = u.Name,
            //    Value = u.Id.ToString()
            //});

            //si può assegnare un nome casuale a categoryList dopo ViewBag. e indica la chiave e il CategoryList dopo = indica il valore
            //ViewBag.CategoryList = CategoryList;

            //ViewData["CategoryList"] = CategoryList;

            ProductVM productVM = new()
            {
                CategoryList = _unityOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };

            if(id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unityOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }

                
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {


            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    //qui l'immagine avra un identificativo random (GUID) e si aggiunmgerà la stessa estenzione del file di partenza
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    //dove lochiamo l'immagine
                    string productpath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath,productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productpath, fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                
                if(productVM.Product.Id==0)
                {
                    _unityOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    _unityOfWork.Product.Update(productVM.Product);
                    TempData["success"] = "Product updated successfully";
                }
                    _unityOfWork.Save();
                
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unityOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }
       
        //rimuovo anche la delete view dal folder view in aerea/product


        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? objProduct = _unityOfWork.Product.Get(u => u.Id == id);

        //    if (objProduct == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(objProduct);
        //}
        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePOST(int? id)
        //{
        //    Product? obj = _unityOfWork.Product.Get(u => u.Id == id);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
        //    _unityOfWork.Product.Remove(obj);
        //    _unityOfWork.Save();
        //    TempData["success"] = "Product deleted successfully";
        //    return RedirectToAction("Index");
        //}

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unityOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = objProductList});
        }
        [HttpDelete]
       public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unityOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            var oldImagePath = 
                Path.Combine(_webHostEnvironment.WebRootPath, 
                productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unityOfWork.Product.Remove(productToBeDeleted);
            _unityOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}