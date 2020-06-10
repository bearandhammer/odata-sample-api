using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using WtaTennisPlayers.Models;
using WtaTennisPlayers.Services.Implementations;
using WtaTennisPlayers.Shared;
using WtaTennisPlayers.Shared.AppSettings;

namespace WtaTennisPlayers.Services.Interfaces
{
    public class PlayerDataService : IPlayerDataService
    {
        /// <summary>
        /// This is the current live ranking WTA website address.
        /// </summary>
        private const string WTA_LIVE_RANKING_SITE = "https://live-tennis.eu/en/wta-live-ranking";

        /// <summary>
        /// A type for interacting with the cache.
        /// </summary>
        private readonly IMemoryCache cache;

        /// <summary>
        /// The cache timeout to apply to any
        /// caching operations.
        /// </summary>
        private readonly int cacheTimeout;

        /// <summary>
        /// Initialises a new instance of the service, as required (passing in dependencies).
        /// </summary>
        /// <param name="memCache">A type for interacting with the cache.</param>
        /// <param name="configProvider">The cache timeout to apply to any caching operations.</param>
        public PlayerDataService(IMemoryCache memCache, IConfiguration configProvider)
        {
            cache = memCache;
            cacheTimeout = configProvider.GetSection(nameof(CacheSettings)).Get<CacheSettings>().CacheTimeoutInMinutes;
        }

        /// <inheritdoc/>
        public IEnumerable<WtaPlayer> GetPlayersData()
        {
            // Attempt to retrieve WTA player data from the memory cache (or go and retrieve it, if necessary)
            if (!cache.TryGetValue(CacheKeys.PlayerData, out List<WtaPlayer> playerData))
            {
                // Initialise the WTA player data and store the results in the cache (with a stock expiry time)
                playerData = GetDataFromLiveRankingsPage();

                cache.Set(CacheKeys.PlayerData, playerData, new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(cacheTimeout)));
            }

            return playerData;
        }

        #region Data Store Private Static Methods

        #region Data Store Public Methods

        /// <summary>
        /// Uses the types found within the HtmlAgilityPack NuGet package to retrieve the
        /// top 100 WTA tennis players from the live-tennis.eu website (a small touch of HTML scraping in place).
        /// </summary>
        /// <returns>A List of <see cref="WtaPlayer"/> types, to be further interrogated based on the users query.</returns>
        private static List<WtaPlayer> GetDataFromLiveRankingsPage()
        {
            List<WtaPlayer> liveRankingPlayers = new List<WtaPlayer>();

            try
            {
                // Retrieve a HtmlDocument type that contains the live ranking table data and create WTA player objects from this
                HtmlDocument wtaPlayerHtmlDocument = GetLiveRankingsWebDocument();

                if (wtaPlayerHtmlDocument != null)
                {
                    liveRankingPlayers.AddRange(GetPlayersFromHtmlDocument(wtaPlayerHtmlDocument));
                }
            }
            catch (Exception ex)
            {
                // Add appropriately logging, on error, as required...
                Debug.WriteLine(ex.Message);
            }

            return liveRankingPlayers;
        }

        #endregion Data Store Public Methods

        /// <summary>
        /// Simple static helper method that gets the HtmlDocument with WTA live ranking HTML.
        /// </summary>
        /// <returns></returns>
        private static HtmlDocument GetLiveRankingsWebDocument() => new HtmlWeb().Load(WTA_LIVE_RANKING_SITE);

        /// <summary>
        /// Static helper method that tears apart the provided HtmlDocument to construct
        /// a collection of WtaPlayer objects.
        /// </summary>
        /// <param name="wtaPlayerHtmlDocument">The HtmlDocument that contains the WTA player data.</param>
        /// <returns>An IEnumerable of type <see cref="WtaPlayer"/>.</returns>
        private static IEnumerable<WtaPlayer> GetPlayersFromHtmlDocument(HtmlDocument wtaPlayerHtmlDocument)
        {
            // Work with the returned HTML data - first step is to identify the table rows (only a single table on the page at
            // the time of producing this code sample). We only want every other 'tr', as these are the only ones that contain data, also
            // keeping in mind that spacer 'tr' elements exist (without a 'td' Count of 14, so also remove these elements)
            HtmlNodeCollection tbodyRowNodes = wtaPlayerHtmlDocument.DocumentNode.SelectNodes("//tbody/tr");

            IEnumerable<HtmlNode> everyOtherNode = tbodyRowNodes
                .Where((node, index) => index % 2 == 0 && node?.ChildNodes?.Count == 14)
                .Take(100);

            // Setup a regex to clean up the rank information
            Regex rankCleanerRegex = new Regex(@"<[^>]+>|&nbsp;");

            // Construct and return WtaPlayer objects based on some very (fixed, agreed!) ripping of text from td elements
            return everyOtherNode.Select(node =>
                 new WtaPlayer(int.Parse(rankCleanerRegex.Replace(node.ChildNodes[0].InnerText, string.Empty).Trim()),
                    node.ChildNodes[3].InnerText.Trim(),
                    int.Parse(node.ChildNodes[6].InnerText.Trim())));
        }

        #endregion Data Store Private Static Methods
    }
}
