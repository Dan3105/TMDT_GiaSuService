namespace GiaSuService.Repository.Interface
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> All();
        IQueryable<T> FindAllBased(int id);
        Task<bool> Create(T entity);
        Task<bool> Update(T entity);

        Task<bool> SaveChanges();
    }
}
