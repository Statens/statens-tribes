using System.Collections.Generic;
using System.Threading.Tasks;

namespace Statens.Tribes.App.Domain.Interfaces
{
    public interface IRepositoryOfType<T> where T: class
    {
        Task<T> SaveAsync(T entity);

        Task<List<T>> ReadAllAsync();

        Task<T> ReadAsync(string id);
    }
}