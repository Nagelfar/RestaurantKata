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

    public class IndexModel : PageModel
    {
        private readonly ILogger<MenuModel> _logger;
        private readonly HttpClient _guestExperienceClient;
        private readonly EventStore _events;

        public bool RetrievedMenu { get; }

        public IndexModel(ILogger<MenuModel> logger, IHttpClientFactory factory, EventStore events)
        {
            _logger = logger;
            _guestExperienceClient = factory.CreateClient("GuestExperience");
            _events = events;

            RetrievedMenu = _events.Project(false, (state, @event) => @event switch
            {
                MenuRetrieved _ => true,
                _ => state
            });
        }
        
        public async Task<IActionResult> OnPostRetrieveMenuAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var menu = await _guestExperienceClient.GetFromJsonAsync<Menu>("menu");

            _events.Append(new MenuRetrieved { Menu = menu });

            return RedirectToPage("/Menu");
        }

    }
}
