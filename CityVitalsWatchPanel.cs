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
        private UISlider elementarySchoolMeter;
        private UISlider highSchoolMeter;
        private UISlider universityMeter;
        private UISlider employmentMeter;

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
                GameObject.Destroy(this.settingsPanel.gameObject);
            }
        }

        /// <summary>
        /// Creates, positions, and styles all panel controls, as well as the toggle button in the main UI.
        /// </summary>
        private void SetUpControls() {
            this.CreateToggleButton();
            this.CreateDragHandle();
            this.CreatePanelButtons();

            // Grab the electricity panel now so its title titleFont can be copied and used for this panel's title
            var electricityPanel = this.uiParent.GetComponentInChildren<ElectricityInfoViewPanel>();
            var electricityTitle = electricityPanel.Find<UILabel>("Label");
            var titleFont = electricityTitle.font;

            // Grab a label from the electricity panel to use as a template for the settings panel label
            UILabel labelTemplate = electricityPanel.Find<UILabel>("ElectricityAvailability");

            this.CreatePanelTitle(titleFont);
            this.CreateInfoPanel();
            this.CreateSettingsPanel(titleFont, labelTemplate);

            Color targetColor = Color.cyan;
            Color negativeColor = Color.red;

            if (Singleton<InfoManager>.exists) {
                targetColor = Singleton<InfoManager>.instance.m_properties.m_modeProperties[4].m_targetColor;
                negativeColor = Singleton<InfoManager>.instance.m_properties.m_modeProperties[4].m_negativeColor;
            }

            int zOrder = 1;

            // Set up electricity controls
            if (CityVitalsWatch.Settings.DisplayElectricityAvailability) {
                var electricityAvailabilityLabel = GameObject.Instantiate<UILabel>(electricityPanel.Find<UILabel>("ElectricityAvailability"));
                this.PositionInfoControl(electricityAvailabilityLabel, ref zOrder);

                this.electricityMeter = GameObject.Instantiate<UISlider>(electricityPanel.Find<UISlider>("ElectricityMeter"));
                this.PositionInfoControl(this.electricityMeter, ref zOrder);
            }

            var waterPanel = this.uiParent.GetComponentInChildren<WaterInfoViewPanel>();

            // Set up water controls
            if (CityVitalsWatch.Settings.DisplayWaterAvailability) {
                var waterAvailabilityLabel = GameObject.Instantiate<UILabel>(waterPanel.Find<UILabel>("WaterAvailability"));
                this.PositionInfoControl(waterAvailabilityLabel, ref zOrder);

                this.waterMeter = GameObject.Instantiate<UISlider>(waterPanel.Find<UISlider>("WaterMeter"));
                this.PositionInfoControl(this.waterMeter, ref zOrder);
            }

            // Set up sewage controls
            if (CityVitalsWatch.Settings.DisplaySewageTreatment) {
                var sewageAvailabilityLabel = GameObject.Instantiate<UILabel>(waterPanel.Find<UILabel>("SewageAvailability"));
                this.PositionInfoControl(sewageAvailabilityLabel, ref zOrder);

                this.sewageMeter = GameObject.Instantiate<UISlider>(waterPanel.Find<UISlider>("SewageMeter"));
                this.PositionInfoControl(this.sewageMeter, ref zOrder);
            }

            var garbagePanel = this.uiParent.GetComponentInChildren<GarbageInfoViewPanel>();

            // Set up landfill controls
            if (CityVitalsWatch.Settings.DisplayLandfillUsage) {
                var landfillUsageLabel = GameObject.Instantiate<UILabel>(garbagePanel.Find<UILabel>("LandfillUsage"));
                this.PositionInfoControl(landfillUsageLabel, ref zOrder);

                this.landfillMeter = GameObject.Instantiate<UISlider>(garbagePanel.Find<UISlider>("LandfillMeter"));
                var landfillTexture = this.landfillMeter.Find<UITextureSprite>("LandfillGradient");
                landfillTexture.renderMaterial.SetColor("_ColorA", targetColor);
                landfillTexture.renderMaterial.SetColor("_ColorB", negativeColor);
                this.PositionInfoControl(this.landfillMeter, ref zOrder);
            }

            // Set up incineration controls
            if (CityVitalsWatch.Settings.DisplayIncinerationStatus) {
                var incinerationStatusLabel = GameObject.Instantiate<UILabel>(garbagePanel.Find<UILabel>("IncinerationStatus"));
                this.PositionInfoControl(incinerationStatusLabel, ref zOrder);

                this.incineratorMeter = GameObject.Instantiate<UISlider>(garbagePanel.Find<UISlider>("IncineratorMeter"));
                this.PositionInfoControl(this.incineratorMeter, ref zOrder);
            }

            var healthPanel = this.uiParent.GetComponentInChildren<HealthInfoViewPanel>();

            // Set up healthcare controls
            if (CityVitalsWatch.Settings.DisplayHealthcareAvailability) {
                var healthcareAvailabilityLabel = GameObject.Instantiate<UILabel>(healthPanel.Find<UILabel>("HealthcareAvaibility")); // NOTE: CO misspelled this
                this.PositionInfoControl(healthcareAvailabilityLabel, ref zOrder);

                this.healthcareMeter = GameObject.Instantiate<UISlider>(healthPanel.Find<UISlider>("HealthcareMeter"));
                this.PositionInfoControl(this.healthcareMeter, ref zOrder);
            }

            // Set up average health controls
            if (CityVitalsWatch.Settings.DisplayAverageHealth) {
                var averageHealthLabel = GameObject.Instantiate<UILabel>(healthPanel.Find<UILabel>("AvgHealth"));
                this.PositionInfoControl(averageHealthLabel, ref zOrder);

                this.averageHealthMeter = GameObject.Instantiate<UISlider>(healthPanel.Find<UISlider>("AvgHealthBar"));
                var averageHealthTexture = this.averageHealthMeter.Find<UITextureSprite>("Background");
                averageHealthTexture.renderMaterial.SetColor("_ColorA", negativeColor);
                averageHealthTexture.renderMaterial.SetColor("_ColorB", targetColor);
                this.PositionInfoControl(this.averageHealthMeter, ref zOrder);
            }

            // Set up cemetery controls
            if (CityVitalsWatch.Settings.DisplayCemeteryUsage) {
                var cemeteryUsageLabel = GameObject.Instantiate(healthPanel.Find<UILabel>("CemetaryUsage")); // NOTE: CO misspelled this
                this.PositionInfoControl(cemeteryUsageLabel, ref zOrder);

                this.cemeteryMeter = GameObject.Instantiate<UISlider>(healthPanel.Find<UISlider>("CemetaryMeter")); // NOTE: CO misspelled this
                var cemeteryTexture = this.cemeteryMeter.Find<UITextureSprite>("Background");
                cemeteryTexture.renderMaterial.SetColor("_ColorA", targetColor);
                cemeteryTexture.renderMaterial.SetColor("_ColorB", negativeColor);
                this.PositionInfoControl(this.cemeteryMeter, ref zOrder);
            }

            // Set up crematorium controls
            if (CityVitalsWatch.Settings.DisplayCrematoriumAvailability) {
                var crematoriumAvailabilityLabel = GameObject.Instantiate<UILabel>(healthPanel.Find<UILabel>("Incinerator"));
                this.PositionInfoControl(crematoriumAvailabilityLabel, ref zOrder);

                this.crematoriumMeter = GameObject.Instantiate<UISlider>(healthPanel.Find<UISlider>("DeathcareMeter"));
                this.PositionInfoControl(this.crematoriumMeter, ref zOrder);
            }

            var firePanel = this.uiParent.GetComponentInChildren<FireSafetyInfoViewPanel>();

            // Set up fire hazard controls
            if (CityVitalsWatch.Settings.DisplayFireHazard) {
                var fireHazardLabel = GameObject.Instantiate<UILabel>(firePanel.Find<UILabel>("SafetyLabel"));
                this.PositionInfoControl(fireHazardLabel, ref zOrder);

                this.fireMeter = GameObject.Instantiate<UISlider>(firePanel.Find<UISlider>("SafetyMeter"));
                var fireTexture = this.fireMeter.Find<UITextureSprite>("SafetyGradient");
                fireTexture.renderMaterial.SetColor("_ColorA", targetColor);
                fireTexture.renderMaterial.SetColor("_ColorB", negativeColor);
                this.PositionInfoControl(fireMeter, ref zOrder);
            }

            var crimePanel = this.uiParent.GetComponentInChildren<CrimeInfoViewPanel>();

            // Set up crime controls
            if (CityVitalsWatch.Settings.DisplayCrimeRate) {
                var crimeRateLabel = GameObject.Instantiate<UILabel>(crimePanel.Find<UILabel>("SafetyLabel"));
                this.PositionInfoControl(crimeRateLabel, ref zOrder);

                this.crimeMeter = GameObject.Instantiate<UISlider>(crimePanel.Find<UISlider>("SafetyMeter"));
                var crimeTexture = this.crimeMeter.Find<UITextureSprite>("SafetyGradient");
                crimeTexture.renderMaterial.SetColor("_ColorA", targetColor);
                crimeTexture.renderMaterial.SetColor("_ColorB", negativeColor);
                this.PositionInfoControl(this.crimeMeter, ref zOrder);
            }

            var educationPanel = this.uiParent.GetComponentInChildren<EducationInfoViewPanel>();

            // Set up elementary school controls
            if (CityVitalsWatch.Settings.DisplayElementarySchoolAvailability) {
                var elementarySchoolLabel = GameObject.Instantiate<UILabel>(educationPanel.Find<UILabel>("ElementaryAvailability"));
                this.PositionInfoControl(elementarySchoolLabel, ref zOrder);

                this.elementarySchoolMeter = GameObject.Instantiate<UISlider>(educationPanel.Find<UISlider>("ElementaryMeter"));
                this.PositionInfoControl(this.elementarySchoolMeter, ref zOrder);
            }

            // Set up high school controls
            if (CityVitalsWatch.Settings.DisplayHighSchoolAvailability) {
                var highSchoolLabel = GameObject.Instantiate<UILabel>(educationPanel.Find<UILabel>("HighAvailability"));
                this.PositionInfoControl(highSchoolLabel, ref zOrder);

                this.highSchoolMeter = GameObject.Instantiate<UISlider>(educationPanel.Find<UISlider>("HighMeter"));
                this.PositionInfoControl(this.highSchoolMeter, ref zOrder);
            }

            // Set up university controls
            if (CityVitalsWatch.Settings.DisplayUniversityAvailability) {
                var universityLabel = GameObject.Instantiate<UILabel>(educationPanel.Find<UILabel>("UnivAvailability"));
                this.PositionInfoControl(universityLabel, ref zOrder);

                this.universityMeter = GameObject.Instantiate<UISlider>(educationPanel.Find<UISlider>("UnivMeter"));
                this.PositionInfoControl(this.universityMeter, ref zOrder);
            }

            // Set up unemployment controls
            if (CityVitalsWatch.Settings.DisplayEmployment) {
                var employmentLabel = GameObject.Instantiate<UILabel>(healthPanel.Find<UILabel>("Incinerator"));
                employmentLabel.localeID = "STATS_9";
                this.PositionInfoControl(employmentLabel, ref zOrder);

                this.employmentMeter = GameObject.Instantiate<UISlider>(garbagePanel.Find<UISlider>("LandfillMeter"));
                var employmentTexture = this.employmentMeter.Find<UITextureSprite>("LandfillGradient");
                employmentTexture.renderMaterial.SetColor("_ColorA", negativeColor);
                employmentTexture.renderMaterial.SetColor("_ColorB", targetColor);
                this.PositionInfoControl(this.employmentMeter, ref zOrder);
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
            optionsButton.width = 25f;
            optionsButton.height = 25f;
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
            this.infoPanel.autoLayoutPadding = new RectOffset(8, 16, 0, 5);
            this.infoPanel.autoLayout = true;
            this.infoPanel.relativePosition = new Vector3(0f, TitleBarHeight + 5f);
            this.infoPanel.autoSize = true;
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
        private void PositionInfoControl(UIComponent control, ref int zOrder) {
            control.cachedTransform.parent = this.infoPanel.transform;
            control.autoSize = true;
            control.zOrder = zOrder;

            if (control is UISlider) {
                control.width = MeterWidth;
            }

            var panelHeightAdjustment = control.height + this.infoPanel.autoLayoutPadding.top + this.infoPanel.autoLayoutPadding.bottom;
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
            int elementarySchoolCapacity = 0;
            int elementarySchoolNeed = 0;
            int highSchoolCapacity = 0;
            int highSchoolNeed = 0;
            int universityCapacity = 0;
            int universityNeed = 0;
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
                healCapacity = info.GetHealCapacity();
                sickCount = info.GetSickCount();
                averageHealth = info.m_residentialData.m_finalHealth;
                deadCapacity = info.GetDeadCapacity();
                deadAmount = info.GetDeadAmount();
                cremateCapacity = info.GetCremateCapacity();
                deadCount = info.GetDeadCount();
                crimeRate = info.m_finalCrimeRate;
                elementarySchoolCapacity = info.GetEducation1Capacity();
                elementarySchoolNeed = info.GetEducation1Need();
                highSchoolCapacity = info.GetEducation2Capacity();
                highSchoolNeed = info.GetEducation2Need();
                universityCapacity = info.GetEducation3Capacity();
                universityNeed = info.GetEducation3Need();
                unemployment = info.GetUnemployment();
            }

            if (Singleton<ImmaterialResourceManager>.exists) {
                Singleton<ImmaterialResourceManager>.instance.CheckTotalResource(ImmaterialResourceManager.Resource.FireHazard, out fireHazard);
            }

            if (this.electricityMeter != null) {
                this.electricityMeter.value = this.GetPercentage(electricityCapacity, electricityConsumption);
                this.electricityMeter.tooltip = this.GetUsageString(electricityCapacity / 1000f, electricityConsumption / 1000f);
            }

            if (this.waterMeter != null) {
                this.waterMeter.value = this.GetPercentage(waterCapacity, waterConsumption);
                this.waterMeter.tooltip = this.GetUsageString(waterCapacity, waterConsumption);
            }

            if (this.sewageMeter != null) {
                this.sewageMeter.value = this.GetPercentage(sewageCapacity, sewageAccumulation);
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
                this.incineratorMeter.value = this.GetPercentage(incinerationCapacity, garbageAccumulation);
                this.incineratorMeter.tooltip = this.GetUsageString(incinerationCapacity, garbageAccumulation);
            }

            if (this.healthcareMeter != null) {
                this.healthcareMeter.value = this.GetPercentage(healCapacity, sickCount);
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
                this.crematoriumMeter.value = this.GetPercentage(cremateCapacity, deadCount);
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

            if (this.elementarySchoolMeter != null) {
                this.elementarySchoolMeter.value = this.GetPercentage(elementarySchoolCapacity, elementarySchoolNeed);
                this.elementarySchoolMeter.tooltip = this.GetUsageString(elementarySchoolCapacity, elementarySchoolNeed);
            }

            if (this.highSchoolMeter != null) {
                this.highSchoolMeter.value = this.GetPercentage(highSchoolCapacity, highSchoolNeed);
                this.highSchoolMeter.tooltip = this.GetUsageString(highSchoolCapacity, highSchoolNeed);
            }

            if (this.universityMeter != null) {
                this.universityMeter.value = this.GetPercentage(universityCapacity, universityNeed);
                this.universityMeter.tooltip = this.GetUsageString(universityCapacity, universityNeed);
            }

            if (this.employmentMeter != null) {
                this.employmentMeter.value = Mathf.Round(100f - unemployment);
                this.employmentMeter.tooltip = this.employmentMeter.value + "%";
            }
        }

        /// <summary>
        /// Calculates a percentage  based on the specified capacity and consumption values using Cities: Skylines' percentage algorithm.
        /// </summary>
        /// <param name="capacity">The capacity value.</param>
        /// <param name="consumption">The consumption value.</param>
        /// <returns>The percentage.</returns>
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