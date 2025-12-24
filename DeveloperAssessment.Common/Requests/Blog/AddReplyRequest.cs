using System.ComponentModel.DataAnnotations;

namespace DeveloperAssessment.Common.Requests.Blog
{
    public class AddReplyRequest
    {
        [Required, StringLength(80)]
        public string Name { get; set; } = "";

        [Required, EmailAddress, StringLength(200)]
        public string EmailAddress { get; set; } = "";

        [Required, StringLength(2000)]
        public string Message { get; set; } = "";
    }
}
