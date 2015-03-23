using System;
using System.Globalization;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

public class CityVitalsWatchPanel : UIPanel {

    private static float WidthScale;
    private static float HeightScale;
    private static float PanelWidth = 275f;
    private static float PanelHeight = 400f;
    private static float DistanceFromLeft = 10f;
    private static float DistanceFromTop = 65f;
    private static float ControlHeight = 25f;

    private bool previousContainsMouse = true;
    private UIView uiParent;
    private UIPanel infoPanel;

    private UISlider electricityMeter;
    private UISlider waterMeter;
    private UISlider sewageMeter;
    private UISlider landfillMeter;
    private UISlider incineratorMeter;
    private UISlider cemeteryMeter;
    private UISlider crematoriumMeter;

    public override void Start() {
        WidthScale = 1f;//Screen.currentResolution.width / 1920f;
        HeightScale = 1f;//Screen.currentResolution.height / 1080f;
        PanelWidth *= WidthScale;
        PanelHeight *= HeightScale;
        ControlHeight *= HeightScale;
        foreach (var uiView in GameObject.FindObjectsOfType<UIView>()) {
            if (uiView.name == "UIView") {
                this.uiParent = uiView;
                this.transform.parent = this.uiParent.transform;
                this.relativePosition = new Vector3(DistanceFromLeft, DistanceFromTop);
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

        if (this.previousContainsMouse != this.containsMouse) {
            this.previousContainsMouse = this.containsMouse;
            this.opacity = this.containsMouse ? 1f : 0.4f;
        }

        // This is required to make sure the bottom of the panel covers all controls
        // incineratorMeter is the lowest control, so use that to test the bounds of the panel
        //var yBottom = this.infoPanel.position.y + this.incineratorMeter.position.y + this.incineratorMeter.height + (10f * HeightScale);
        //if (yBottom > this.position.y + this.height) {
        //    //DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, yBottom + ", " + this.height + ", " + this.infoPanel.position.y + ", " + this.incineratorMeter.position.y + ", " + this.incineratorMeter.height);
        //    this.height = yBottom;
        //}

        //foreach (UIComponent control in this.infoPanel.GetComponentsInChildren<UIComponent>()) {
        //    if (control != this.infoPanel && !control.cachedTransform.parent.GetComponent<UISlider>()) {
        //        Vector3 controlPosition = control.position;
        //        controlPosition.x = (this.width / 2f) - (control.width / 2f);
        //        control.position = controlPosition;
        //    }
        //}

        this.UpdateDisplay();
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

        // Create a sub-panel to auto-position the info controls
        var infoPanelObject = new GameObject("ControlPanel");
        infoPanelObject.transform.parent = this.transform;
        this.infoPanel = infoPanelObject.AddComponent<UIPanel>();
        this.infoPanel.transform.localPosition = Vector3.zero;
        this.infoPanel.width = this.width;
        this.infoPanel.height = this.height - (40f * HeightScale);
        this.infoPanel.autoLayoutDirection = LayoutDirection.Vertical;
        this.infoPanel.autoLayoutStart = LayoutStart.TopLeft;
        var widthPadding = Mathf.RoundToInt(8 * WidthScale);
        this.infoPanel.autoLayoutPadding = new RectOffset(widthPadding, widthPadding * 2, 0, Mathf.RoundToInt(10 * HeightScale));
        this.infoPanel.autoLayout = true;
        this.infoPanel.relativePosition = new Vector3(0f, 45f * HeightScale);
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

        var healthPanel = this.uiParent.GetComponentInChildren<HealthInfoViewPanel>();

        // Set up cemetery and crematorium controls
        GameObject cemeteryUsageLabelObject = new GameObject("CemetaryUsage");
        var cemeteryUsageLabel = cemeteryUsageLabelObject.AddComponent<UILabel>();
        this.CopyLabel(healthPanel.Find<UILabel>("CemetaryUsage"), cemeteryUsageLabel);
        zOrder = this.SetUpInfoControl(cemeteryUsageLabel, zOrder);

        this.cemeteryMeter = GameObject.Instantiate<UISlider>(healthPanel.Find<UISlider>("CemetaryMeter"));
        zOrder = this.SetUpInfoControl(this.cemeteryMeter, zOrder);

        GameObject crematoriumAvailabilityLabelObject = new GameObject("Incinerator");
        var crematoriumAvailabilityLabel = crematoriumAvailabilityLabelObject.AddComponent<UILabel>();
        this.CopyLabel(healthPanel.Find<UILabel>("Incinerator"), crematoriumAvailabilityLabel);
        zOrder = this.SetUpInfoControl(crematoriumAvailabilityLabel, zOrder);

        this.crematoriumMeter = GameObject.Instantiate<UISlider>(healthPanel.Find<UISlider>("DeathcareMeter"));
        zOrder = this.SetUpInfoControl(this.crematoriumMeter, zOrder);
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
        target.color = source.color;
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
        int deadCapacity = 0;
        int deadAmount = 0;
        int cremateCapacity = 0;
        int deadCount = 0;

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
            deadCapacity = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetDeadCapacity();
            deadAmount = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetDeadAmount();
            cremateCapacity = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetCremateCapacity();
            deadCount = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetDeadCount();
        }

        this.electricityMeter.value = this.GetPercentage(electricityCapacity, electricityConsumption);
        this.electricityMeter.tooltip = string.Format(
            "{0} / {1}",
            Mathf.RoundToInt(electricityConsumption / 1000f),
            Mathf.RoundToInt(electricityCapacity / 1000f));
        this.waterMeter.value = this.GetPercentage(waterCapacity, waterConsumption);
        this.waterMeter.tooltip = string.Format("{0} / {1}",
            waterConsumption.ToString("#,#", CultureInfo.InvariantCulture),
            waterCapacity.ToString("#,#", CultureInfo.InvariantCulture));
        this.sewageMeter.value = this.GetPercentage(sewageCapacity, sewageAccumulation);
        this.sewageMeter.tooltip = string.Format(
            "{0} / {1}",
            sewageAccumulation.ToString("#,#", CultureInfo.InvariantCulture),
            sewageCapacity.ToString("#,#", CultureInfo.InvariantCulture));
        if (garbageCapacity > 0) {
            this.landfillMeter.value = (garbageAmount / (float)garbageCapacity) * 100f;
        }
        else {
            this.landfillMeter.value = 0f;
        }
        this.landfillMeter.tooltip = this.landfillMeter.value + "%";
        this.incineratorMeter.value = this.GetPercentage(incinerationCapacity, garbageAccumulation);
        this.incineratorMeter.tooltip = string.Format(
            "{0} / {1}",
            garbageAccumulation.ToString("#,#", CultureInfo.InvariantCulture),
            incinerationCapacity == 0 ? "0" : incinerationCapacity.ToString("#,#", CultureInfo.InvariantCulture));
        if (deadCapacity > 0) {
            this.cemeteryMeter.value = (deadAmount / (float)deadCapacity) * 100f;
        }
        else {
            this.cemeteryMeter.value = 0f;
        }
        this.cemeteryMeter.tooltip = this.cemeteryMeter.value + "%";
        this.crematoriumMeter.value = this.GetPercentage(cremateCapacity, deadCount);
        this.crematoriumMeter.tooltip = string.Format(
            "{0} / {1}",
            deadCount == 0 ? "0" : deadCount.ToString("#,#", CultureInfo.InvariantCulture),
            cremateCapacity == 0 ? "0" : cremateCapacity.ToString("#,#", CultureInfo.InvariantCulture));
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