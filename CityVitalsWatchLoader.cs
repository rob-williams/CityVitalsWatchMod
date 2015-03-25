namespace CityVitalsWatch {

    using ColossalFramework.UI;
    using ICities;
    using UnityEngine;

    /// <summary>
    /// Loads the City Vitals Watch panel when a game is loaded or started.
    /// </summary>
    public class CityVitalsWatchLoader : ILoadingExtension {

        private static CityVitalsWatchPanel panel = null;

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
            // Create object and attach panel component if a game is being loaded or started
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame) {
                UIView uiViewParent = null;

                foreach (var uiView in GameObject.FindObjectsOfType<UIView>()) {
                    if (uiView.name == "UIView") {
                        uiViewParent = uiView;
                        break;
                    }
                }

                if (uiViewParent != null) {
                    GameObject obj = new GameObject("CityVitalsWatch");
                    obj.transform.parent = uiViewParent.cachedTransform;
                    CityVitalsWatchLoader.panel = obj.AddComponent<CityVitalsWatchPanel>();
                }
            }
        }

        /// <summary>
        /// Called when a level is unloaded.
        /// </summary>
        public void OnLevelUnloading() {
            if (CityVitalsWatchLoader.panel != null) {
                GameObject.Destroy(CityVitalsWatchLoader.panel.gameObject);
                CityVitalsWatchLoader.panel = null;
            }
        }

        /// <summary>
        /// Called when the mod is released. Does nothing.
        /// </summary>
        public void OnReleased() {
        }
    }
}
