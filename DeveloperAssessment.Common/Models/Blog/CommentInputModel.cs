using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DeveloperAssessment.Common.Models.Blog
{
    public class CommentInputModel
    {
        // Exercise 3 - required fields
        [Required, StringLength(80)]
        public string Name { get; set; } = "";

        [Required, EmailAddress, StringLength(200)]
        public string EmailAddress { get; set; } = "";

        [Required, StringLength(2000)]
        public string Message { get; set; } = "";

        // Exercise 5
        public List<IFormFile> Files { get; set; } = new();
    }
}
