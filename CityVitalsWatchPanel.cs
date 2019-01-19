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

        private static readonly float PanelWidth = 215f;
        private static readonly float TitleBarHeight = 40f;
        private static readonly float MeterWidth = 200f;
        private static readonly float MeterHeight = 12f;
        private static readonly float MeterIndicatorSize = 14f;

        private bool previousContainsMouse = true;
        private UIView uiParent;
        private UIButton toggleButton;
        private UIPanel infoPanel;
        private CityVitalsWatchSettingsPanel settingsPanel;

        private UISlider electricityMeter;
        private UISlider waterMeter;
        private UISlider sewageMeter;
        private UISlider landfillMeter;
        private UISlider incineratorMeter;
        private UISlider healthcareMeter;
        private UISlider averageHealthMeter;
        private UISlider cemeteryMeter;
        private UISlider crematoriumMeter;
        private UISlider fireMeter;
        private UISlider crimeMeter;
        private UISlider jailMeter;
        private UISlider elementarySchoolMeter;
        private UISlider highSchoolMeter;
        private UISlider universityMeter;
        private UISlider employmentMeter;
        private UISlider jobAvailabilityMeter;

        /// <summary>
        /// The button in the main UI used to toggle the panel's visibility.
        /// </summary>
        public UIButton ToggleButton {
            get { return this.toggleButton; }
        }

        /// <summary>
        /// Called before the first frame after the panel is created.
        /// </summary>
        public override void Start() {
            // Find the top-level UIView object containing all game controls to copy
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
            this.height = TitleBarHeight + 10f;

            try {
                this.SetUpControls();
            }
            catch (Exception e) {
                // If for some reason control setup threw an exception, destroy the panel instead of staying broken
                GameObject.Destroy(this.gameObject);

                // Rethrow the exception to help debug any issues (Message is not helpful, but StackTrace is)
                throw new Exception(e.Message + " - " + e.StackTrace);
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
                    float hoveredOpacity = 1f;
                    float unhoveredOpacity = CityVitalsWatch.Settings.TransparentUnhovered ? 0.4f : 1f;
                    this.previousContainsMouse = this.containsMouse;
                    this.opacity = this.containsMouse ? hoveredOpacity : unhoveredOpacity;
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
                GameObject.Destroy(this.settingsPanel.gameObject);
            }
        }

        /// <summary>
        /// Creates, positions, and styles all panel controls, as well as the toggle button in the main UI.
        /// </summary>
        private void SetUpControls() {
            this.CreateToggleButton();
            
            // Grab the electricity panel now so its title titleFont can be copied and used for this panel's title
            var electricityPanel = this.uiParent.GetComponentInChildren<ElectricityInfoViewPanel>();
            var electricityTitle = electricityPanel.Find<UILabel>("Label");
            var titleFont = electricityTitle.font;

            // Grab a label from the electricity panel to use as a template for the settings panel label
            UILabel labelTemplate = electricityPanel.Find<UILabel>("ElectricityAvailability");

            // Create and position all non-stat controls first
            this.CreatePanelTitle(titleFont);
            this.CreateDragHandle();
            this.CreatePanelButtons();
            this.CreateInfoPanel();
            this.CreateSettingsPanel(titleFont, labelTemplate);

            Color targetColor = Color.cyan;
            Color negativeColor = Color.red;

            if (Singleton<InfoManager>.exists) {
                targetColor = Singleton<InfoManager>.instance.m_properties.m_modeProperties[4].m_targetColor;
                negativeColor = Singleton<InfoManager>.instance.m_properties.m_modeProperties[4].m_negativeColor;
            }

            // zOrder must be manually assigned so the controls are aligned properly by the UI framework
            int zOrder = 1;

            // Set up electricity controls
            if (CityVitalsWatch.Settings.DisplayElectricityAvailability) {
                var electricityAvailabilityLabel = this.CreateLabel(electricityPanel.Find<UILabel>("ElectricityAvailability"));
                this.PositionInfoControl(electricityAvailabilityLabel, ref zOrder, this.CreateServiceMenuClickHandler(4));

                this.electricityMeter = this.CreateAvailabilityMeter("Electricity");
                this.PositionInfoControl(this.electricityMeter, ref zOrder, this.CreateServiceMenuClickHandler(4));
            }

            var waterPanel = this.uiParent.GetComponentInChildren<WaterInfoViewPanel>();

            // Set up water controls
            if (CityVitalsWatch.Settings.DisplayWaterAvailability) {
                var waterAvailabilityLabel = this.CreateLabel(waterPanel.Find<UILabel>("WaterAvailability"));
                this.PositionInfoControl(waterAvailabilityLabel, ref zOrder, this.CreateServiceMenuClickHandler(5));

                this.waterMeter = this.CreateAvailabilityMeter("Water");
                this.PositionInfoControl(this.waterMeter, ref zOrder, this.CreateServiceMenuClickHandler(5));
            }

            // Set up sewage controls
            if (CityVitalsWatch.Settings.DisplaySewageTreatment) {
                var sewageAvailabilityLabel = this.CreateLabel(waterPanel.Find<UILabel>("SewageAvailability"));
                this.PositionInfoControl(sewageAvailabilityLabel, ref zOrder, this.CreateServiceMenuClickHandler(5));

                this.sewageMeter = this.CreateAvailabilityMeter("Sewage");
                this.PositionInfoControl(this.sewageMeter, ref zOrder, this.CreateServiceMenuClickHandler(5));
            }

            var garbagePanel = this.uiParent.GetComponentInChildren<GarbageInfoViewPanel>();

            // Set up landfill controls
            if (CityVitalsWatch.Settings.DisplayLandfillUsage) {
                var landfillUsageLabel = this.CreateLabel(garbagePanel.Find<UILabel>("LandfillUsage"));
                this.PositionInfoControl(landfillUsageLabel, ref zOrder, this.CreateServiceMenuClickHandler(6));

                var landfillTexture = garbagePanel.Find<UISlider>("LandfillMeter").Find<UITextureSprite>("LandfillGradient");
                this.landfillMeter = this.CreateGradientMeter("Landfill", landfillTexture, targetColor, negativeColor);
                this.PositionInfoControl(this.landfillMeter, ref zOrder, this.CreateServiceMenuClickHandler(6));
            }

            // Set up incineration controls
            if (CityVitalsWatch.Settings.DisplayIncinerationStatus) {
                var incinerationStatusLabel = this.CreateLabel(garbagePanel.Find<UILabel>("IncinerationStatus"));
                this.PositionInfoControl(incinerationStatusLabel, ref zOrder, this.CreateServiceMenuClickHandler(6));

                this.incineratorMeter = this.CreateAvailabilityMeter("Incinerator");
                this.PositionInfoControl(this.incineratorMeter, ref zOrder, this.CreateServiceMenuClickHandler(6));
            }

            var healthPanel = this.uiParent.GetComponentInChildren<HealthInfoViewPanel>();

            // Set up healthcare controls
            if (CityVitalsWatch.Settings.DisplayHealthcareAvailability) {
                var healthcareAvailabilityLabel = this.CreateLabel(healthPanel.Find<UILabel>("HealthcareAvaibility")); // NOTE: CO misspelled this
                this.PositionInfoControl(healthcareAvailabilityLabel, ref zOrder, this.CreateServiceMenuClickHandler(8));

                this.healthcareMeter = this.CreateAvailabilityMeter("Healthcare");
                this.PositionInfoControl(this.healthcareMeter, ref zOrder, this.CreateServiceMenuClickHandler(8));
            }

            // Set up average health controls
            if (CityVitalsWatch.Settings.DisplayAverageHealth) {
                var averageHealthLabel = this.CreateLabel(healthPanel.Find<UILabel>("AvgHealth"));
                this.PositionInfoControl(averageHealthLabel, ref zOrder, this.CreateServiceMenuClickHandler(8));

                var averageHealthTexture = healthPanel.Find<UISlider>("AvgHealthBar").Find<UITextureSprite>("Background");
                this.averageHealthMeter = this.CreateGradientMeter("AverageHealth", averageHealthTexture, negativeColor, targetColor);
                this.PositionInfoControl(this.averageHealthMeter, ref zOrder, this.CreateServiceMenuClickHandler(8));
            }

            // Set up cemetery controls
            if (CityVitalsWatch.Settings.DisplayCemeteryUsage) {
                var cemeteryUsageLabel = this.CreateLabel(healthPanel.Find<UILabel>("CemetaryUsage")); // NOTE: CO misspelled this
                this.PositionInfoControl(cemeteryUsageLabel, ref zOrder, this.CreateServiceMenuClickHandler(8));

                var cemeteryTexture = healthPanel.Find<UISlider>("CemetaryMeter").Find<UITextureSprite>("Background"); // NOTE: CO misspelled this
                this.cemeteryMeter = this.CreateGradientMeter("Cemetery", cemeteryTexture, targetColor, negativeColor);
                this.PositionInfoControl(this.cemeteryMeter, ref zOrder, this.CreateServiceMenuClickHandler(8));
            }

            // Set up crematorium controls
            if (CityVitalsWatch.Settings.DisplayCrematoriumAvailability) {
                var crematoriumAvailabilityLabel = this.CreateLabel(healthPanel.Find<UILabel>("Incinerator"));
                this.PositionInfoControl(crematoriumAvailabilityLabel, ref zOrder, this.CreateServiceMenuClickHandler(8));

                this.crematoriumMeter = this.CreateAvailabilityMeter("Crematorium");
                this.PositionInfoControl(this.crematoriumMeter, ref zOrder, this.CreateServiceMenuClickHandler(8));
            }

            var firePanel = this.uiParent.GetComponentInChildren<FireSafetyInfoViewPanel>();

            // Set up fire hazard controls
            if (CityVitalsWatch.Settings.DisplayFireHazard) {
                var fireHazardLabel = this.CreateLabel(firePanel.Find<UILabel>("SafetyLabel"));
                this.PositionInfoControl(fireHazardLabel, ref zOrder, this.CreateServiceMenuClickHandler(9));

                var fireTexture = firePanel.Find<UISlider>("SafetyMeter").Find<UITextureSprite>("SafetyGradient");
                this.fireMeter = this.CreateGradientMeter("FireHazard", fireTexture, targetColor, negativeColor);
                this.PositionInfoControl(fireMeter, ref zOrder, this.CreateServiceMenuClickHandler(9));
            }

            var crimePanel = this.uiParent.GetComponentInChildren<CrimeInfoViewPanel>();

            // Set up crime controls
            if (CityVitalsWatch.Settings.DisplayCrimeRate) {
                var crimeRateLabel = this.CreateLabel(crimePanel.Find<UILabel>("SafetyLabel"));
                this.PositionInfoControl(crimeRateLabel, ref zOrder, this.CreateServiceMenuClickHandler(10));

                var crimeTexture = crimePanel.Find<UISlider>("SafetyMeter").Find<UITextureSprite>("SafetyGradient");
                this.crimeMeter = this.CreateGradientMeter("CrimeRate", crimeTexture, targetColor, negativeColor);
                this.PositionInfoControl(this.crimeMeter, ref zOrder, this.CreateServiceMenuClickHandler(10));
            }

            if (CityVitalsWatch.Settings.DisplayJailAvailability) {
                var jailAvailabilityLabel = this.CreateLabel(crimePanel.Find<UILabel>("JailAvailability"));
                this.PositionInfoControl(jailAvailabilityLabel, ref zOrder, this.CreateServiceMenuClickHandler(10));

                this.jailMeter = this.CreateAvailabilityMeter("Jail");
                this.PositionInfoControl(this.jailMeter, ref zOrder, this.CreateServiceMenuClickHandler(10));
            }

            var educationPanel = this.uiParent.GetComponentInChildren<EducationInfoViewPanel>();

            // Set up elementary school controls
            if (CityVitalsWatch.Settings.DisplayElementarySchoolAvailability) {
                var elementarySchoolLabel = this.CreateLabel(educationPanel.Find<UILabel>("ElementaryAvailability"));
                this.PositionInfoControl(elementarySchoolLabel, ref zOrder, this.CreateServiceMenuClickHandler(12));

                this.elementarySchoolMeter = this.CreateAvailabilityMeter("ElementarySchool");
                this.PositionInfoControl(this.elementarySchoolMeter, ref zOrder, this.CreateServiceMenuClickHandler(12));
            }

            // Set up high school controls
            if (CityVitalsWatch.Settings.DisplayHighSchoolAvailability) {
                var highSchoolLabel = this.CreateLabel(educationPanel.Find<UILabel>("HighAvailability"));
                this.PositionInfoControl(highSchoolLabel, ref zOrder, this.CreateServiceMenuClickHandler(12));

                this.highSchoolMeter = this.CreateAvailabilityMeter("HighSchool");
                this.PositionInfoControl(this.highSchoolMeter, ref zOrder, this.CreateServiceMenuClickHandler(12));
            }

            // Set up university controls
            if (CityVitalsWatch.Settings.DisplayUniversityAvailability) {
                var universityLabel = this.CreateLabel(educationPanel.Find<UILabel>("UnivAvailability"));
                this.PositionInfoControl(universityLabel, ref zOrder, this.CreateServiceMenuClickHandler(12));

                this.universityMeter = this.CreateAvailabilityMeter("University");
                this.PositionInfoControl(this.universityMeter, ref zOrder, this.CreateServiceMenuClickHandler(12));
            }

            // Set up unemployment controls
            if (CityVitalsWatch.Settings.DisplayEmployment) {
                var employmentLabel = this.CreateLabel(healthPanel.Find<UILabel>("Incinerator"));
                employmentLabel.localeID = "STATS_9";
                this.PositionInfoControl(employmentLabel, ref zOrder);

                var employmentTexture = garbagePanel.Find<UISlider>("LandfillMeter").Find<UITextureSprite>("LandfillGradient");
                this.employmentMeter = this.CreateGradientMeter("Employment", employmentTexture, negativeColor, targetColor);
                this.PositionInfoControl(this.employmentMeter, ref zOrder);
            }
            
            // Set up job availability controls
            if (CityVitalsWatch.Settings.DisplayJobAvailability) {
                var jobAvailabilityLabel = this.CreateLabel(healthPanel.Find<UILabel>("Incinerator"));
                jobAvailabilityLabel.localeID = "STATS_10";
                this.PositionInfoControl(jobAvailabilityLabel, ref zOrder);

                var jobAvailabilityTexture = garbagePanel.Find<UISlider>("LandfillMeter").Find<UITextureSprite>("LandfillGradient");
                this.jobAvailabilityMeter = this.CreateGradientMeter("JobAvailability", jobAvailabilityTexture, negativeColor, targetColor);
                this.PositionInfoControl(this.jobAvailabilityMeter, ref zOrder);
            }
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
            dragHandle.zOrder = 0;
            dragHandle.BringToFront();
        }

        /// <summary>
        /// Creates and positions the panel's options and close buttons.
        /// </summary>
        private void CreatePanelButtons() {
            var closeButtonObject = new GameObject("CloseButton");
            closeButtonObject.transform.parent = this.transform;
            closeButtonObject.transform.localPosition = Vector3.zero;
            var closeButton = closeButtonObject.AddComponent<UIButton>();
            closeButton.width = 32f;
            closeButton.height = 32f;
            closeButton.normalBgSprite = "buttonclose";
            closeButton.hoveredBgSprite = "buttonclosehover";
            closeButton.pressedBgSprite = "buttonclosepressed";
            closeButton.relativePosition = new Vector3(this.width - closeButton.width, 2f);
            closeButton.eventClick += this.OnCloseButtonClick;

            var optionsButtonObject = new GameObject("OptionsButton");
            optionsButtonObject.transform.parent = this.transform;
            optionsButtonObject.transform.localPosition = Vector3.zero;
            var optionsButton = optionsButtonObject.AddComponent<UIButton>();
            optionsButton.width = 24f;
            optionsButton.height = 24f;
            optionsButton.normalBgSprite = "RoundBackBig";
            optionsButton.hoveredBgSprite = "RoundBackBigHovered";
            optionsButton.pressedBgSprite = "RoundBackBigPressed";
            optionsButton.normalFgSprite = "Options";
            optionsButton.relativePosition = new Vector3(this.width - closeButton.width - optionsButton.width, 7f);
            optionsButton.eventClick += OnOptionsButtonClick;
        }

        /// <summary>
        /// Creates and positions the panel's title label.
        /// </summary>
        /// <param name="titleFont">The font to use for the title.</param>
        private void CreatePanelTitle(UIFont font) {
            var titleObject = new GameObject("Title");
            titleObject.transform.parent = this.transform;
            titleObject.transform.localPosition = Vector3.zero;
            var title = titleObject.AddComponent<UILabel>();
            title.text = "City Vitals";
            title.textAlignment = UIHorizontalAlignment.Center;
            title.font = font;
            title.position = new Vector3((this.width / 2f) - (title.width / 2f), -20f + (title.height / 2f));
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
            this.infoPanel.autoLayoutPadding = new RectOffset(8, 16, 0, 0);
            this.infoPanel.autoLayout = true;
            this.infoPanel.relativePosition = new Vector3(0f, TitleBarHeight + 5f);
        }

        /// <summary>
        /// Creates and initializes the settings panel in the main UI.
        /// </summary>
        /// <param name="titleFont">The font to use for the settings panel title.</param>
        /// <param name="labelTemplate">The template to use for all settings labels.</param>
        private void CreateSettingsPanel(UIFont titleFont, UILabel labelTemplate) {
            var settingsPanelObject = new GameObject("CityVitalsWatchSettingsPanel");
            settingsPanelObject.transform.parent = this.uiParent.transform;
            this.settingsPanel = settingsPanelObject.AddComponent<CityVitalsWatchSettingsPanel>();
            this.settingsPanel.Initialize(titleFont, labelTemplate);
        }

        /// <summary>
        /// Creates a new label and copies the properties of the provided template.
        /// </summary>
        /// <param name="labelTemplate">The label template.</param>
        /// <returns>The created label.</returns>
        private UILabel CreateLabel(UILabel labelTemplate) {
            GameObject labelObject = new GameObject(labelTemplate.name);
            labelObject.transform.parent = this.infoPanel.transform;
            UILabel label = labelObject.AddComponent<UILabel>();
            label.font = labelTemplate.font;
            label.textColor = labelTemplate.textColor;
            label.textScale = labelTemplate.textScale;
            label.localeID = labelTemplate.localeID;
            return label;
        }

        /// <summary>
        /// Creates a new availability meter using the standard meter sprite.
        /// </summary>
        /// <param name="statName">The name of the meter's stat, used for object naming.</param>
        /// <returns>The created availability meter.</returns>
        private UISlider CreateAvailabilityMeter(string statName) {
            // Create the slider
            GameObject sliderObject = new GameObject(statName + "Meter");
            sliderObject.transform.parent = this.infoPanel.transform;
            UISlider slider = sliderObject.AddComponent<UISlider>();
            slider.width = MeterWidth;
            slider.height = MeterHeight;
            slider.backgroundSprite = "MeterBackground";

            // Create the indicator
            GameObject indicatorObject = new GameObject(statName + "Indicator");
            indicatorObject.transform.parent = slider.transform;
            UISprite indicator = indicatorObject.AddComponent<UISprite>();
            indicator.spriteName = "MeterIndicator";
            indicator.width = MeterIndicatorSize;
            indicator.height = MeterIndicatorSize;
            slider.thumbObject = indicator;

            return slider;
        }

        /// <summary>
        /// Creates a new gradient meter using the specified meter texture and colors.
        /// </summary>
        /// <param name="statName">The name of the meter's stat, used for object naming.</param>
        /// <param name="gradientTexture">The texture to used for the meter's gradient.</param>
        /// <param name="colorA">The color to render on the left side of the meter.</param>
        /// <param name="colorB">The color to render on the right side of the meter.</param>
        /// <returns>The created gradient meter.</returns>
        private UISlider CreateGradientMeter(string statName, UITextureSprite gradientTexture, Color colorA, Color colorB) {
            // Create the slider
            GameObject sliderObject = new GameObject(statName + "Meter");
            sliderObject.transform.parent = this.infoPanel.transform;
            UISlider slider = sliderObject.AddComponent<UISlider>();
            slider.width = MeterWidth;
            slider.height = MeterHeight;

            // Create the indicator
            GameObject indicatorObject = new GameObject(statName + "Indicator");
            indicatorObject.transform.parent = slider.transform;
            UISprite indicator = indicatorObject.AddComponent<UISprite>();
            indicator.spriteName = "MeterIndicator";
            indicator.width = MeterIndicatorSize;
            indicator.height = MeterIndicatorSize;
            slider.thumbObject = indicator;

            // Create the gradient texture and setup how its material is rendered
            UITextureSprite gradient = GameObject.Instantiate<UITextureSprite>(gradientTexture);
            gradient.name = statName + "Gradient";
            gradient.transform.parent = slider.transform;
            gradient.transform.localPosition = Vector3.zero;
            gradient.width = slider.width;
            gradient.height = slider.height;
            gradient.renderMaterial.SetColor("_ColorA", colorA);
            gradient.renderMaterial.SetColor("_ColorB", colorB);

            return slider;
        }

        /// <summary>
        /// Creates a click handler delegate that opens the service menu with the specified index.
        /// </summary>
        /// <param name="menuIndex">The service menu index to open on click.</param>
        /// <returns>The created click handler delegate.</returns>
        private MouseEventHandler CreateServiceMenuClickHandler(int menuIndex) {
            return delegate(UIComponent component, UIMouseEventParameter eventParam) {
                UITabstrip mainToolstrip = this.uiParent.FindUIComponent<UITabstrip>("MainToolstrip");

                if (mainToolstrip != null) {
                    mainToolstrip.selectedIndex = menuIndex;
                }
            };
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
        /// Called when the settings button is clicked, toggling settings panel visibility.
        /// </summary>
        /// <param name="component">The settings button component.</param>
        /// <param name="eventParam">The event parameters.</param>
        private void OnOptionsButtonClick(UIComponent component, UIMouseEventParameter eventParam) {
            this.settingsPanel.relativePosition = this.relativePosition;
            this.settingsPanel.isVisible = true;
            this.settingsPanel.BringToFront();
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
        /// <param name="clickEventHandler">The event handler for when the control is clicked.</param>
        private void PositionInfoControl(UIComponent control, ref int zOrder, MouseEventHandler clickEventHandler = null) {
            control.cachedTransform.parent = this.infoPanel.transform;
            control.zOrder = zOrder;

            var panelHeightAdjustment = control.height;

            if (control is UISlider) {
                control.width = MeterWidth;
            }
            else if (control is UILabel) {
                var label = control as UILabel;

                // Set the top padding of the label if it's not the first
                if (zOrder != 1) {
                    label.padding.top = 5;
                }

                panelHeightAdjustment += label.padding.top + label.padding.bottom;
            }

            if (clickEventHandler != null) {
                control.eventClick += clickEventHandler;
            }

            this.height += panelHeightAdjustment;
            this.infoPanel.height += panelHeightAdjustment;

            zOrder++;
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
            int healCapacity = 0;
            int sickCount = 0;
            int averageHealth = 0;
            int deadCapacity = 0;
            int deadAmount = 0;
            int cremateCapacity = 0;
            int deadCount = 0;
            int fireHazard = 0;
            int crimeRate = 0;
            int criminalCapacity = 0;
            int criminalAmount = 0;
            int extraCriminalAmount = 0;
            int elementarySchoolCapacity = 0;
            int elementarySchoolNeed = 0;
            int highSchoolCapacity = 0;
            int highSchoolNeed = 0;
            int universityCapacity = 0;
            int universityNeed = 0;
            float unemployment = 0f;
            float jobAvailability = 0f;

            // Grab all of the stat values from the singleton DistrictManager instance
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
                healCapacity = info.GetHealCapacity();
                sickCount = info.GetSickCount();
                averageHealth = info.m_residentialData.m_finalHealth;
                deadCapacity = info.GetDeadCapacity();
                deadAmount = info.GetDeadAmount();
                cremateCapacity = info.GetCremateCapacity();
                deadCount = info.GetDeadCount();
                crimeRate = info.m_finalCrimeRate;
                criminalCapacity = info.GetCriminalCapacity();
                criminalAmount = info.GetCriminalAmount();
                extraCriminalAmount = info.GetExtraCriminals();
                elementarySchoolCapacity = info.GetEducation1Capacity();
                elementarySchoolNeed = info.GetEducation1Need();
                highSchoolCapacity = info.GetEducation2Capacity();
                highSchoolNeed = info.GetEducation2Need();
                universityCapacity = info.GetEducation3Capacity();
                universityNeed = info.GetEducation3Need();
                unemployment = info.GetUnemployment();
                jobAvailability = GetJobAvailability(info);
            }

            // Fire hazard is stored in the singleton ImmaterialResourceManager instead
            if (Singleton<ImmaterialResourceManager>.exists) {
                Singleton<ImmaterialResourceManager>.instance.CheckTotalResource(ImmaterialResourceManager.Resource.FireHazard, out fireHazard);
            }

            if (this.electricityMeter != null) {
                this.electricityMeter.value = this.GetAvailabilityPercentage(electricityCapacity, electricityConsumption);
                this.electricityMeter.tooltip = this.GetUsageString(electricityCapacity / 1000f, electricityConsumption / 1000f);
            }

            if (this.waterMeter != null) {
                this.waterMeter.value = this.GetAvailabilityPercentage(waterCapacity, waterConsumption);
                this.waterMeter.tooltip = this.GetUsageString(waterCapacity, waterConsumption);
            }

            if (this.sewageMeter != null) {
                this.sewageMeter.value = this.GetAvailabilityPercentage(sewageCapacity, sewageAccumulation);
                this.sewageMeter.tooltip = this.GetUsageString(sewageCapacity, sewageAccumulation);
            }

            if (this.landfillMeter != null) {
                if (garbageCapacity > 0) {
                    this.landfillMeter.value = (garbageAmount / (float)garbageCapacity) * 100f;
                }
                else {
                    this.landfillMeter.value = 0f;
                }

                this.landfillMeter.tooltip = this.landfillMeter.value + "%";
            }

            if (this.incineratorMeter != null) {
                this.incineratorMeter.value = this.GetAvailabilityPercentage(incinerationCapacity, garbageAccumulation);
                this.incineratorMeter.tooltip = this.GetUsageString(incinerationCapacity, garbageAccumulation);
            }

            if (this.healthcareMeter != null) {
                this.healthcareMeter.value = this.GetAvailabilityPercentage(healCapacity, sickCount);
                this.healthcareMeter.tooltip = this.GetUsageString(healCapacity, sickCount);
            }

            if (this.averageHealthMeter != null) {
                this.averageHealthMeter.value = (float)averageHealth;
                this.averageHealthMeter.tooltip = averageHealth + "%";
            }

            if (this.cemeteryMeter != null) {
                if (deadCapacity > 0) {
                    this.cemeteryMeter.value = (deadAmount / (float)deadCapacity) * 100f;
                }
                else {
                    this.cemeteryMeter.value = 0f;
                }

                this.cemeteryMeter.tooltip = this.cemeteryMeter.value + "%";
            }

            if (this.crematoriumMeter != null) {
                this.crematoriumMeter.value = this.GetAvailabilityPercentage(cremateCapacity, deadCount);
                this.crematoriumMeter.tooltip = this.GetUsageString(cremateCapacity, deadCount);
            }

            if (this.fireMeter != null) {
                this.fireMeter.value = Mathf.Clamp(fireHazard, 0f, 100f);
                this.fireMeter.tooltip = this.fireMeter.value + "%";
            }

            if (this.crimeMeter != null) {
                this.crimeMeter.value = crimeRate;
                this.crimeMeter.tooltip = this.crimeMeter.value + "%";
            }

            if (this.jailMeter != null) {
                this.jailMeter.value = this.GetAvailabilityPercentage(criminalCapacity, criminalAmount + extraCriminalAmount);
                this.jailMeter.tooltip = this.GetUsageString(criminalCapacity, criminalAmount + extraCriminalAmount);
            }

            if (this.elementarySchoolMeter != null) {
                this.elementarySchoolMeter.value = this.GetAvailabilityPercentage(elementarySchoolCapacity, elementarySchoolNeed);
                this.elementarySchoolMeter.tooltip = this.GetUsageString(elementarySchoolCapacity, elementarySchoolNeed);
            }

            if (this.highSchoolMeter != null) {
                this.highSchoolMeter.value = this.GetAvailabilityPercentage(highSchoolCapacity, highSchoolNeed);
                this.highSchoolMeter.tooltip = this.GetUsageString(highSchoolCapacity, highSchoolNeed);
            }

            if (this.universityMeter != null) {
                this.universityMeter.value = this.GetAvailabilityPercentage(universityCapacity, universityNeed);
                this.universityMeter.tooltip = this.GetUsageString(universityCapacity, universityNeed);
            }

            if (this.employmentMeter != null) {
                this.employmentMeter.value = Mathf.Round(100f - unemployment);
                this.employmentMeter.tooltip = this.employmentMeter.value + "%";
            }
            
            if (this.jobAvailabilityMeter != null) {
                this.jobAvailabilityMeter.value = Mathf.Round(100f - jobAvailability);
                this.jobAvailabilityMeter.tooltip = this.jobAvailabilityMeter.value + "%";
            }
        }

        // Calculates percentage of jobs available
        private float GetJobAvailability(District district)
        {
            int currentJobsFilled = (int)district.m_commercialData.m_finalAliveCount
                + (int)district.m_industrialData.m_finalAliveCount
                + (int)district.m_officeData.m_finalAliveCount
                + (int)district.m_playerData.m_finalAliveCount;

            int availableJobs = (int)district.m_commercialData.m_finalHomeOrWorkCount
                + (int)district.m_industrialData.m_finalHomeOrWorkCount
                + (int)district.m_officeData.m_finalHomeOrWorkCount
                + (int)district.m_playerData.m_finalHomeOrWorkCount;

            float result = 100f * currentJobsFilled / availableJobs;
            return result;
        }

        /// <summary>
        /// Calculates an availability percentage based on the specified capacity and consumption values using
        /// Cities: Skylines' percentage algorithm.
        /// </summary>
        /// <param name="capacity">The capacity value.</param>
        /// <param name="consumption">The consumption value.</param>
        /// <returns>The availability percentage.</returns>
        private float GetAvailabilityPercentage(int capacity, int consumption, int consumptionMin = 45, int consumptionMax = 55) {
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

        /// <summary>
        /// Get a user-friendly string to display numeric service usage.
        /// </summary>
        /// <param name="capacity">The capacity value.</param>
        /// <param name="consumption">The consumption value.</param>
        /// <returns>The user-friendly string.</returns>
        private string GetUsageString(float capacity, float consumption) {
            return string.Format(
                "{0} / {1}",
                consumption == 0 ? "0" : Mathf.RoundToInt(consumption).ToString("#,#", CultureInfo.InvariantCulture),
                capacity == 0 ? "0" : Mathf.RoundToInt(capacity).ToString("#,#", CultureInfo.InvariantCulture));
        }
    }
}
