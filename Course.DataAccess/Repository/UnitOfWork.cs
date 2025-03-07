using Course.DataAccess.Data;
using Course.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            //nel costruttore di category repository  bisogna mettere un oggetto di tipo ApplicationDbContext
            Category = new CategoryRepository(_db);
        }
        


        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
