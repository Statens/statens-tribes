using System.Collections.Generic;
using System.Threading.Tasks;
using Statens.Tribes.App.Domain.Model;

namespace Statens.Tribes.App.Domain.Interfaces
{
    public interface ITribeRepository
    {
        Task<Tribe> SaveAsync(Tribe tribe);

        Task<List<Tribe>> ReadAllAsync();

        Task<Tribe> ReadAsync(string id);
    }
}