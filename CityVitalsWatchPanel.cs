﻿using System;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

public class CityVitalsWatchPanel : UIPanel {

    private static float WidthScale;
    private static float HeightScale;
    private static float PanelWidth = 275f;
    private static float PanelHeight = 300f;
    private static float DistanceFromBottom = 417f;
    private static float ControlHeight = 25f;

    private UIView uiParent;
    private UIPanel infoPanel;

    private UISlider electricityMeter;
    private UISlider waterMeter;
    private UISlider sewageMeter;
    private UISlider landfillMeter;
    private UISlider incineratorMeter;

    public override void Start() {
        WidthScale = Screen.currentResolution.width / 1920f;
        HeightScale = Screen.currentResolution.height / 1080f;
        PanelWidth *= WidthScale;
        PanelHeight *= HeightScale;
        DistanceFromBottom *= HeightScale;
        ControlHeight *= HeightScale;
        foreach (var uiView in GameObject.FindObjectsOfType<UIView>()) {
            if (uiView.name == "UIView") {
                this.uiParent = uiView;
                this.transform.parent = this.uiParent.transform;
                this.relativePosition = new Vector3(Screen.currentResolution.width - PanelWidth,
                                                    Screen.currentResolution.height - DistanceFromBottom);
                break;
            }
        }

        base.Start();

        this.backgroundSprite = "MenuPanel";
        this.isVisible = true;
        this.canFocus = true;
        this.isInteractive = true;
        this.width = PanelWidth;
        this.height = PanelHeight;

        try {
            SetUpControls();
        }
        catch (Exception e) {
            GameObject.Destroy(this.gameObject);
            throw e;
        }
    }

    public override void Update() {
        base.Update();

        this.UpdateDisplay();

        // This is required to make sure the bottom of the panel covers all controls
        // incineratorMeter is the lowest control, so use that to test the bounds of the panel
        var yBottom = this.infoPanel.relativePosition.y + this.incineratorMeter.relativePosition.y + this.incineratorMeter.height + (10f * HeightScale);
        if (yBottom > this.position.y + this.height) {
            this.height = yBottom;
        }
    }

