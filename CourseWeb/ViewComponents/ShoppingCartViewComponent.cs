using Course.DataAccess.Repository.IRepository;
using Course.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourseWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //metodo async per tener conto degli oggetti nel carrello
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if(HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
                }
                
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }else
            {
                HttpContext.Session.Clear();
                return View(0);
            }

        }
    }
}
