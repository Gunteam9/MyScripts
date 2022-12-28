namespace Common.Entity
{
    /// <summary>
    ///     Informations sur le joueur
    /// </summary>
    public class PlayerInfo
    {
        public PlayerInfo()
        {
        }

        public PlayerInfo(long id, string steamId)
        {
            Id = id;
            SteamId = steamId;
        }

        /// <summary>
        ///     ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     Identifiant steam
        /// </summary>
        public string SteamId { get; set; }
    }
}