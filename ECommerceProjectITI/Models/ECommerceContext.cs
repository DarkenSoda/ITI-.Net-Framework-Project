using ECommerceProjectITI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceProjectITI.Models;

public class ECommerceContext : IdentityDbContext<User, IdentityRole<int>, int> {
	public ECommerceContext(DbContextOptions options) : base(options) { }

	public virtual DbSet<Seller> Seller { get; set; }
	public virtual DbSet<Product> Product { get; set; }
	public virtual DbSet<Customer> Customer { get; set; }

	protected override void OnModelCreating(ModelBuilder builder) {
		base.OnModelCreating(builder);

		builder.Entity<User>().ToTable("User");

		builder.Entity<IdentityRole<int>>()
		   .ToTable("Role")
		   .HasData(
			   new IdentityRole<int> { Id = -1, Name = "Seller", NormalizedName = "SELLER", ConcurrencyStamp = "SELLER" },
			   new IdentityRole<int> { Id = -2, Name = "Customer", NormalizedName = "CUSTOMER", ConcurrencyStamp = "CUSTOMER" }
		   );


		builder.Entity<IdentityUserRole<int>>().ToTable("UserRole")
			.HasKey(r => new { r.UserId, r.RoleId });

		builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaim");
		builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaim");
		builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogin");
		builder.Entity<IdentityUserToken<int>>().ToTable("UserToken");
	}
}
