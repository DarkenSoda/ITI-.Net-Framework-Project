using Microsoft.EntityFrameworkCore;

namespace ECommerceProjectITI.Models;

public class CartItem {
	public virtual Product? Product { get; set; }
	public int ShoppingCartID { get; set; }
	public int Quantity { get; set; } = 1;
}