
namespace DeveloperAssessment.DAL.Contracts
{
    /// <summary>
    /// Generic (polymorphic) abstraction
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : class, new()
    {
        Task<T> GetAsync();
        Task SaveAsync(T entity);
    }
}
