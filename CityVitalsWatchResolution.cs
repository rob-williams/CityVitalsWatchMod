namespace CityVitalsWatch {

    /// <summary>
    /// Represents the position of the panel and toggle button at a specific resolution.
    /// </summary>
    public class CityVitalsWatchResolution {

        /// <summary>
        /// The width of the screen.
        /// </summary>
        public int ScreenWidth;

        /// <summary>
        /// The height of the screen.
        /// </summary>
        public int ScreenHeight;

        /// <summary>
        /// The relative x-position of the panel.
        /// </summary>
        public float PanelPositionX = 10f;

        /// <summary>
        /// The relative y-position of the panel.
        /// </summary>
        public float PanelPositionY = 65f;

        /// <summary>
        /// The absolute x-position of the toggle button.
        /// </summary>
        public float ToggleButtonPositionX = 125f;

        /// <summary>
        /// The absolute y-position of the toggle button.
        /// </summary>
        public float ToggleButtonPositionY = 12f;
    }
}
