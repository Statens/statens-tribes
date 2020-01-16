using Statens.Tribes.App.Domain.Model;

namespace Statens.Tribes.App.Models
{
    public class CreateTribeModel
    {
        public string Name { get; set; }

        public TribeType TribeType { get; set; }
    }
}