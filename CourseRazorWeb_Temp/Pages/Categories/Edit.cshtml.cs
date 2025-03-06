using CourseRazorWeb_Temp.Data;
using CourseWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseRazorWeb_Temp.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public Category? Category { get; set; }

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Update(Category);
                _db.SaveChanges();
                TempData["Success"] = "Category updated successfully";
                return RedirectToPage("Index");
            }
            return Page();
        }
        public void OnGet(int? id)
        {
            if(id!=null && id!=0) 
            {
                Category = _db.Categories.Find(id);
            }
        }
    }
}
