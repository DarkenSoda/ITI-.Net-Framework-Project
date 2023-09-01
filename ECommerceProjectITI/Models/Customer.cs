using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceProjectITI.Models;

public class Customer {	
	public int Id { get; set; }
	public string? Address { get; set; }

	[ForeignKey("User")]
	public int UserID { get; set; }
	public virtual User? User { get; set; }
}