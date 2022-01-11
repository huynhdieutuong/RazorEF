using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace RazorEF.Models
{
    // 2. Create AppUser Model has Users's Columns: UserName, Email, PasswordHash,...
    public class AppUser : IdentityUser
    {
        // Had some default properties in IdentityUser
        // Can add more properties in here
        // 6. Add new Column for Users Table
        [Column(TypeName = "nvarchar")]
        [StringLength(400)]
        public string HomeAddress { get; set; }

        // 14.1 Add BirthDate for Users table, then migrations to update database
        // DateTime have value by default. Want BirthDate can be NULL: Not [Required] + DateTime?
        // [Required]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
    }
}