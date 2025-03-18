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
        public ProductController(IUnitOfWork unityOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unityOfWork = unityOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unityOfWork.Product.GetAll(includeProperties: "Category").ToList();

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

            if (id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                //update (includeProperties deve matchare la prop in dbcontext)
                productVM.Product = _unityOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductImages");
                return View(productVM);
            }


        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
        {


            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
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


                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        //qui l'immagine avra un identificativo random (GUID) e si aggiunmgerà la stessa estenzione del file di partenza
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productVM.Product.Id;
                        //dove lochiamo l'immagine
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl =  @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.Product.Id
                        };

                        if (productVM.Product.ProductImages == null)
                        {
                            productVM.Product.ProductImages = new List<ProductImage>();
                        }

                        productVM.Product.ProductImages.Add(productImage);
                        

                    }

                    _unityOfWork.Product.Update(productVM.Product);
                    _unityOfWork.Save();





                    //    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    //    {
                    //        //delete the old image
                    //        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                    //        if (System.IO.File.Exists(oldImagePath))
                    //        {
                    //            System.IO.File.Delete(oldImagePath);
                    //        }
                    //    }

                    //    using (var fileStream = new FileStream(Path.Combine(productpath, fileName), FileMode.Create))
                    //    {
                    //        file.CopyTo(fileStream);
                    //    }

                    //    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }

                TempData["success"]= "Product created/updated successfully";
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


        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _unityOfWork.ProductImage.Get(u=>u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath =
                        Path.Combine(_webHostEnvironment.WebRootPath,
                        imageToBeDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _unityOfWork.ProductImage.Remove(imageToBeDeleted);
                _unityOfWork.Save();

                TempData["success"] = "Deleted successfully";
            }
            return RedirectToAction(nameof(Upsert), new {id = productId});
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unityOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unityOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            

            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (!Directory.Exists(finalPath))
            {

                //eliminiamo prima tutti i file presenti nella directory e poi eliminiamo la directory
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(finalPath);
            }

            _unityOfWork.Product.Remove(productToBeDeleted);
            _unityOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });

        }

        #endregion
    }
}