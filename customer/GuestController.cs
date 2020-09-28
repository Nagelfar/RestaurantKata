using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;

namespace Customer
{
    public class DeliveredItems
    {
        public int Delivery { get; set; }
        public int[] Food { get; set; }
        public int[] Drinks { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class GuestController : ControllerBase
    {
        private readonly EventStore eventStore;
        private readonly ImmutableDictionary<int, IImmutableSet<int>> _guestsWithOrders;

        public GuestController(EventStore eventStore)
        {
            this.eventStore = eventStore;
            _guestsWithOrders = eventStore.Project(ImmutableDictionary<int, IImmutableSet<int>>.Empty, (state, @event) =>
                 @event switch
                 {
                     OrderPlaced order =>
                         state.SetItem(
                             order.Guest,
                             state.GetValueOrDefault(order.Guest, ImmutableHashSet<int>.Empty).Add(order.Confirmation.Order)
                             ),
                     _ => state
                 }
            );
        }

        [HttpPost("{guest}/deliveries/{order}")]
        public IActionResult Post([FromRoute] int guest, [FromRoute] int order, [FromBody] DeliveredItems items)
        {
            if (_guestsWithOrders.TryGetValue(guest, out var orders))
            {
                if (orders.Contains(order))
                {
                    eventStore.Append(new DeliveryReceived(items.Delivery, guest, order, items));
                    return Ok();
                }
                else
                {
                    return BadRequest("Unknown order id");
                }
            }
            else
                return NotFound("The guest is not known");

        }
    }
}