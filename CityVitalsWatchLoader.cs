namespace CityVitalsWatch {

    using ColossalFramework.UI;
    using ICities;
    using UnityEngine;

    /// <summary>
    /// Creates the City Vitals Watch panel when a game is loaded or started.
    /// </summary>
    public class CityVitalsWatchLoader : ILoadingExtension {

        private static CityVitalsWatchPanel Panel = null;

        /// <summary>
        /// Loads the mod's settings and creates the City Vitals Watch panel.
        /// </summary>
        public static void CreatePanel() {
            CityVitalsWatch.Settings = CityVitalsWatchSerializer.LoadSettings();

            UIView uiViewParent = null;

            foreach (var uiView in GameObject.FindObjectsOfType<UIView>()) {
                if (uiView.name == "UIView") {
                    uiViewParent = uiView;
                    break;
                }
            }

            if (uiViewParent != null) {
                GameObject obj = new GameObject("CityVitalsWatchPanel");
                obj.transform.parent = uiViewParent.cachedTransform;
                Panel = obj.AddComponent<CityVitalsWatchPanel>();
            }
        }

        /// <summary>
        /// Saves the mod controls' positions to settings and destroys the City Vitals Watch panel.
        /// </summary>
        public static void DestroyPanel() {
            var resolutionData = CityVitalsWatch.Settings.GetResolutionData(Screen.currentResolution.width, Screen.currentResolution.height);
            resolutionData.PanelPositionX = Panel.relativePosition.x;
            resolutionData.PanelPositionY = Panel.relativePosition.y;
            resolutionData.ToggleButtonPositionX = Panel.ToggleButton.absolutePosition.x;
            resolutionData.ToggleButtonPositionY = Panel.ToggleButton.absolutePosition.y;

            CityVitalsWatchSerializer.SaveSettings(CityVitalsWatch.Settings);

            Panel.gameObject.SetActive(false);
            GameObject.Destroy(Panel.gameObject);
            Panel = null;
        }

        /// <summary>
        /// Called when the mod is created; does nothing.
        /// </summary>
        /// <param name="loading">Ignored parameter.</param>
        public void OnCreated(ILoading loading) {
        }

        /// <summary>
        /// Called when a level is loaded.
        /// </summary>
        /// <param name="mode">The type of loading operation being performed.</param>
        public void OnLevelLoaded(LoadMode mode) {
            // If a game is being loaded or started, load the settings and then create an object and attach the panel component
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame) {
                CreatePanel();
            }
        }

        /// <summary>
        /// Called when a level is unloaded.
        /// </summary>
        public void OnLevelUnloading() {
            if (Panel != null) {
                DestroyPanel();
            }
        }

        /// <summary>
        /// Called when the mod is released; does nothing.
        /// </summary>
        public void OnReleased() {
        }
    }
}