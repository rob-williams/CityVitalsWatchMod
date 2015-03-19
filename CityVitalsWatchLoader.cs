namespace CityVitalsWatch {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ICities;
    using UnityEngine;
    using ColossalFramework.UI;
    public class CityVitalsWatchLoader : ILoadingExtension {

        private static CityVitalsWatchPanel panel = null;

        public void OnCreated(ILoading loading) {
        }

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

        public void OnLevelUnloading() {
            if (CityVitalsWatchLoader.panel != null) {
                GameObject.Destroy(CityVitalsWatchLoader.panel.gameObject);
                CityVitalsWatchLoader.panel = null;
            }
        }

        public void OnReleased() {
        }
    }
}
