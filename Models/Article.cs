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

        [StringLength(255, MinimumLength = 5, ErrorMessage = "{0} must between {2}-{1} characters")]
        [Required(ErrorMessage = "{0} is required")]
        [Column(TypeName = "nvarchar")]
        [DisplayName("Article's Title")]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "{0} is required")]
        [DisplayName("Date")]
        public DateTime Created { get; set; }

        [Column(TypeName = "ntext")]
        [DisplayName("Article's Content")]
        public string Content { get; set; }
    }
}