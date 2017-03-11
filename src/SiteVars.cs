//  Copyright 2007-2010 USFS Portland State University, Northern Research Station, University of Wisconsin
//  Authors:  Robert M. Scheller, Brian R. Miranda

using System.Collections.Generic;
using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;

namespace Landis.Extension.DynamicFuels
{

    ///<summary>
    /// Site Variables for a fuels plug-in.
    /// </summary>
    public static class SiteVars
    {
        private static ISiteVar<int> fuelType;
        private static ISiteVar<int> decidFuelType;
        private static ISiteVar<int> percentConifer;
        private static ISiteVar<int> percentHardwood;
        private static ISiteVar<int> percentDeadFir;
        private static ISiteVar<string> harvestPrescriptionName;
        private static ISiteVar<int> timeOfLastHarvest;
        private static ISiteVar<int> harvestCohortsKilled;
        private static ISiteVar<int> timeOfLastFire;
        private static ISiteVar<byte> fireSeverity;
        private static ISiteVar<int> timeOfLastWind;
        private static ISiteVar<byte> windSeverity; 
        private static ISiteVar<Dictionary<int,int>> numberDeadFirCohorts;

        private static ISiteVar<ISiteCohorts> cohorts;

        //---------------------------------------------------------------------

        public static void Initialize()
        {


            cohorts = PlugIn.ModelCore.GetSiteVar<ISiteCohorts>("Succession.AgeCohorts");

            fuelType     = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            decidFuelType   = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            percentConifer  = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            percentHardwood = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            percentDeadFir  = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            
            harvestPrescriptionName = PlugIn.ModelCore.GetSiteVar<string>("Harvest.PrescriptionName");
            timeOfLastHarvest       = PlugIn.ModelCore.GetSiteVar<int>("Harvest.TimeOfLastEvent");
            harvestCohortsKilled = PlugIn.ModelCore.GetSiteVar<int>("Harvest.CohortsDamaged");
            timeOfLastFire          = PlugIn.ModelCore.GetSiteVar<int>("Fire.TimeOfLastEvent");
            fireSeverity            = PlugIn.ModelCore.GetSiteVar<byte>("Fire.Severity");
            timeOfLastWind          = PlugIn.ModelCore.GetSiteVar<int>("Wind.TimeOfLastEvent");
            windSeverity            = PlugIn.ModelCore.GetSiteVar<byte>("Wind.Severity");
            numberDeadFirCohorts    = PlugIn.ModelCore.GetSiteVar<Dictionary<int,int>>("BDA.NumCFSConifers");

            PlugIn.ModelCore.RegisterSiteVar(SiteVars.FuelType, "Fuels.CFSFuelType");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.DecidFuelType, "Fuels.DecidFuelType");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.PercentConifer, "Fuels.PercentConifer");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.PercentHardwood, "Fuels.PercentHardwood");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.PercentDeadFir, "Fuels.PercentDeadFir");
        }
        public static void ReInitialize()
        {
            harvestPrescriptionName = PlugIn.ModelCore.GetSiteVar<string>("Harvest.PrescriptionName");
            timeOfLastHarvest = PlugIn.ModelCore.GetSiteVar<int>("Harvest.TimeOfLastEvent");
            harvestCohortsKilled = PlugIn.ModelCore.GetSiteVar<int>("Harvest.CohortsDamaged");
            timeOfLastFire = PlugIn.ModelCore.GetSiteVar<int>("Fire.TimeOfLastEvent");
            fireSeverity = PlugIn.ModelCore.GetSiteVar<byte>("Fire.Severity");
            timeOfLastWind = PlugIn.ModelCore.GetSiteVar<int>("Wind.TimeOfLastEvent");
            windSeverity = PlugIn.ModelCore.GetSiteVar<byte>("Wind.Severity");
            //numberDeadFirCohorts = PlugIn.ModelCore.GetSiteVar<int[]>("BDA.NumCFSConifers");
            numberDeadFirCohorts = PlugIn.ModelCore.GetSiteVar<Dictionary<int,int>>("BDA.NumCFSConifers");
           
        }
        //---------------------------------------------------------------------

        public static ISiteVar<int> FuelType
        {
            get {
                return fuelType;
            }
            set {
                fuelType = value;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<int> DecidFuelType
        {
            get {
                return decidFuelType;
            }
            set {
                decidFuelType = value;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<int> PercentConifer
        {
            get {
                return percentConifer;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<int> PercentHardwood
        {
            get {
                return percentHardwood;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<int> PercentDeadFir
        {
            get {
                return percentDeadFir;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<string> HarvestPrescriptionName
        {
            get {
                return harvestPrescriptionName;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<int> TimeOfLastHarvest
        {
            get {
                return timeOfLastHarvest;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<int> HarvestCohortsKilled
        {
            get {
                return harvestCohortsKilled;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<int> TimeOfLastFire
        {
            get
            {
                return timeOfLastFire;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<byte> FireSeverity
        {
            get
            {
                return fireSeverity;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<int> TimeOfLastWind
        {
            get
            {
                return timeOfLastWind;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<byte> WindSeverity
        {
            get
            {
                return windSeverity;
            }
        }
        //---------------------------------------------------------------------

        //public static ISiteVar<int[]> NumberDeadFirCohorts
        public static ISiteVar<Dictionary<int,int>> NumberDeadFirCohorts
        {
            get {
                return numberDeadFirCohorts;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<ISiteCohorts> Cohorts
        {
            get
            {
                return cohorts;
            }
        }
    }
}
