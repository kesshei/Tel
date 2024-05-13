using System.ComponentModel.DataAnnotations;

namespace Tel.Api.Models
{
    public class GetTokenRequest
    {
        [Required]
        public string name { get; set; }

        [Required]
        public string password { get; set; }
    }
}
