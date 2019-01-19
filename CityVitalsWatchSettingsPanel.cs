namespace CityVitalsWatch {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// The City Vitals Watch settings panel component.
    /// </summary>
    public class CityVitalsWatchSettingsPanel : UIPanel {

        /// <summary>
        /// Maps a <see cref="CityVitalsWatchStat"/> to the localization ID of the label used for its name.
        /// </summary>
        private static readonly Dictionary<CityVitalsWatchStat, string> StatLocaleIdMap = new Dictionary<CityVitalsWatchStat, string>
        {
            { CityVitalsWatchStat.ElectricityAvailability, "INFO_ELECTRICITY_AVAILABILITY" },
            { CityVitalsWatchStat.WaterAvailability, "INFO_WATER_WATERAVAILABILITY" },
            { CityVitalsWatchStat.SewageTreatment, "INFO_WATER_SEWAGEAVAILABILITY" },
            { CityVitalsWatchStat.LandfillUsage, "INFO_GARBAGE_LANDFILL" },
            { CityVitalsWatchStat.IncinerationStatus, "INFO_GARBAGE_INCINERATOR" },
            { CityVitalsWatchStat.HealthcareAvailability, "INFO_HEALTH_HEALTHCARE_AVAILABILITY" },
            { CityVitalsWatchStat.AverageHealth, "INFO_HEALTH_AVERAGE" },
            { CityVitalsWatchStat.CemeteryUsage, "INFO_HEALTH_CEMETARYUSAGE" },
            { CityVitalsWatchStat.CrematoriumAvailability, "INFO_HEALTH_CREMATORIUMAVAILABILITY" },
            { CityVitalsWatchStat.FireHazard, "INFO_FIRE_METER" },
            { CityVitalsWatchStat.CrimeRate, "INFO_CRIMERATE_METER" },
            { CityVitalsWatchStat.JailAvailability, "INFO_CRIME_JAIL_AVAILABILITY" },
            { CityVitalsWatchStat.ElementarySchoolAvailability, "INFO_EDUCATION_AVAILABILITY1" },
            { CityVitalsWatchStat.HighSchoolAvailability, "INFO_EDUCATION_AVAILABILITY2" },
            { CityVitalsWatchStat.UniversityAvailability, "INFO_EDUCATION_AVAILABILITY3" },
            { CityVitalsWatchStat.Employment, "STATS_9" },
            { CityVitalsWatchStat.JobAvailability, "STATS_10" },
        };

        private static readonly float PanelWidth = 215f;
        private static readonly float TitleBarHeight = 40f;

        private UIPanel controlPanel;
        private UICheckBox defaultVisibilityCheckBox;
        private UICheckBox transparentUnhoveredCheckBox;
        private Dictionary<CityVitalsWatchStat, UICheckBox> statControlMap;

        /// <summary>
        /// Initializes the panel's controls.
        /// </summary>
        /// <param name="titleFont">The font to use for the settings panel title.</param>
        /// <param name="labelTemplate">The template to use for all settings labels.</param>
        public void Initialize(UIFont titleFont, UILabel labelTemplate) {
            this.backgroundSprite = "MenuPanel";
            this.isVisible = false;
            this.canFocus = true;
            this.isInteractive = true;
            this.height = TitleBarHeight + 8f;
            this.width = PanelWidth;

            this.CreatePanelTitle(titleFont);
            this.CreateDragHandle();
            this.CreateCloseButton();
            this.CreateControlPanel();

            int zOrder = 1;
            
            this.defaultVisibilityCheckBox = this.CreateSettingsControl(
                CityVitalsWatch.Settings.DefaultPanelVisibility, "Default Visibility", true, labelTemplate, ref zOrder);
            this.transparentUnhoveredCheckBox = this.CreateSettingsControl(
                CityVitalsWatch.Settings.TransparentUnhovered, "Transparent Unhovered", true, labelTemplate, ref zOrder);

            this.statControlMap = new Dictionary<CityVitalsWatchStat, UICheckBox>();

            foreach (CityVitalsWatchStat stat in Enum.GetValues(typeof(CityVitalsWatchStat)).Cast<CityVitalsWatchStat>()) {
                this.statControlMap[stat] = this.CreateSettingsControl(
                    CityVitalsWatch.Settings.StatDisplayed(stat), StatLocaleIdMap[stat], false, labelTemplate, ref zOrder);
            }
        }

        /// <summary>
        /// Creates and positions a drag handle to allow the panel to be moved by its title bar.
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
        /// Creates and positions the panel's close button.
        /// </summary>
        private void CreateCloseButton() {
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
            closeButton.eventClick += OnCloseButtonClick;
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
            title.text = "City Vitals Settings";
            title.textAlignment = UIHorizontalAlignment.Center;
            title.font = font;
            title.position = new Vector3((this.width / 2f) - (title.width / 2f), -20f + (title.height / 2f));
        }

        /// <summary>
        /// Creates and positions the sub-panel to contain all settings controls.
        /// </summary>
        private void CreateControlPanel() {
            var controlPanelObject = new GameObject("ControlPanel");
            controlPanelObject.transform.parent = this.transform;
            this.controlPanel = controlPanelObject.AddComponent<UIPanel>();
            this.controlPanel.transform.localPosition = Vector3.zero;
            this.controlPanel.width = this.width;
            this.controlPanel.height = 0f;
            this.controlPanel.autoLayoutDirection = LayoutDirection.Vertical;
            this.controlPanel.autoLayoutStart = LayoutStart.TopLeft;
            this.controlPanel.autoLayoutPadding = new RectOffset(8, 16, 0, 20);
            this.controlPanel.autoLayout = true;
            this.controlPanel.relativePosition = new Vector3(0f, TitleBarHeight + 5f);
            this.controlPanel.autoSize = true;
        }

        /// <summary>
        /// Creates and positions a check box, sprite, and label.
        /// </summary>
        /// <param name="value">The value of the check box.</param>
        /// <param name="localeId">The localization ID of the label or a string if <paramref name="setRawText"/> is true.</param>
        /// <param name="setRawText">Indicates whether the text of the label should be set instead of the localization ID.</param>
        /// <param name="labelTemplate">The template to use for the label.</param>
        /// <param name="zOrder">The order index of the control used when automatically layout out the control.</param>
        /// <returns>The created check box control.</returns>
        private UICheckBox CreateSettingsControl(bool value, string localeId, bool setRawText, UILabel labelTemplate, ref int zOrder) {
            // First, create the check box for the setting
            GameObject checkBoxObject = new GameObject("CheckBox" + zOrder);
            checkBoxObject.transform.parent = this.controlPanel.transform;
            UICheckBox checkBox = checkBoxObject.AddComponent<UICheckBox>();
            checkBox.autoSize = true;
            checkBox.zOrder = zOrder;
            zOrder++;
            
            // Create the sprite displayed when the check box is unchecked and position it within the check box
            GameObject uncheckedObject = new GameObject("Unchecked");
            uncheckedObject.transform.parent = checkBox.transform;
            UISprite uncheckedSprite = uncheckedObject.AddComponent<UISprite>();
            uncheckedSprite.spriteName = "check-unchecked";
            uncheckedSprite.width = 16f;
            uncheckedSprite.height = 16f;
            uncheckedSprite.relativePosition = new Vector3(3f, 3f);

            // Create the sprite displayed when the check box is checked and position it within the check box
            GameObject checkedObject = new GameObject("Checked");
            checkedObject.transform.parent = uncheckedSprite.transform;
            UISprite checkedSprite = checkedObject.AddComponent<UISprite>();
            checkedSprite.spriteName = "check-checked";
            checkedSprite.width = 16f;
            checkedSprite.height = 16f;
            checkedSprite.relativePosition = Vector3.zero;
            checkBox.checkedBoxObject = checkedSprite;

            // Create the label to display the setting's name and position it within the check box
            GameObject labelObject = new GameObject("Label");
            labelObject.transform.parent = checkBox.transform;
            labelObject.transform.localPosition = Vector3.zero;
            UILabel label = labelObject.AddComponent<UILabel>();
            label.font = labelTemplate.font;
            label.textColor = labelTemplate.textColor;
            label.textScale = labelTemplate.textScale;

            if (setRawText) {
                label.text = localeId;
            }
            else {
                label.localeID = localeId;
            }

            label.relativePosition = new Vector3(25f, 4f);
            
            checkBox.isChecked = value;
            var panelHeightAdjustment = checkBox.height + this.controlPanel.autoLayoutPadding.top + this.controlPanel.autoLayoutPadding.bottom;
            this.height += panelHeightAdjustment;
            this.controlPanel.height += panelHeightAdjustment;

            return checkBox;
        }

        /// <summary>
        /// Called when the close button is clicked, saving settings and recreating the panel if any settings were changed
        /// and making the panel invisible if not.
        /// </summary>
        /// <param name="component">The close button component.</param>
        /// <param name="eventParam">The event parameters.</param>
        private void OnCloseButtonClick(UIComponent component, UIMouseEventParameter eventParam) {
            bool settingsChanged = false;

            if (this.defaultVisibilityCheckBox.isChecked != CityVitalsWatch.Settings.DefaultPanelVisibility) {
                CityVitalsWatch.Settings.DefaultPanelVisibility = this.defaultVisibilityCheckBox.isChecked;
                settingsChanged = true;
            }

            if (this.transparentUnhoveredCheckBox.isChecked != CityVitalsWatch.Settings.TransparentUnhovered) {
                CityVitalsWatch.Settings.TransparentUnhovered = this.transparentUnhoveredCheckBox.isChecked;
                settingsChanged = true;
            }

            foreach (CityVitalsWatchStat stat in Enum.GetValues(typeof(CityVitalsWatchStat)).Cast<CityVitalsWatchStat>()) {
                if (this.statControlMap[stat].isChecked != CityVitalsWatch.Settings.StatDisplayed(stat)) {
                    CityVitalsWatch.Settings.SetStatDisplayed(stat, this.statControlMap[stat].isChecked);
                    settingsChanged = true;
                }
            }

            this.isVisible = false;

            // If any settings were changed, recreate the panel
            if (settingsChanged) {
                CityVitalsWatchLoader.DestroyPanel();
                CityVitalsWatchLoader.CreatePanel();
            }
        }
    }
}
