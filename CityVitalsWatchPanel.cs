namespace CityVitalsWatch {

    using System;
    using System.Globalization;
    using ColossalFramework;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// The City Vitals Watch panel component, responsible for creating and updating all visible controls.
    /// </summary>
    public class CityVitalsWatchPanel : UIPanel {

        private static readonly float PanelWidth = 275f;
        private static readonly float TitleBarHeight = 40f;

        private bool previousContainsMouse = true;
        private UIView uiParent;
        private UIButton toggleButton;
        private UIPanel infoPanel;

        private UISlider electricityMeter;
        private UISlider waterMeter;
        private UISlider sewageMeter;
        private UISlider landfillMeter;
        private UISlider incineratorMeter;
        private UISlider cemeteryMeter;
        private UISlider crematoriumMeter;
        private UISlider employmentMeter;

        public UIButton ToggleButton {
            get { return this.toggleButton; }
        }

        /// <summary>
        /// Called before the first frame after the panel is created.
        /// </summary>
        public override void Start() {
            foreach (var uiView in GameObject.FindObjectsOfType<UIView>()) {
                if (uiView.name == "UIView") {
                    this.uiParent = uiView;
                    this.transform.parent = this.uiParent.transform;

                    var resolutionData = CityVitalsWatch.Settings.GetResolutionData(Screen.currentResolution.width, Screen.currentResolution.height);
                    this.relativePosition = new Vector3(resolutionData.PanelPositionX, resolutionData.PanelPositionY);

                    break;
                }
            }

            base.Start();

            this.backgroundSprite = "MenuPanel";
            this.isVisible = CityVitalsWatch.Settings.DefaultPanelVisibility;
            this.canFocus = true;
            this.isInteractive = true;
            this.width = PanelWidth;
            this.height = TitleBarHeight + 5f;

            try {
                SetUpControls();
            }
            catch (Exception e) {
                // If for some reason control setup threw an exception, destroy the panel instead of staying broken
                GameObject.Destroy(this.gameObject);

                // Rethrow the exception to help debug any issues
                throw e;
            }
        }

        /// <summary>
        /// Called every frame the panel is active.
        /// </summary>
        public override void Update() {
            // The base Update must be called to lay out controls
            base.Update();

            // Toggle the panel's visibility when Alt-V is pressed
            if (Input.GetKeyDown(KeyCode.V) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))) {
                this.isVisible = !this.isVisible;
            }

            // Update panel opacity and displayed stats if visible
            if (this.isVisible) {
                if (this.previousContainsMouse != this.containsMouse) {
                    this.previousContainsMouse = this.containsMouse;
                    this.opacity = this.containsMouse ? 1f : 0.4f;
                }

                this.UpdateDisplay();
            }
        }

        /// <summary>
        /// Called when the panel is destroyed.
        /// </summary>
        public override void OnDestroy() {
            base.OnDestroy();

            if (this.toggleButton != null) {
                GameObject.Destroy(this.toggleButton.gameObject);
            }
        }

        /// <summary>
        /// Creates, positions, and styles all panel controls, as well as the toggle button in the main UI.
        /// </summary>
        private void SetUpControls() {
            this.CreateToggleButton();
            this.CreateDragHandle();
            this.CreateCloseButton();

            // Grab the electricity panel now so its title font can be copied and used for this panel's title
            var electricityPanel = this.uiParent.GetComponentInChildren<ElectricityInfoViewPanel>();
            var electricityTitle = electricityPanel.Find<UILabel>("Label");
            var gameFont = electricityTitle.font;

            this.CreatePanelTitle(gameFont);
            this.CreateInfoPanel();

            int zOrder = 1;

            // Set up electricity controls
            var electricityAvailabilityLabel = GameObject.Instantiate<UILabel>(electricityPanel.Find<UILabel>("ElectricityAvailability"));
            zOrder = this.PositionInfoControl(electricityAvailabilityLabel, zOrder);

            this.electricityMeter = GameObject.Instantiate<UISlider>(electricityPanel.Find<UISlider>("ElectricityMeter"));
            zOrder = this.PositionInfoControl(this.electricityMeter, zOrder);

            var waterPanel = this.uiParent.GetComponentInChildren<WaterInfoViewPanel>();

            // Set up water and sewage controls
            var waterAvailabilityLabel = GameObject.Instantiate<UILabel>(waterPanel.Find<UILabel>("WaterAvailability"));
            zOrder = this.PositionInfoControl(waterAvailabilityLabel, zOrder);

            this.waterMeter = GameObject.Instantiate<UISlider>(waterPanel.Find<UISlider>("WaterMeter"));
            zOrder = this.PositionInfoControl(this.waterMeter, zOrder);

            var sewageAvailabilityLabel = GameObject.Instantiate<UILabel>(waterPanel.Find<UILabel>("SewageAvailability"));
            zOrder = this.PositionInfoControl(sewageAvailabilityLabel, zOrder);

            this.sewageMeter = GameObject.Instantiate<UISlider>(waterPanel.Find<UISlider>("SewageMeter"));
            zOrder = this.PositionInfoControl(this.sewageMeter, zOrder);

            var garbagePanel = this.uiParent.GetComponentInChildren<GarbageInfoViewPanel>();

            // Set up landfill and incineration controls
            var landfillUsageLabel = GameObject.Instantiate<UILabel>(garbagePanel.Find<UILabel>("LandfillUsage"));
            zOrder = this.PositionInfoControl(landfillUsageLabel, zOrder);

            this.landfillMeter = GameObject.Instantiate<UISlider>(garbagePanel.Find<UISlider>("LandfillMeter"));
            zOrder = this.PositionInfoControl(this.landfillMeter, zOrder);

            var incinerationStatusLabel = GameObject.Instantiate<UILabel>(garbagePanel.Find<UILabel>("IncinerationStatus"));
            zOrder = this.PositionInfoControl(incinerationStatusLabel, zOrder);

            this.incineratorMeter = GameObject.Instantiate<UISlider>(garbagePanel.Find<UISlider>("IncineratorMeter"));
            zOrder = this.PositionInfoControl(this.incineratorMeter, zOrder);

            var healthPanel = this.uiParent.GetComponentInChildren<HealthInfoViewPanel>();

            // Set up cemetery and crematorium controls
            var cemeteryUsageLabel = GameObject.Instantiate(healthPanel.Find<UILabel>("CemetaryUsage"));
            zOrder = this.PositionInfoControl(cemeteryUsageLabel, zOrder);

            this.cemeteryMeter = GameObject.Instantiate<UISlider>(healthPanel.Find<UISlider>("CemetaryMeter"));
            zOrder = this.PositionInfoControl(this.cemeteryMeter, zOrder);

            var crematoriumAvailabilityLabel = GameObject.Instantiate<UILabel>(healthPanel.Find<UILabel>("Incinerator"));
            zOrder = this.PositionInfoControl(crematoriumAvailabilityLabel, zOrder);

            this.crematoriumMeter = GameObject.Instantiate<UISlider>(healthPanel.Find<UISlider>("DeathcareMeter"));
            zOrder = this.PositionInfoControl(this.crematoriumMeter, zOrder);

            // Set up unemployment controls
            var employmentLabel = GameObject.Instantiate<UILabel>(healthPanel.Find<UILabel>("Incinerator"));
            employmentLabel.localeID = "STATS_9";
            zOrder = this.PositionInfoControl(employmentLabel, zOrder);

            this.employmentMeter = GameObject.Instantiate<UISlider>(healthPanel.Find<UISlider>("CemetaryMeter"));
            zOrder = this.PositionInfoControl(this.employmentMeter, zOrder);
        }

        /// <summary>
        /// Creates and positions the toggle button in the main UI.
        /// </summary>
        private void CreateToggleButton() {
            var toggleButtonObject = new GameObject("CityVitalsWatchButton");
            toggleButtonObject.transform.parent = this.uiParent.transform;
            toggleButtonObject.transform.localPosition = Vector3.zero;
            this.toggleButton = toggleButtonObject.AddComponent<UIButton>();
            this.toggleButton.normalBgSprite = "ButtonMenu";
            this.toggleButton.hoveredBgSprite = "ButtonMenuHovered";
            this.toggleButton.pressedBgSprite = "ButtonMenuPressed";
            this.toggleButton.normalFgSprite = "ThumbStatistics";
            this.toggleButton.width = 40f;
            this.toggleButton.height = 40f;
            var resolutionData = CityVitalsWatch.Settings.GetResolutionData(Screen.currentResolution.width, Screen.currentResolution.height);
            this.toggleButton.absolutePosition = new Vector3(resolutionData.ToggleButtonPositionX, resolutionData.ToggleButtonPositionY);
            this.toggleButton.tooltip = "City Vitals";
            this.toggleButton.eventClick += OnToggleButtonClick;
            var toggleButtonDragHandleObject = new GameObject("CityVitalsWatchButtonDragHandler");
            toggleButtonDragHandleObject.transform.parent = this.toggleButton.transform;
            toggleButtonDragHandleObject.transform.localPosition = Vector3.zero;
            var toggleButtonDragHandle = toggleButtonDragHandleObject.AddComponent<UIDragHandle>();
            toggleButtonDragHandle.width = this.toggleButton.width;
            toggleButtonDragHandle.height = this.toggleButton.height;
        }

        /// <summary>
        /// Creates and position a drag handle to allow the panel to be moved by its title bar.
        /// </summary>
        private void CreateDragHandle() {
            var dragHandleObject = new GameObject("DragHandler");
            dragHandleObject.transform.parent = this.transform;
            dragHandleObject.transform.localPosition = Vector3.zero;
            var dragHandle = dragHandleObject.AddComponent<UIDragHandle>();
            dragHandle.width = this.width;
            dragHandle.height = TitleBarHeight;
            dragHandle.zOrder = 1000;
        }

        /// <summary>
        /// Creates and positions the panel's close button.
        /// </summary>
        private void CreateCloseButton() {
            var closeButtonObject = new GameObject("CloseButton");
            closeButtonObject.transform.parent = this.transform;
            closeButtonObject.transform.localPosition = Vector3.zero;
            var closeButton = closeButtonObject.AddComponent<UIButton>();
            closeButton.width = 32f;
            closeButton.height = 32f;
            closeButton.normalBgSprite = "ButtonClose";
            closeButton.hoveredBgSprite = "ButtonCloseHover";
            closeButton.pressedBgSprite = "ButtonClosePressed";
            closeButton.relativePosition = new Vector3(this.width - closeButton.width, 2f);
            closeButton.eventClick += this.OnCloseButtonClick;
        }

        /// <summary>
        /// Creates and positions the panel's title label.
        /// </summary>
        /// <param name="font">The font to use for the title.</param>
        private void CreatePanelTitle(UIFont font) {
            var titleObject = new GameObject("Title");
            titleObject.transform.parent = this.transform;
            titleObject.transform.localPosition = Vector3.zero;
            var title = titleObject.AddComponent<UILabel>();
            title.text = "City Vitals";
            title.textAlignment = UIHorizontalAlignment.Center;
            title.font = font;
            title.position = new Vector3((this.width / 2f) - (title.width / 2f), -20f + ( title.height / 2f));
        }

        /// <summary>
        /// Creates and positions the sub-panel to contain all info controls.
        /// </summary>
        private void CreateInfoPanel() {
            var infoPanelObject = new GameObject("ControlPanel");
            infoPanelObject.transform.parent = this.transform;
            this.infoPanel = infoPanelObject.AddComponent<UIPanel>();
            this.infoPanel.transform.localPosition = Vector3.zero;
            this.infoPanel.width = this.width;
            this.infoPanel.height = 0f;
            this.infoPanel.autoLayoutDirection = LayoutDirection.Vertical;
            this.infoPanel.autoLayoutStart = LayoutStart.TopLeft;
            this.infoPanel.autoLayoutPadding = new RectOffset(8, 16, 0, 5);
            this.infoPanel.autoLayout = true;
            this.infoPanel.relativePosition = new Vector3(0f, 45f);
            this.infoPanel.autoSize = true;
        }

        /// <summary>
        /// Called when the toggle button in the main UI is clicked, toggling panel visibility.
        /// </summary>
        /// <param name="component">The toggle button component.</param>
        /// <param name="eventParam">The event parameters.</param>
        private void OnToggleButtonClick(UIComponent component, UIMouseEventParameter eventParam) {
            this.isVisible = !this.isVisible;
        }

        /// <summary>
        /// Called when the close button is clicked, making the panel invisible.
        /// </summary>
        /// <param name="component">The close button component.</param>
        /// <param name="eventParam">The event parameters.</param>
        private void OnCloseButtonClick(UIComponent component, UIMouseEventParameter eventParam) {
            this.isVisible = false;
        }

        /// <summary>
        /// Positions the provided control.
        /// </summary>
        /// <param name="control">The control to position.</param>
        /// <param name="zOrder">The order index of the control used when automatically laying out the control.</param>
        /// <returns>The incremented order index to be set to the next control.</returns>
        private int PositionInfoControl(UIComponent control, int zOrder) {
            control.cachedTransform.parent = this.infoPanel.transform;
            control.autoSize = true;
            control.zOrder = zOrder;
            var panelHeightAdjustment = control.height + this.infoPanel.autoLayoutPadding.top + this.infoPanel.autoLayoutPadding.bottom;
            this.height += panelHeightAdjustment;
            this.infoPanel.height += panelHeightAdjustment;
            return zOrder + 1;
        }

        /// <summary>
        /// Copies the font, localization label ID, and color from the source label to the specified target label.
        /// </summary>
        /// <param name="source">The source label from which to copy property values.</param>
        /// <param name="target">The target label.</param>
        private void CopyLabel(UILabel source, UILabel target) {
            target.font = source.font;
            target.localeID = source.localeID;
            target.color = source.color;
        }

        /// <summary>
        /// Updates the displayed stat values and tooltips for all info controls.
        /// </summary>
        private void UpdateDisplay() {
            int electricityCapacity = 0;
            int electricityConsumption = 0;
            int waterCapacity = 0;
            int waterConsumption = 0;
            int sewageCapacity = 0;
            int sewageAccumulation = 0;
            int garbageCapacity = 0;
            int garbageAmount = 0;
            int incinerationCapacity = 0;
            int garbageAccumulation = 0;
            int deadCapacity = 0;
            int deadAmount = 0;
            int cremateCapacity = 0;
            int deadCount = 0;
            float unemployment = 0f;

            if (Singleton<DistrictManager>.exists) {
                var info = Singleton<DistrictManager>.instance.m_districts.m_buffer[0];
                electricityCapacity = info.GetElectricityCapacity();
                electricityConsumption = info.GetElectricityConsumption();
                waterCapacity = info.GetWaterCapacity();
                waterConsumption = info.GetWaterConsumption();
                sewageCapacity = info.GetSewageCapacity();
                sewageAccumulation = info.GetSewageAccumulation();
                garbageCapacity = info.GetGarbageCapacity();
                garbageAmount = info.GetGarbageAmount();
                incinerationCapacity = info.GetIncinerationCapacity();
                garbageAccumulation = info.GetGarbageAccumulation();
                deadCapacity = info.GetDeadCapacity();
                deadAmount = info.GetDeadAmount();
                cremateCapacity = info.GetCremateCapacity();
                deadCount = info.GetDeadCount();
                unemployment = info.GetUnemployment();
            }

            this.electricityMeter.value = this.GetPercentage(electricityCapacity, electricityConsumption);
            this.electricityMeter.tooltip = this.GetUsageString(electricityCapacity / 1000f, electricityConsumption / 1000f);

            this.waterMeter.value = this.GetPercentage(waterCapacity, waterConsumption);
            this.waterMeter.tooltip = this.GetUsageString(waterCapacity, waterConsumption);
            this.sewageMeter.value = this.GetPercentage(sewageCapacity, sewageAccumulation);
            this.sewageMeter.tooltip = this.GetUsageString(sewageCapacity, sewageAccumulation);

            if (garbageCapacity > 0) {
                this.landfillMeter.value = (garbageAmount / (float)garbageCapacity) * 100f;
            }
            else {
                this.landfillMeter.value = 0f;
            }

            this.landfillMeter.tooltip = this.landfillMeter.value + "%";
            this.incineratorMeter.value = this.GetPercentage(incinerationCapacity, garbageAccumulation);
            this.incineratorMeter.tooltip = this.GetUsageString(incinerationCapacity, garbageAccumulation);

            if (deadCapacity > 0) {
                this.cemeteryMeter.value = (deadAmount / (float)deadCapacity) * 100f;
            }
            else {
                this.cemeteryMeter.value = 0f;
            }

            this.cemeteryMeter.tooltip = this.cemeteryMeter.value + "%";
            this.crematoriumMeter.value = this.GetPercentage(cremateCapacity, deadCount);
            this.crematoriumMeter.tooltip = this.GetUsageString(cremateCapacity, deadCount);

            this.employmentMeter.value = Mathf.Round(100f - unemployment);
            this.employmentMeter.tooltip = this.employmentMeter.value + "%";
        }

        /// <summary>
        /// Calculates a percentage  based on the specified capacity and consumption values using Cities: Skylines' percentage algorithm.
        /// </summary>
        /// <param name="capacity">The capacity value.</param>
        /// <param name="consumption">The consumption value.</param>
        /// <returns></returns>
        private float GetPercentage(int capacity, int consumption, int consumptionMin = 45, int consumptionMax = 55) {
            /* This algorithm is what's used by the class InfoViewPanel to determine percentages displayed through the UI.
             * I'm unaware of the reasons for choosing the default values for consumptionMin and consumptionMax, but I
             * wanted to keep the logic consistent with the built-in UI sliders (obviously basePercent is always
             * multiplied by 50, but I don't know why). The logic is the same but I cleaned it up a bit.
             */
            if (capacity == 0) {
                return 0f;
            }
            else {
                float basePercent = capacity / (float)consumption;
                float percentModifier = (float)((consumptionMin + consumptionMax) / 2);
                return basePercent * percentModifier;
            }
        }

        private string GetUsageString(float capacity, float consumption) {
            return string.Format(
                "{0} / {1}",
                consumption == 0 ? "0" : Mathf.RoundToInt(consumption).ToString("#,#", CultureInfo.InvariantCulture),
                capacity == 0 ? "0" : Mathf.RoundToInt(capacity).ToString("#,#", CultureInfo.InvariantCulture));
        }
    }
}