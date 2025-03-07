using Course.DataAccess.Data;
using Course.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Course.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }
        public void Add(T entity)
        {
            dbSet.Add(entity); 
        }

        public T Get(Expression<Func<T, bool>> filter)
        {   
            /*
             * Inizialmente assegamo a una variabile query(di tipo IQueryable) il dbSet(in base all'entità che stiamo usando), poi assegnamo la variabile query alla condizione
             * passata dal metodo get e nel return ci ritorna l'oggetto. Questo procedimento equivale a scrivere : _db.Categories.Where(u => u.Id == id).FirstOrDefault();
             * dove prima assegnamo il db di riferimento poi troviamo l'elemento e poi ce lo ritorna.
             */
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll()
        {
            //Assegnamo il db di riferimento
            IQueryable<T> query = dbSet;
            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
