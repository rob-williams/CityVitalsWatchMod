namespace CityVitalsWatch {

    using ICities;

    // Path to Workshop folder: C:\Program Files (x86)\Steam\SteamApps\workshop\content\255710\410151616

    /// <summary>
    /// Represents basic details about the mod.
    /// </summary>
    public class CityVitalsWatch : IUserMod {

        /// <summary>
        /// The mod's settings.
        /// </summary>
        public static CityVitalsWatchSettings Settings;

        /// <summary>
        /// The name of the mod.
        /// </summary>
        public string Name {
            get { return "City Vitals Watch"; }
        }

        /// <summary>
        /// The description of the mod.
        /// </summary>
        public string Description {
            get { return "Adds a configurable panel to display vital city stats at a glance."; }
        }
    }
}