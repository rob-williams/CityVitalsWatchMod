namespace CityVitalsWatch {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;

    /// <summary>
    /// Represents the mod's settings, saved persistently through play sessions.
    /// </summary>
    public class CityVitalsWatchSettings {

        /// <summary>
        /// The positions of the panel and toggle button at different resolutions.
        /// </summary>
        public List<CityVitalsWatchResolution> Resolutions = new List<CityVitalsWatchResolution>();

        /// <summary>
        /// Indicates whether the panel should be visible by default.
        /// </summary>
        public bool DefaultPanelVisibility = true;

        /// <summary>
        /// Returns the <see cref="CityVitalsWatchResolution"/> instance corresponding to the specified screen width and height.
        /// </summary>
        /// <param name="screenWidth">The width of the screen.</param>
        /// <param name="screenHeight">The height of the screen.</param>
        /// <returns>
        /// The <see cref="CityVitalsWatchResolution"/> instance corresponding to the specified screen width and height.
        /// </returns>
        public CityVitalsWatchResolution GetResolutionData(int screenWidth, int screenHeight) {
            CityVitalsWatchResolution resolutionData = null;

            foreach (var resolution in this.Resolutions) {
                if (resolution.ScreenWidth == screenWidth && resolution.ScreenHeight == screenHeight) {
                    resolutionData = resolution;
                    break;
                }
            }

            if (resolutionData == null) {
                resolutionData = new CityVitalsWatchResolution();
                resolutionData.ScreenWidth = screenWidth;
                resolutionData.ScreenHeight = screenHeight;
                this.Resolutions.Add(resolutionData);
            }

            return resolutionData;
        }
    }
}
