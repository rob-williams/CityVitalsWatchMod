namespace CityVitalsWatch {

    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Provides methods to load and save <see cref="CityVitalsWatchSettings"/> instances using XML serialization.
    /// </summary>
    public static class CityVitalsWatchSerializer {

        /// <summary>
        /// The name of the XML file.
        /// </summary>
        private static readonly string SettingsFileName = "CityVitalsWatchSettings.xml";

        /// <summary>
        /// Loads the settings from the XML file.
        /// </summary>
        /// <returns>The loaded settings.</returns>
        public static CityVitalsWatchSettings LoadSettings() {
            CityVitalsWatchSettings settings = null;
            FileStream stream = null;

            try {
                XmlSerializer serializer = new XmlSerializer(typeof(CityVitalsWatchSettings));
                stream = new FileStream(SettingsFileName, FileMode.Open);
                settings = (CityVitalsWatchSettings)serializer.Deserialize(stream);
            }
            catch {
                settings = new CityVitalsWatchSettings();
            }
            finally {
                if (stream != null) {
                    stream.Close();
                }
            }

            return settings;
        }

        /// <summary>
        /// Saves the provided settings to the XML file.
        /// </summary>
        /// <param name="settings">The settings to save.</param>
        public static void SaveSettings(CityVitalsWatchSettings settings) {
            StreamWriter stream = null;

            try {
                XmlSerializer serializer = new XmlSerializer(typeof(CityVitalsWatchSettings));
                stream = new StreamWriter(SettingsFileName);
                serializer.Serialize(stream, settings);
            }
            catch {
                // Do nothing on exception
            }
            finally {
                if (stream != null) {
                    stream.Close();
                }
            }
        }
    }
}
