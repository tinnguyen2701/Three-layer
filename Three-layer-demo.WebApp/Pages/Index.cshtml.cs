using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Three_layer_demo.Domain.Entities;
using Three_layer_demo.Domain.Interfaces.Services;

namespace Three_layer_demo.WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IActorService _actorService;

        public IList<Actor> Actors { get; set; }


        public IndexModel(ILogger<IndexModel> logger, IActorService actorService)
        {
            _logger = logger;
            _actorService = actorService;
        }

        public async Task OnGet()
        {
            Actors = await _actorService.GetAll();
        }
    }
}
