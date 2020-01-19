using System.ComponentModel.DataAnnotations;
using Statens.Tribes.App.Domain.Model;

namespace Statens.Tribes.App.Models
{
    public class CreateTribeModel
    {
        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [Required]
        public TribeType TribeType { get; set; }
    }
}