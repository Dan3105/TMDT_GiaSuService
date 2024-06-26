﻿namespace GiaSuService.Repository.Interface
{
    public interface IRepository<T> where T : class
    {
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<bool> SaveChanges();
    }
}
