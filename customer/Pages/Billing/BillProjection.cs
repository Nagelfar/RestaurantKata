using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Api.Billing.Model;

namespace Customer.Pages.Billing
{
    public class BillViewModel
    {
        public int Guest { get; internal set; }
        public int BilledItems { get; internal set; }
        public decimal TotalSum { get; internal set; }
        public DateTime BillFrom { get; internal set; }
        public int Bill { get; set; }
        public decimal? PaidAmount { get; private set; }
        public IEnumerable<PaidOrder> PaidOrders { get; private set; }
        public DateTime? PaidOn { get; private set; }

        public static Projector<BillViewModel> OpenBillFor(int guest)
        {
            return (BillViewModel state, Event @event) =>
                @event switch
                {
                    BillReceived b when b.Guest == guest =>
                        new BillViewModel
                        {
                            Bill = b.Bill,
                            Guest = guest,
                            BilledItems = b.OrderedFood.Concat(b.OrderedDrinks).Count(),
                            TotalSum = b.TotalSum,
                            BillFrom = b.On
                        },
                    BillUpdated b when b.Guest == guest =>
                        new BillViewModel
                        {
                            Bill = b.Bill,
                            Guest = guest,
                            BilledItems = b.OrderedFood.Concat(b.OrderedDrinks).Count(),
                            TotalSum = b.TotalSum,
                            BillFrom = b.On
                        },
                    BillPaid b when b.Guest == guest && b.Bill == state?.Bill =>
                        null,
                    _ => state
                };
        }

        public BillViewModel MarkAsPaid(decimal paidAmount, IEnumerable<PaidOrder> paidOrders, DateTime paidOn)
        {
            return new BillViewModel
            {
                Guest = Guest,
                BilledItems = BilledItems,
                TotalSum = TotalSum,
                BillFrom = BillFrom,
                Bill = Bill,
                PaidAmount = paidAmount,
                PaidOrders = paidOrders,
                PaidOn = paidOn
            };
        }

        public static Projector<ImmutableDictionary<int, BillViewModel>> BillsFor(int guest)
        {
            return (ImmutableDictionary<int, BillViewModel> state, Event @event) =>
                @event switch
                {
                    BillReceived b when b.Guest == guest =>
                        state.Add(b.Bill,
                            new BillViewModel
                            {
                                Bill = b.Bill,
                                Guest = guest,
                                BilledItems = b.OrderedFood.Concat(b.OrderedDrinks).Count(),
                                TotalSum = b.TotalSum,
                                BillFrom = b.On
                            }),
                    BillUpdated b when b.Guest == guest =>
                        state.SetItem(b.Bill, new BillViewModel
                        {
                            Bill = b.Bill,
                            Guest = guest,
                            BilledItems = b.OrderedFood.Concat(b.OrderedDrinks).Count(),
                            TotalSum = b.TotalSum,
                            BillFrom = b.On
                        }),
                    BillPaid b when b.Guest == guest =>
                        state.SetItem(b.Bill,
                            state[b.Bill].MarkAsPaid(b.Amount, b.PaidOrders, b.On)
                        ),
                    _ => state
                };
        }
    }
}