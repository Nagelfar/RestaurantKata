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
            public int Guest { get; internal set; }
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var billHttpResponse = await _billingClient.PostJsonAsync($"payment/{Command.Bill}", new BillPayment
            {
                Amount = Command.Amount,
                PaymentMethod = Command.PaymentMethod
            });
            billHttpResponse.EnsureSuccessStatusCode();
            var paid = await billHttpResponse.Content.ReadContentAsJson<PaidBill>();

            _events.Append(new BillPaid(Command.Guest, Command.Bill, Command.Amount, paid.PaidOrders));

            return RedirectToPage("./Orders/Detail", new { guest = Command.Guest });
        }

    }
}
