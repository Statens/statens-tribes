using System.Collections.Generic;

namespace Statens.Tribes.App.Domain.Model
{
    public class Tribe
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public TribeType Type { get; set; }
        public List<TribeMember> Members { get; set; }
    }
}