using Microsoft.AspNetCore.Identity;

namespace ECommerceProjectITI.Models;

public class User : IdentityUser<int> {
	public string Name { get; set; } = string.Empty;
	public string? Image { get;set; }
}
