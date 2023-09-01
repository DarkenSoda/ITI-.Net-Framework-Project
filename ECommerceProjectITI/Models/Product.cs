using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceProjectITI.Models;

public class Product {
	public int Id { get; set; }

	[Display(Name = "Product Name")]
	[MaxLength(32, ErrorMessage = "Name Length Limit Reached!")]
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }

	[Range(10, 9999999, ErrorMessage = "Minimum Price 10 L.E., Maximum Price 9999999 L.E.")]
	public float Price { get; set; }

	[Display(Name = "Available Quantity")]
	[Range(1, 1000, ErrorMessage = "Available Quantity Should be from 1 to 1000.")]
	public int MaxQuantity { get; set; }

	[Required(ErrorMessage = "Image Field is Required!")]
	public string Image { get; set; } = string.Empty;

	[ForeignKey("Seller")]
	public int SellerID { get; set; }

	public virtual Seller? Seller { get; set; }
}
