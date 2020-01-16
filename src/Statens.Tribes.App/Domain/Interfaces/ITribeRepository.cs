using System.Collections.Generic;
using Statens.Tribes.App.Domain.Model;

namespace Statens.Tribes.App.Domain.Interfaces
{
    public interface ITribeRepository
    {
        Tribe Save(Tribe tribe);

        List<Tribe> ReadAll();

        Tribe Read(string id);
    }
}