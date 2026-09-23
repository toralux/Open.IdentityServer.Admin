using System.ComponentModel.DataAnnotations;

namespace Toralux.Open.IdentityServer.STS.Identity.ViewModels.Account
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
