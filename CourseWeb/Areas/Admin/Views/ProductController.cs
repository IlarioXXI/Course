using Course.DataAccess.Repository.IRepository;
using Course.Models;
using Course.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace CourseWeb.Areas.Admin.Views
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unityOfWork;
        public ProductController(IUnitOfWork unityOfWork)
        {
            _unityOfWork = unityOfWork;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unityOfWork.Product.GetAll().ToList();
            
            return View(objProductList);
        }

        public IActionResult Create()
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

                return View(productVM);
        }
        [HttpPost]
        public IActionResult Create(ProductVM productVM)
        {

            if (ModelState.IsValid)
            {
                _unityOfWork.Product.Add(productVM.Product);
                _unityOfWork.Save();
                TempData["success"] = "Product created successfully";
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
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? objProduct = _unityOfWork.Product.Get(u => u.Id == id);
            //Category? objCategory1 = _db.Categories.FirstOrDefault(u=>u.Id == id);
            //Category? objCategory2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

            if (objProduct == null)
            {
                return NotFound();
            }
            return View(objProduct);
        }
        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unityOfWork.Product.Update(obj);
                _unityOfWork.Save();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? objProduct = _unityOfWork.Product.Get(u => u.Id == id);

            if (objProduct == null)
            {
                return NotFound();
            }
            return View(objProduct);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product? obj = _unityOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unityOfWork.Product.Remove(obj);
            _unityOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
