using AlamandaApi.Services.Comics;
using AlamandaApi.Services.User;

namespace AlamandaApi.Services.Cart {
  public class CartModel {
    public int Id { get; set; }

    public int UserId { get; set; }
    public UserModel User { get; set; } = null!;

    public ICollection<CartItemModel> Items { get; set; } = new List<CartItemModel>();
  }

  public class CartItemModel {
      public int Id { get; set; }
      public int CartId { get; set; }
      public CartModel Cart { get; set; } = null!;
      public int ComicId { get; set; }
      public ComicModel Comic { get; set; } = null!;
      public int Quantity { get; set; }
  }
}
