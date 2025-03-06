using CourseRazorWeb_Temp.Data;
using CourseWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseRazorWeb_Temp.Pages.Categories
{
    public class DeleteModel : PageModel
    {

        private readonly ApplicationDbContext _db;

        [BindProperty]
        public Category? Category { get; set; }

        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult OnPost()
        {   
            Category? objCategory = _db.Categories.Find(Category.Id);
            if (objCategory == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(objCategory);
            _db.SaveChanges();
            return RedirectToPage("Index");
        }
        public void OnGet(int? id)
        {
            if (id != null && id != 0)
            {
                Category = _db.Categories.Find(id);
            }
        }
    }
}
