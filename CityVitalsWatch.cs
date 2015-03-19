namespace CityVitalsWatch
{
    using ICities;

    public class CityVitalsWatch : IUserMod {

        public string Name {
            get { return "City Vitals Watch"; }
        }

        public string Description {
            get { return "Adds a panel to display vital city stats at a glance"; }
        }
    }
}