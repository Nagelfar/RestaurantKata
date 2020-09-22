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


namespace Customer.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly EventStore _events;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory factory, EventStore events)
        {
            _logger = logger;
            _httpClient = factory.CreateClient("GuestExperience");
            _events = events;
        }

        public Menu Menu { get; set; }

        public void OnGet()
        {
            Menu = _events.Project(default(Menu), (state, @event) => @event switch
            {
                MenuRetrieved menu => menu.Menu,
                _ => state
            });
        }


        public async Task<IActionResult> OnPostRetrieveMenuAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var menu = await _httpClient.GetFromJsonAsync<Menu>("menu");

            _events.Append(new MenuRetrieved { Menu = menu });

            return RedirectToPage("/Index");
        }
    }
}
