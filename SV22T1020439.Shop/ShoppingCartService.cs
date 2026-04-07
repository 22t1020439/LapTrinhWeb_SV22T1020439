using SV22T1020439.Models;
using System.Collections.Generic;

namespace SV22T1020439.Shop
{
    public static class ShoppingCartService
    {
        private const string CART = "ShopCart";

        public static List<CartItem> GetShoppingCart()
        {
            var cart = ApplicationContext.GetSessionData<List<CartItem>>(CART);
            if (cart == null)
            {
                cart = new List<CartItem>();
                ApplicationContext.SetSessionData(CART, cart);
            }    
            return cart;
        }

        public static void AddCartItem(CartItem item)
        {
            var cart = GetShoppingCart();
            var existsItem = cart.Find(m => m.ProductID == item.ProductID);
            if (existsItem == null)
                cart.Add(item);
            else
                existsItem.Quantity += item.Quantity;
            ApplicationContext.SetSessionData(CART, cart);   
        }

        public static void UpdateCartItem(int productID, int quantity)
        {
            var cart = GetShoppingCart();
            var item = cart.Find(m => m.ProductID == productID);
            if (item != null)
            {
                item.Quantity = quantity;
                ApplicationContext.SetSessionData(CART, cart);
            }    
        }

        public static void RemoveCartItem(int productID)
        {
            var cart = GetShoppingCart();
            int index = cart.FindIndex(m => m.ProductID == productID);
            if (index >= 0)
            {
                cart.RemoveAt(index);
                ApplicationContext.SetSessionData(CART, cart);
            }    
        }

        public static void ClearCart()
        {
            ApplicationContext.SetSessionData(CART, new List<CartItem>());
        }
    }
}
