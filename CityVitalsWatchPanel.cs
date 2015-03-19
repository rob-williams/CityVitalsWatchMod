using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using UnityEngine;

public class CityVitalsWatchPanel : UIPanel {

    private static readonly float controlHeight = 25f;

    private UIView uiParent;

    private UISlider electricityMeter;
    private UISlider waterMeter;
    private UISlider sewageMeter;
    private UISlider landfillMeter;
    private UISlider incineratorMeter;

    public override void Start() {
        foreach (var uiView in GameObject.FindObjectsOfType<UIView>()) {
            if (uiView.name == "UIView") {
                this.uiParent = uiView;
                this.transform.parent = this.uiParent.transform;
                this.relativePosition = new Vector3(1620f, 663f);
                break;
            }
        }

        base.Start();

        this.backgroundSprite = "MenuPanel";
        this.isVisible = true;
        this.canFocus = true;
        this.isInteractive = true;
        this.width = 300;
        this.height = 300;

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
    }

    private void SetUpControls() {
        // Create a drag handle for this panel
        var dragHandleObject = new GameObject("DragHandler");
        dragHandleObject.transform.parent = this.transform;
        dragHandleObject.transform.localPosition = Vector3.zero;
        var dragHandle = dragHandleObject.AddComponent<UIDragHandle>();
        dragHandle.width = this.width;
        dragHandle.height = 40f;
        dragHandle.zOrder = 100;

        // Create a title for this panel
        var titleObject = new GameObject("Title");
        titleObject.transform.parent = this.transform;
        titleObject.transform.localPosition = Vector3.zero;
        var title = titleObject.AddComponent<UILabel>();
        title.text = "City Vitals";

        var electricityPanel = this.uiParent.GetComponentInChildren<ElectricityInfoViewPanel>();

        // Grab the electricity panel title and copy its font
        var electricityTitle = electricityPanel.Find<UILabel>("Label");
        title.font = electricityTitle.font;
        this.PositionControl(title, -10f);

        float yOffset = -50f;

        // Set up electricity controls
        GameObject electricityAvailabilityLabelObject = new GameObject("ElectricityAvailability");
        var electricityAvailabilityLabel = electricityAvailabilityLabelObject.AddComponent<UILabel>();
        this.CopyLabel(electricityPanel.Find<UILabel>("ElectricityAvailability"), electricityAvailabilityLabel);
        yOffset = this.PositionControl(electricityAvailabilityLabel, yOffset);

        this.electricityMeter = GameObject.Instantiate<UISlider>(electricityPanel.Find<UISlider>("ElectricityMeter"));
        yOffset = this.PositionControl(this.electricityMeter, yOffset);

        var waterPanel = this.uiParent.GetComponentInChildren<WaterInfoViewPanel>();

        // Set up water and sewage controls
        GameObject waterAvailabilityLabelObject = new GameObject("WaterAvailability");
        var waterAvailabilityLabel = waterAvailabilityLabelObject.AddComponent<UILabel>();
        this.CopyLabel(waterPanel.Find<UILabel>("WaterAvailability"), waterAvailabilityLabel);
        yOffset = this.PositionControl(waterAvailabilityLabel, yOffset);

        this.waterMeter = GameObject.Instantiate<UISlider>(waterPanel.Find<UISlider>("WaterMeter"));
        yOffset = this.PositionControl(this.waterMeter, yOffset);

        GameObject sewageAvailabilityLabelObject = new GameObject("SewageAvailability");
        var sewageAvailabilityLabel = sewageAvailabilityLabelObject.AddComponent<UILabel>();
        this.CopyLabel(waterPanel.Find<UILabel>("SewageAvailability"), sewageAvailabilityLabel);
        yOffset = this.PositionControl(sewageAvailabilityLabel, yOffset);

        this.sewageMeter = GameObject.Instantiate<UISlider>(waterPanel.Find<UISlider>("SewageMeter"));
        yOffset = this.PositionControl(this.sewageMeter, yOffset);

        var garbagePanel = this.uiParent.GetComponentInChildren<GarbageInfoViewPanel>();

        // Set up landfill and incineration controls
        GameObject landfillUsageLabelObject = new GameObject("LandfillUsage");
        var landfillUsageLabel = landfillUsageLabelObject.AddComponent<UILabel>();
        this.CopyLabel(garbagePanel.Find<UILabel>("LandfillUsage"), landfillUsageLabel);
        yOffset = this.PositionControl(landfillUsageLabel, yOffset);

        this.landfillMeter = GameObject.Instantiate<UISlider>(garbagePanel.Find<UISlider>("LandfillMeter"));
        yOffset = this.PositionControl(this.landfillMeter, yOffset);

        GameObject incinerationStatusLabelObject = new GameObject("IncinerationStatus");
        var incinerationStatusLabel = incinerationStatusLabelObject.AddComponent<UILabel>();
        this.CopyLabel(garbagePanel.Find<UILabel>("IncinerationStatus"), incinerationStatusLabel);
        yOffset = this.PositionControl(incinerationStatusLabel, yOffset);

        this.incineratorMeter = GameObject.Instantiate<UISlider>(garbagePanel.Find<UISlider>("IncineratorMeter"));
        yOffset = this.PositionControl(this.incineratorMeter, yOffset);
    }

    private float PositionControl(UIComponent control, float yOffset) {
        control.cachedTransform.parent = this.transform;
        DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, control.name + " - " + (control.cachedTransform.parent == null) + ", " + (control.cachedTransform.parent == this.transform));
        DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, control.name + " - width: " + control.width + ", height: " + control.height + ", yOffset: " + yOffset + ", relative position: " + control.position);
        control.position = new Vector3((this.width / 2f) - (control.width / 2f), yOffset, 0f);
        DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, control.name + " - width: " + control.width + ", height: " + control.height + ", yOffset: " + yOffset + ", relative position: " + control.position);
        return yOffset - controlHeight;
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