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
using System.Collections.Immutable;
using Api.Billing.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Customer.Pages.Billing
{
    public class PaymentModel : PageModel
    {
        public class PaymentCommand
        {
            public int Bill { get; set; }
            public decimal Amount { get; set; }
            public PaymentMethod PaymentMethod { get; set; }
            public int Guest { get; set; }
        }

        private readonly ILogger<PaymentModel> _logger;
        private readonly HttpClient _billingClient;
        private readonly EventStore _events;

        public BillViewModel Bill { get; private set; }
        public IEnumerable<SelectListItem> SupportedPaymentMethods { get; private set; }

        [BindProperty]
        public PaymentCommand Command { get; set; }

        public PaymentModel(ILogger<PaymentModel> logger, IHttpClientFactory factory, EventStore events)
        {
            _logger = logger;
            _billingClient = factory.CreateClient("Billing");
            _events = events;
        }

        public async Task OnGetAsync(int guest)
        {
            Bill = _events
                .Project(
                    default(BillViewModel),
                    BillViewModel.OpenBillFor(guest)
                );
            var paymentMethods = await _billingClient.GetFromJsonAsync<PaymentMethod[]>($"payment/{Bill.Bill}");

            _events.Append(new PaymentMethodsSupported(guest, Bill.Bill, paymentMethods));

            SupportedPaymentMethods = paymentMethods
                .Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() })
                .ToList();

            Command = new PaymentCommand
            {
                Guest = guest,
                Bill = Bill.Bill
            };

        }

        private void AssertOrFail(bool condition, string reason)
        {
            if (!condition)
                throw new ArgumentException("Condition failed: " + reason);
        }

        private void EnsureContract(PaidBill bill)
        {
            AssertOrFail(bill.Bill >= 0, "Billing ID");
            AssertOrFail(bill.PaidOrders != null, "Expected a value in paid orders");
            foreach (var order in bill.PaidOrders)
            {
                AssertOrFail(order.Order >= 0, "An order id should be set");
                AssertOrFail(
                    order.PaidDrinks != null && order.PaidDrinks.Any() || order.PaidFood != null && order.PaidFood.Any(),
                    "Expected either paid drinks or paid food as part of the order"
                    );
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var billHttpResponse = await _billingClient.PostJsonAsync($"payment/{Command.Bill}", new BillPayment
            {
                Amount = Command.Amount,
                PaymentMethod = Command.PaymentMethod
            });
            if (!billHttpResponse.IsSuccessStatusCode)
            {
                var content = await billHttpResponse.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Expected a success status code but got {billHttpResponse.StatusCode} instead with body:\n\n{content}",
                    inner: null,
                    statusCode: billHttpResponse.StatusCode
                );
            }

            var paid = await billHttpResponse.Content.ReadContentAsJson<PaidBill>();

            EnsureContract(paid);

            _events.Append(new BillPaid(Command.Guest, Command.Bill, Command.Amount, paid.PaidOrders));

            return RedirectToPage("/Orders/Detail", new { guest = Command.Guest });
        }

    }
}
