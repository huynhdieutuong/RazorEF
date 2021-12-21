using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorEF.Models
{
    // [Table("posts")]
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        [Required]
        [Column(TypeName = "nvarchar")]
        [DisplayName("Article's Title")]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [Required]
        [DisplayName("Date")]
        public DateTime Created { get; set; }

        [Column(TypeName = "ntext")]
        [DisplayName("Article's Content")]
        public string Content { get; set; }
    }
}