using System;
using System.Collections.Generic;
using System.Linq;
using Statens.Tribes.App.Domain.Model;

namespace Statens.Tribes.App.Domain.Interfaces
{
    public interface ITribeRepository
    {
        Tribe Save(Tribe tribe);

        List<Tribe> ReadAll();

        Tribe Read(string id);
    }

    public class TribeRepository : ITribeRepository
    {
        private static readonly List<Tribe> tribes = new List<Tribe>()
        {
            new Tribe
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Tribe 1"
            }
        };

        public Tribe Save(Tribe tribe)
        {
            tribes.Add(tribe);
            return tribe;
        }

        public List<Tribe> ReadAll()
        {
            return tribes;
        }

        public Tribe Read(string id)
        {
            return ReadAll().FirstOrDefault(x => x.Id == id);
        }
    }
}