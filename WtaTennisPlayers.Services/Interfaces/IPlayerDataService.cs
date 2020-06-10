using System.Collections.Generic;
using WtaTennisPlayers.Models;

namespace WtaTennisPlayers.Services.Implementations
{
    /// <summary>
    /// Interface for a player data service.
    /// </summary>
    public interface IPlayerDataService
    {
        /// <summary>
        /// Public entry point for retrieving WTA player information.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{WtaPlayer}"/> types.</returns>
        IEnumerable<WtaPlayer> GetPlayersData();
    }
}
