using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.GuestExperience.Model;
using Api.TableService.Model;

namespace Customer
{
    public abstract class Event
    {
        DateTime On { get; set; } = DateTime.Now;
    }

    public class MenuRetrieved : Event
    {
        public Menu Menu { get; set; }
    }

    public class OrderPlaced : Event
    {
        public class OrderItem
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public List<string> Nutrition { get; set; }

            public decimal Price { get; set; }
        }

        public int Guest {get;set;}
        public OrderConfirmation Confirmation { get; set; }
        public List<OrderItem> Order { get; set; }
    }
}