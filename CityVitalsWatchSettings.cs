namespace CityVitalsWatch {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;

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

        public bool DisplayElectricityAvailability = true;

        public bool DisplayWaterAvailability = true;

        public bool DisplaySewageTreatment = true;

        public bool DisplayLandfillUsage = true;

        public bool DisplayIncinerationStatus = true;

        public bool DisplayHealthcareAvailability = false;

        public bool DisplayAverageHealth = false;

        public bool DisplayCemeteryUsage = true;

        public bool DisplayCrematoriumAvailability = true;

        public bool DisplayFireHazard = false;

        public bool DisplayCrimeRate = false;

        public bool DisplayElementarySchoolAvailability = false;

        public bool DisplayHighSchoolAvailability = false;

        public bool DisplayUniversityAvailability = false;

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
