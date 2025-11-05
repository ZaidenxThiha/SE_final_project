using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AWEfinal.DAL.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Storage { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Colors { get; set; } = "[]"; // JSON array as string

        [Column(TypeName = "nvarchar(max)")]
        public string? Sizes { get; set; } // JSON array as string

        [Required]
        [Column(TypeName = "decimal(3,2)")]
        public decimal Rating { get; set; } = 0;

        [Column(TypeName = "nvarchar(max)")]
        public string Images { get; set; } = "[]"; // JSON array as string

        [Column(TypeName = "nvarchar(max)")]
        public string Features { get; set; } = "[]"; // JSON array as string

        [Required]
        public bool InStock { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

