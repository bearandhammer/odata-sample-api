namespace WtaTennisPlayers.Models
{
    /// <summary>
    /// Class that represents a WTA tennis player (basic information only).
    /// </summary>
    public class WtaPlayer
    {
        /// <summary>
        /// The players tour rank.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// The players name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The current WTA player points the player has (not next/max points).
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Entry point constructor for creating a new WtaPlayer object.
        /// </summary>
        /// <param name="playerRank">Initialises the player with a rank.</param>
        /// <param name="playerName">Initialises the player with a name.</param>
        /// <param name="playerPoints">Initialises the player with a points value.</param>
        public WtaPlayer(int playerRank, string playerName, int playerPoints)
        {
            Rank = playerRank;
            Name = playerName;
            Points = playerPoints;
        }
    }
}
