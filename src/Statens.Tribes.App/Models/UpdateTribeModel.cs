using System.ComponentModel.DataAnnotations;
using Statens.Tribes.App.Domain.Model;

namespace Statens.Tribes.App.Models
{
    public class UpdateTribeModel
    {
        [Required]
        [StringLength(5)]
        public string Name { get; set; }

        [Required]
        public TribeType TribeType { get; set; }
    }
}