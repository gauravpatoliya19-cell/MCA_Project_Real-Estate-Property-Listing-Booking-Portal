using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstatePortal.Web.Models
{
    public class PropertyFeature
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public virtual Property? Property { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Feature Name")]
        public string FeatureName { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Icon CSS Class")]
        public string? IconClass { get; set; } = "bi-check-circle-fill";
    }
}
