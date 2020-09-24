using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Api.GuestExperience.Model;
using Customer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using Api.TableService.Model;

namespace Customer.Pages
{

    public class OrderViewModel
    {
        public OrderViewModel(int guest, List<OrderPlaced.OrderItem> order, OrderConfirmation confirmation)
        {
            Guest = guest;
            Order = order;
            Confirmation = confirmation;
        }

        public int Guest { get; }
        public List<OrderPlaced.OrderItem> Order { get; }
        public OrderConfirmation Confirmation { get; }
    }

    public class OrdersModel : PageModel
    {
        private readonly ILogger<MenuModel> _logger;
        private readonly EventStore _events;

        public OrdersModel(ILogger<MenuModel> logger, IHttpClientFactory factory, EventStore events)
        {
            _logger = logger;
            _events = events;
        }

        public void OnGet()
        {
            Orders = _events.Project(new List<OrderViewModel>(), (state, @event) => @event switch
            {
                OrderPlaced order => state.AddToList(new OrderViewModel(order.Guest,order.Order, order.Confirmation)),
                _ => state
            });
        }

        public List<OrderViewModel> Orders { get; internal set; }

    }
}
