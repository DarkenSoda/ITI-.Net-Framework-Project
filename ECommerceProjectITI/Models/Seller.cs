using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceProjectITI.Models;

public class Seller {
	public int Id { get; set; }

	[ForeignKey("User")]
	public int UserID { get; set; }

	public virtual User? User { get; set; }
	public virtual ICollection<Product>? ProductList { get; set; }
}