namespace CityVitalsWatch
{
    using ICities;

    // Path to Workshop folder: C:\Program Files (x86)\Steam\SteamApps\workshop\content\255710\410151616

    public class CityVitalsWatch : IUserMod {

        public string Name {
            get { return "City Vitals Watch"; }
        }

        public string Description {
            get { return "Adds a panel to display vital city stats at a glance"; }
        }
    }
}