    private void SetUpControls() {
        // Create a drag handle for this panel
        var dragHandleObject = new GameObject("DragHandler");
        dragHandleObject.transform.parent = this.transform;
        dragHandleObject.transform.localPosition = Vector3.zero;
        var dragHandle = dragHandleObject.AddComponent<UIDragHandle>();
        dragHandle.width = this.width;
        dragHandle.height = 40f * HeightScale;
        dragHandle.zOrder = 1000;

        // Create a resize handle for this panel
        //var resizeHandleObject = new GameObject("Resize Handler");
        //resizeHandleObject.transform.parent = this.transform;
        //resizeHandleObject.transform.localPosition = Vector3.zero;
        //var resizeHandle = resizeHandleObject.AddComponent<UIResizeHandle>();
        //resizeHandle.width = this.width;
        //resizeHandle.height = this.height;
        //resizeHandle.zOrder = 1000;

        // Create a title for this panel
        var titleObject = new GameObject("Title");
        titleObject.transform.parent = this.transform;
        titleObject.transform.localPosition = Vector3.zero;
        var title = titleObject.AddComponent<UILabel>();
        title.text = "City Vitals";
        title.textAlignment = UIHorizontalAlignment.Center;

        var electricityPanel = this.uiParent.GetComponentInChildren<ElectricityInfoViewPanel>();

        // Grab the electricity panel title and copy its font
        var electricityTitle = electricityPanel.Find<UILabel>("Label");
        title.font = electricityTitle.font;
        title.cachedTransform.parent = this.cachedTransform;
        title.position = new Vector3((this.width / 2f) - (title.width / 2f), -10f * HeightScale, 0f);

        var controlPanelYPosition = title.position.y - (40f * HeightScale);

        // Create a sub-panel to auto-position the info controls
        var infoPanelObject = new GameObject("ControlPanel");
        infoPanelObject.transform.parent = this.transform;
        this.infoPanel = infoPanelObject.AddComponent<UIPanel>();
        this.infoPanel.transform.localPosition = Vector3.zero;
        this.infoPanel.width = this.width;
        this.infoPanel.height = this.height + (this.position.y - controlPanelYPosition);
        this.infoPanel.autoLayoutDirection = LayoutDirection.Vertical;
        this.infoPanel.autoLayoutStart = LayoutStart.TopLeft;
        var widthPadding = Mathf.RoundToInt(10 * WidthScale);
        this.infoPanel.autoLayoutPadding = new RectOffset(widthPadding, widthPadding * 2, 0, Mathf.RoundToInt(10 * HeightScale));
        this.infoPanel.autoLayout = true;
        this.infoPanel.position = new Vector3(0f, controlPanelYPosition);
        this.infoPanel.autoSize = true;

        int zOrder = 1;

        // Set up electricity controls
        GameObject electricityAvailabilityLabelObject = new GameObject("ElectricityAvailability");
        var electricityAvailabilityLabel = electricityAvailabilityLabelObject.AddComponent<UILabel>();
        this.CopyLabel(electricityPanel.Find<UILabel>("ElectricityAvailability"), electricityAvailabilityLabel);
        zOrder = this.SetUpInfoControl(electricityAvailabilityLabel, zOrder);

        this.electricityMeter = GameObject.Instantiate<UISlider>(electricityPanel.Find<UISlider>("ElectricityMeter"));
        zOrder = this.SetUpInfoControl(this.electricityMeter, zOrder);

        var waterPanel = this.uiParent.GetComponentInChildren<WaterInfoViewPanel>();

        // Set up water and sewage controls
        GameObject waterAvailabilityLabelObject = new GameObject("WaterAvailability");
        var waterAvailabilityLabel = waterAvailabilityLabelObject.AddComponent<UILabel>();
        this.CopyLabel(waterPanel.Find<UILabel>("WaterAvailability"), waterAvailabilityLabel);
        zOrder = this.SetUpInfoControl(waterAvailabilityLabel, zOrder);

        this.waterMeter = GameObject.Instantiate<UISlider>(waterPanel.Find<UISlider>("WaterMeter"));
        zOrder = this.SetUpInfoControl(this.waterMeter, zOrder);

        GameObject sewageAvailabilityLabelObject = new GameObject("SewageAvailability");
        var sewageAvailabilityLabel = sewageAvailabilityLabelObject.AddComponent<UILabel>();
        this.CopyLabel(waterPanel.Find<UILabel>("SewageAvailability"), sewageAvailabilityLabel);
        zOrder = this.SetUpInfoControl(sewageAvailabilityLabel, zOrder);

        this.sewageMeter = GameObject.Instantiate<UISlider>(waterPanel.Find<UISlider>("SewageMeter"));
        zOrder = this.SetUpInfoControl(this.sewageMeter, zOrder);

        var garbagePanel = this.uiParent.GetComponentInChildren<GarbageInfoViewPanel>();

        // Set up landfill and incineration controls
        GameObject landfillUsageLabelObject = new GameObject("LandfillUsage");
        var landfillUsageLabel = landfillUsageLabelObject.AddComponent<UILabel>();
        this.CopyLabel(garbagePanel.Find<UILabel>("LandfillUsage"), landfillUsageLabel);
        zOrder = this.SetUpInfoControl(landfillUsageLabel, zOrder);

        this.landfillMeter = GameObject.Instantiate<UISlider>(garbagePanel.Find<UISlider>("LandfillMeter"));
        zOrder = this.SetUpInfoControl(this.landfillMeter, zOrder);

        GameObject incinerationStatusLabelObject = new GameObject("IncinerationStatus");
        var incinerationStatusLabel = incinerationStatusLabelObject.AddComponent<UILabel>();
        this.CopyLabel(garbagePanel.Find<UILabel>("IncinerationStatus"), incinerationStatusLabel);
        zOrder = this.SetUpInfoControl(incinerationStatusLabel, zOrder);

        this.incineratorMeter = GameObject.Instantiate<UISlider>(garbagePanel.Find<UISlider>("IncineratorMeter"));
        zOrder = this.SetUpInfoControl(this.incineratorMeter, zOrder);
    }

    private int SetUpInfoControl(UIComponent control, int zOrder) {
        control.cachedTransform.parent = this.infoPanel.transform;
        control.height *= HeightScale;
        control.width *= WidthScale;
        control.autoSize = true;
        control.zOrder = zOrder;
        return zOrder + 1;
    }

    private void CopyLabel(UILabel source, UILabel target) {
        target.font = source.font;
        target.text = source.text;
    }

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

        if (Singleton<DistrictManager>.exists) {
            electricityCapacity = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetElectricityCapacity();
            electricityConsumption = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetElectricityConsumption();
            waterCapacity = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetWaterCapacity();
            waterConsumption = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetWaterConsumption();
            sewageCapacity = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetSewageCapacity();
            sewageAccumulation = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetSewageAccumulation();
            garbageCapacity = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetGarbageCapacity();
            garbageAmount = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetGarbageAmount();
            incinerationCapacity = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetIncinerationCapacity();
            garbageAccumulation = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetGarbageAccumulation();
        }

        this.electricityMeter.value = this.GetPercentage(electricityCapacity, electricityConsumption);
        this.waterMeter.value = this.GetPercentage(waterCapacity, waterConsumption);
        this.sewageMeter.value = this.GetPercentage(sewageCapacity, sewageAccumulation);
        if (garbageCapacity > 0) {
            this.landfillMeter.value = (garbageAmount / (float)garbageCapacity) * 100f;
        }
        else {
            this.landfillMeter.value = 0f;
        }
        this.incineratorMeter.value = this.GetPercentage(incinerationCapacity, garbageAccumulation);
    }

    private float GetPercentage(int capacity, int consumption, int consumptionMin = 45, int consumptionMax = 55) {
        /* This algorithm is what's used by the class InfoViewPanel to determine percentages displayed through the UI.
         * I'm unaware of the reasons for choosing the default values for consumptionMin and consumptionMax, but I
         * wanted to keep the logic consistent with the built-in UI sliders. The logic is the same but I cleaned it
         * up a bit.
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
}