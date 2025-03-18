using Course.DataAccess.Data;
using Course.Models;
using Course.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole>roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }
        public void Initialize()
        {
            //migrazioni se non sono ancora state applicate
            try
            {
                if(_db.Database.GetPendingMigrations().Count()>0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }

            //creazione dei ruoli
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Copany)).GetAwaiter().GetResult();

                //se non sono stati creati i ruoli creamo un utente admin

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admindef@gmail.com",
                    Email = "admindef@gmail.com",
                    Name = "Admin",
                    PhoneNumber = " 1234567890",
                    StreetAddress = "Via casamia 1",
                    State = "Ba",
                    PostalCode = "00100",
                    City = "Bari",
                }, "Admin.1").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admindef@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }
            return;
        }
    }
}
