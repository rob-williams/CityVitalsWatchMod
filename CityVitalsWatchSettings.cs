namespace CityVitalsWatch {

    using System.Collections.Generic;

    /// <summary>
    /// Represents the mod's settings, saved persistently through play sessions.
    /// </summary>
    public class CityVitalsWatchSettings {

        /// <summary>
        /// The positions of the panel and toggle button at different resolutions.
        /// </summary>
        public List<CityVitalsWatchResolution> Resolutions = new List<CityVitalsWatchResolution>();

        /// <summary>
        /// Indicates whether the panel should be visible by default.
        /// </summary>
        public bool DefaultPanelVisibility = true;

        /// <summary>
        /// Indicates whether the panel should be transparent when not hovered by the mouse cursor.
        /// </summary>
        public bool TransparentUnhovered = true;

        /// <summary>
        /// Indicates whether electricity availability should be displayed.
        /// </summary>
        public bool DisplayElectricityAvailability = true;

        /// <summary>
        /// Indicates whether water availability should be displayed.
        /// </summary>
        public bool DisplayWaterAvailability = true;

        /// <summary>
        /// Indicates whether sewage treatment should be displayed.
        /// </summary>
        public bool DisplaySewageTreatment = true;

        /// <summary>
        /// Indicates whether landfill usage should be displayed.
        /// </summary>
        public bool DisplayLandfillUsage = true;

        /// <summary>
        /// Indicates whether incineration status should be displayed.
        /// </summary>
        public bool DisplayIncinerationStatus = true;

        /// <summary>
        /// Indicates whether healthcare availability should be displayed.
        /// </summary>
        public bool DisplayHealthcareAvailability = false;

        /// <summary>
        /// Indicates whether average health should be displayed.
        /// </summary>
        public bool DisplayAverageHealth = false;

        /// <summary>
        /// Indicates whether cemetery usage should be displayed.
        /// </summary>
        public bool DisplayCemeteryUsage = true;

        /// <summary>
        /// Indicates whether crematorium availability should be displayed.
        /// </summary>
        public bool DisplayCrematoriumAvailability = true;

        /// <summary>
        /// Indicates whether fire hazard should be displayed.
        /// </summary>
        public bool DisplayFireHazard = false;

        /// <summary>
        /// Indicates whether crime rate should be displayed.
        /// </summary>
        public bool DisplayCrimeRate = false;

        /// <summary>
        /// Indicates whether jail availability should be displayed.
        /// </summary>
        public bool DisplayJailAvailability = false;

        /// <summary>
        /// Indicates whether elementary school availability should be displayed.
        /// </summary>
        public bool DisplayElementarySchoolAvailability = false;

        /// <summary>
        /// Indicates whether high school availability should be displayed.
        /// </summary>
        public bool DisplayHighSchoolAvailability = false;

        /// <summary>
        /// Indicates whether university availability should be displayed.
        /// </summary>
        public bool DisplayUniversityAvailability = false;

        /// <summary>
        /// Indicates whether employment rate should be displayed.
        /// </summary>
        public bool DisplayEmployment = true;

        /// <summary>
        /// Returns the <see cref="CityVitalsWatchResolution"/> instance corresponding to the specified screen width and height.
        /// </summary>
        /// <param name="screenWidth">The width of the screen.</param>
        /// <param name="screenHeight">The height of the screen.</param>
        /// <returns>
        /// The <see cref="CityVitalsWatchResolution"/> instance corresponding to the specified screen width and height.
        /// </returns>
        public CityVitalsWatchResolution GetResolutionData(int screenWidth, int screenHeight) {
            CityVitalsWatchResolution resolutionData = null;

            foreach (var resolution in this.Resolutions) {
                if (resolution.ScreenWidth == screenWidth && resolution.ScreenHeight == screenHeight) {
                    resolutionData = resolution;
                    break;
                }
            }

            if (resolutionData == null) {
                resolutionData = new CityVitalsWatchResolution();
                resolutionData.ScreenWidth = screenWidth;
                resolutionData.ScreenHeight = screenHeight;
                this.Resolutions.Add(resolutionData);
            }

            return resolutionData;
        }

        /// <summary>
        /// Determines whether the specified stat should be displayed.
        /// </summary>
        /// <param name="stat">The stat to check.</param>
        /// <returns>A value determining whether the specified stat should be displayed.</returns>
        public bool StatDisplayed(CityVitalsWatchStat stat) {
            switch (stat) {
                case CityVitalsWatchStat.ElectricityAvailability:
                    return this.DisplayElectricityAvailability;
                case CityVitalsWatchStat.WaterAvailability:
                    return this.DisplayWaterAvailability;
                case CityVitalsWatchStat.SewageTreatment:
                    return this.DisplaySewageTreatment;
                case CityVitalsWatchStat.LandfillUsage:
                    return this.DisplayLandfillUsage;
                case CityVitalsWatchStat.IncinerationStatus:
                    return this.DisplayIncinerationStatus;
                case CityVitalsWatchStat.HealthcareAvailability:
                    return this.DisplayHealthcareAvailability;
                case CityVitalsWatchStat.AverageHealth:
                    return this.DisplayAverageHealth;
                case CityVitalsWatchStat.CemeteryUsage:
                    return this.DisplayCemeteryUsage;
                case CityVitalsWatchStat.CrematoriumAvailability:
                    return this.DisplayCrematoriumAvailability;
                case CityVitalsWatchStat.FireHazard:
                    return this.DisplayFireHazard;
                case CityVitalsWatchStat.CrimeRate:
                    return this.DisplayCrimeRate;
                case CityVitalsWatchStat.JailAvailability:
                    return this.DisplayJailAvailability;
                case CityVitalsWatchStat.ElementarySchoolAvailability:
                    return this.DisplayElementarySchoolAvailability;
                case CityVitalsWatchStat.HighSchoolAvailability:
                    return this.DisplayHighSchoolAvailability;
                case CityVitalsWatchStat.UniversityAvailability:
                    return this.DisplayUniversityAvailability;
                case CityVitalsWatchStat.Employment:
                    return this.DisplayEmployment;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Sets whether the specified stat should be displayed.
        /// </summary>
        /// <param name="stat">The stat to set.</param>
        /// <param name="value">The value indicating whether the specified stat should be displayed.</param>
        public void SetStatDisplayed(CityVitalsWatchStat stat, bool value) {
            switch (stat) {
                case CityVitalsWatchStat.ElectricityAvailability:
                    this.DisplayElectricityAvailability = value;
                    break;
                case CityVitalsWatchStat.WaterAvailability:
                    this.DisplayWaterAvailability = value;
                    break;
                case CityVitalsWatchStat.SewageTreatment:
                    this.DisplaySewageTreatment = value;
                    break;
                case CityVitalsWatchStat.LandfillUsage:
                    this.DisplayLandfillUsage = value;
                    break;
                case CityVitalsWatchStat.IncinerationStatus:
                    this.DisplayIncinerationStatus = value;
                    break;
                case CityVitalsWatchStat.HealthcareAvailability:
                    this.DisplayHealthcareAvailability = value;
                    break;
                case CityVitalsWatchStat.AverageHealth:
                    this.DisplayAverageHealth = value;
                    break;
                case CityVitalsWatchStat.CemeteryUsage:
                    this.DisplayCemeteryUsage = value;
                    break;
                case CityVitalsWatchStat.CrematoriumAvailability:
                    this.DisplayCrematoriumAvailability = value;
                    break;
                case CityVitalsWatchStat.FireHazard:
                    this.DisplayFireHazard = value;
                    break;
                case CityVitalsWatchStat.CrimeRate:
                    this.DisplayCrimeRate = value;
                    break;
                case CityVitalsWatchStat.JailAvailability:
                    this.DisplayJailAvailability = value;
                    break;
                case CityVitalsWatchStat.ElementarySchoolAvailability:
                    this.DisplayElementarySchoolAvailability = value;
                    break;
                case CityVitalsWatchStat.HighSchoolAvailability:
                    this.DisplayHighSchoolAvailability = value;
                    break;
                case CityVitalsWatchStat.UniversityAvailability:
                    this.DisplayUniversityAvailability = value;
                    break;
                case CityVitalsWatchStat.Employment:
                    this.DisplayEmployment = value;
                    break;
            }
        }
    }
}
