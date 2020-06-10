using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WtaTennisPlayers.Models;
using WtaTennisPlayers.Services.Implementations;

namespace WtaTennisPlayers.Api.Controllers
{
    /// <summary>
    /// Represents the controller supporting the retrieval of WTA Player data.
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WtaPlayersController : Controller
    {
        /// <summary>
        /// The core API endpoint that illustrates the functionality of OData through the
        /// use of the EnableQuery() attribute.
        /// </summary>
        /// <returns>WTA player data (stock top 100 players) which is then further manipulated based on the callers query.</returns>
        [HttpGet]
        [EnableQuery()]
        public IEnumerable<WtaPlayer> GetPlayers([FromServices] IPlayerDataService playerDataService) => playerDataService.GetPlayersData();
    }
}
