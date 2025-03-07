﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Course.DataAccess.Repository.IRepository
{
    internal interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();

        //Il parametro d'ingresso della funzione Get sarebbe l'espressione LINQ presente nel FirstOrDefault :     _db.Categories.FirstOrDefault(u=>u.Id == id)
        T Get(Expression<Func<T,bool>>filter);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
