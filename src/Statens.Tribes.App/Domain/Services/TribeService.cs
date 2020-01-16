using System.Collections.Generic;
using Statens.Tribes.App.Domain.Model;

namespace Statens.Tribes.App.Domain.Services
{
    public class TribeService
    {
        public Tribe CreateTribe(string id, string name, TribeType type)
        {
            return new Tribe
            {
                Id = id,
                Name = name,
                Type = type,
                Members = new List<TribeMember>()
            };
        }
    }
}