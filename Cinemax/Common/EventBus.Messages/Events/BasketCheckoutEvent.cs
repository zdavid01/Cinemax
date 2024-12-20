using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Events
{
    public class ShoppingCartItem
    {
        public string MovieId { get; set; }
        public decimal Price { get; set; }
    }

    public class BasketCheckoutEvent : IntegrationBaseEvent
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