﻿namespace Basket.API.Entities
{
    public class BasketCheckout
    {
        // Address
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string EmailAddress { get; set; }

        // Order
        public string BuyerId { get; set; }
        public string BuyerUsername { get; set; }
        public IEnumerable<ShoppingCartItem> OrderItems { get; set; }
    }
}