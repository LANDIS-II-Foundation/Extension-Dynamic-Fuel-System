//  Copyright 2007-2010 USFS Portland State University, Northern Research Station, University of Wisconsin
//  Authors:  Robert M. Scheller, Brian R. Miranda

using System.Collections.Generic;
using Landis.Core;
using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;

namespace Landis.Extension.DynamicFuels
{
    public class PlugIn
        : ExtensionMain
    {
        public static readonly ExtensionType type = new ExtensionType("disturbance:fuels");
        public static readonly string ExtensionName = "Dynamic Fuel System";
        private string mapNameTemplate;
        private string pctConiferMapNameTemplate;
        private string pctDeadFirMapNameTemplate;
        private IEnumerable<IFuelType> fuelTypes;
        private IEnumerable<IDisturbanceType> disturbanceTypes;
        
        private double[] fuelCoefs;
        private int hardwoodMax;
        private int deadFirMaxAge;
        private static IInputParameters parameters;
        private static ICore modelCore;


        //---------------------------------------------------------------------

        public PlugIn()
            : base(ExtensionName, type)
        {
        }
        
        //---------------------------------------------------------------------

        public static ICore ModelCore
        {
            get
            {
                return modelCore;
            }
        }
        
        //---------------------------------------------------------------------

        public override void LoadParameters(string dataFile, ICore mCore)
        {
            modelCore = mCore;
            SiteVars.Initialize();
            InputParameterParser parser = new InputParameterParser();
            parameters = Landis.Data.Load<IInputParameters>(dataFile, parser);
        }
        
        //---------------------------------------------------------------------

        public override void Initialize()
        {
            Timestep = parameters.Timestep;
            mapNameTemplate = parameters.MapFileNames;
            pctConiferMapNameTemplate = parameters.PctConiferFileName;
            pctDeadFirMapNameTemplate = parameters.PctDeadFirFileName;
            fuelTypes = parameters.FuelTypes;
            disturbanceTypes = parameters.DisturbanceTypes;
            fuelCoefs = parameters.FuelCoefficients;
            hardwoodMax = parameters.HardwoodMax;
            deadFirMaxAge = parameters.DeadFirMaxAge;
            MetadataHandler.InitializeMetadata(mapNameTemplate, pctConiferMapNameTemplate, pctDeadFirMapNameTemplate);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Runs the component for a particular timestep.
        /// </summary>
        /// <param name="currentTime">
        /// The current model timestep.
        /// </param>
        public override void Run()
        {
            if (SiteVars.TimeOfLastFire == null)
                SiteVars.ReInitialize();

            SiteVars.FuelType.ActiveSiteValues = 0;
            SiteVars.DecidFuelType.ActiveSiteValues = 0;

            modelCore.UI.WriteLine("  Calculating the Dynamic Fuel Type for all active cells...");
            foreach (ActiveSite site in modelCore.Landscape)
            {
                CalcFuelType(site, fuelTypes, disturbanceTypes);
                SiteVars.PercentDeadFir[site] = CalcPercentDeadFir(site);
            }

            string path = MapNames.ReplaceTemplateVars(mapNameTemplate, modelCore.CurrentTime);
            modelCore.UI.WriteLine("   Writing Fuel map to {0} ...", path);
            using (IOutputRaster<BytePixel> outputRaster = modelCore.CreateRaster<BytePixel>(path, modelCore.Landscape.Dimensions))
            {
                BytePixel pixel = outputRaster.BufferPixel;
                foreach (Site site in modelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                        pixel.MapCode.Value = (byte) ((int) SiteVars.FuelType[site] + 1);
                    else
                        pixel.MapCode.Value = 0;
                    outputRaster.WriteBufferPixel();
                }
            }

            string conpath = MapNames.ReplaceTemplateVars(pctConiferMapNameTemplate, modelCore.CurrentTime);
            modelCore.UI.WriteLine("   Writing % Conifer map to {0} ...", conpath);
            using (IOutputRaster<BytePixel> outputRaster = modelCore.CreateRaster<BytePixel>(conpath, modelCore.Landscape.Dimensions))
            {
                BytePixel pixel = outputRaster.BufferPixel;
                foreach (Site site in modelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                        pixel.MapCode.Value = (byte)((int)SiteVars.PercentConifer[site]);
                    else
                        pixel.MapCode.Value = 0;
                    outputRaster.WriteBufferPixel();
                }
            }
            string firpath = MapNames.ReplaceTemplateVars(pctDeadFirMapNameTemplate, modelCore.CurrentTime);
            modelCore.UI.WriteLine("   Writing % Dead Fir map to {0} ...", firpath);
            using (IOutputRaster<BytePixel> outputRaster = modelCore.CreateRaster<BytePixel>(firpath, modelCore.Landscape.Dimensions))
            {
                BytePixel pixel = outputRaster.BufferPixel;
                foreach (Site site in modelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                        pixel.MapCode.Value = (byte)((int)SiteVars.PercentDeadFir[site]);
                    else
                        pixel.MapCode.Value = 0;
                    outputRaster.WriteBufferPixel();
                }
            }
        }


        //---------------------------------------------------------------------

        private int CalcFuelType(Site site,
                                        IEnumerable<IFuelType> FuelTypes,
                                        IEnumerable<IDisturbanceType> DisturbanceTypes)
        {

            double[] forTypValue = new double[100];  //Maximum of 100 fuel types
            double sumConifer = 0.0;
            double sumDecid = 0.0;

            ISpeciesDataset SpeciesDataset = modelCore.Species;
            foreach(ISpecies species in SpeciesDataset)
            {

                // This is the new algorithm, based on where a cohort is within it's age range.
                // This algorithm is less biased towards older cohorts.
                ISpeciesCohorts speciesCohorts = SiteVars.Cohorts[site][species];

                if(speciesCohorts == null)
                    continue;

                foreach(IFuelType ftype in FuelTypes)
                {

                    if(ftype[species.Index] != 0)
                    {
                        double sppValue = 0.0;

                        foreach(ICohort cohort in speciesCohorts)
                        {
                            double cohortValue =0.0;


                            if(cohort.Age >= ftype.MinAge && cohort.Age <= ftype.MaxAge)
                            {
                                // Adjust max range age to the spp longevity
                                double maxAge = System.Math.Min(ftype.MaxAge, (double) species.Longevity);

                                // The fuel type range must be at least 5 years:
                                double ftypeRange = System.Math.Max(1.0, maxAge - (double) ftype.MinAge);

                                // The cohort age relative to the fuel type range:
                                double relativeCohortAge = System.Math.Max(1.0, (double) cohort.Age - ftype.MinAge);

                                cohortValue = relativeCohortAge / ftypeRange * fuelCoefs[species.Index];

                                // Use the one cohort with the largest value:
                                //sppValue += System.Math.Max(sppValue, cohortValue);  // A BUG, should be...
                                sppValue = System.Math.Max(sppValue, cohortValue);
                            }
                        }

                        if(ftype[species.Index] == -1)
                            forTypValue[ftype.FuelIndex] -= sppValue;
                        if(ftype[species.Index] == 1)
                            forTypValue[ftype.FuelIndex] += sppValue;
                    }
                }

            }

            int finalFuelType = 0;
            int decidFuelType = 0;
            double maxValue = 0.0;
            double maxDecidValue = 0.0;

            //Set the PERCENT CONIFER DOMINANCE:
            int coniferDominance = 0;
            int hardwoodDominance = 0;


            //First accumulate data for the BASE fuel types:
            foreach(IFuelType ftype in FuelTypes)
            {
                if(ftype != null)
                {

                    if ((ftype.BaseFuel == BaseFuelType.Conifer || ftype.BaseFuel == BaseFuelType.ConiferPlantation)
                        && forTypValue[ftype.FuelIndex] > 0)
                    {
                        sumConifer += forTypValue[ftype.FuelIndex];
                    }

                    //This is calculated for the mixed types:
                    if ((ftype.BaseFuel == BaseFuelType.Deciduous)
                        && forTypValue[ftype.FuelIndex] > 0)
                    {
                        sumDecid += forTypValue[ftype.FuelIndex];
                    }

                    if(forTypValue[ftype.FuelIndex] > maxValue)
                    {
                        maxValue = forTypValue[ftype.FuelIndex];
                        finalFuelType = ftype.FuelIndex;
                    }

                    if(ftype.BaseFuel == BaseFuelType.Deciduous && forTypValue[ftype.FuelIndex] > maxDecidValue)
                    {
                        maxDecidValue = forTypValue[ftype.FuelIndex];
                        decidFuelType = ftype.FuelIndex;
                    }

                }
            }

            // Next, use rules to modify the conifer and deciduous dominance:


            foreach(IFuelType ftype in FuelTypes)
            {
                if(ftype != null)
                {

                    if(ftype.FuelIndex == finalFuelType && ftype.BaseFuel == BaseFuelType.ConiferPlantation)
                    {
                        decidFuelType = 0;
                        sumConifer = 100;
                        sumDecid = 0;
                    }

                    // a SLASH type
                    else if(ftype.FuelIndex == finalFuelType && ftype.BaseFuel == BaseFuelType.Slash)
                    {
                        //maxValue = maxSlashValue;
                        //finalFuelType = slashFuelType;
                        //decidFuelType = 0;
                        sumConifer = 0;
                        sumDecid = 0;
                    }

            // an OPEN type
                    else if(ftype.FuelIndex == finalFuelType && ftype.BaseFuel == BaseFuelType.Open)
                    {
                        //maxValue = maxOpenValue;
                        //finalFuelType = openFuelType;
                        //decidFuelType = 0;
                        sumConifer = 0;
                        sumDecid = 0;
                    }

                }
            }
            //Set the PERCENT DOMINANCE values:
            if (sumConifer > 0 || sumDecid > 0)
            {
                coniferDominance = (int)((sumConifer / (sumConifer + sumDecid) * 100) + 0.5);
                hardwoodDominance = (int)((sumDecid / (sumConifer + sumDecid) * 100) + 0.5);
                if (hardwoodDominance < hardwoodMax)
                {
                    coniferDominance = 100;
                    hardwoodDominance = 0;
                }
                if (coniferDominance < hardwoodMax)
                {
                    coniferDominance = 0;
                    hardwoodDominance = 100;
                    finalFuelType = decidFuelType;
                }
            }

            //---------------------------------------------------------------------
            // Next check the disturbance types.  This will override any other existing fuel type.
            foreach(DisturbanceType slash in DisturbanceTypes)
            {
                //if (SiteVars.HarvestCohortsKilled != null && SiteVars.HarvestCohortsKilled[site] > 0)
                //{
                    if (SiteVars.TimeOfLastHarvest != null &&
                        (modelCore.CurrentTime - SiteVars.TimeOfLastHarvest[site] <= slash.MaxAge))
                    {
                        foreach (string pName in slash.PrescriptionNames)
                        {
                            if (SiteVars.HarvestPrescriptionName != null && SiteVars.HarvestPrescriptionName[site].Trim() == pName.Trim())
                            {
                                finalFuelType = slash.FuelIndex; //Name;
                                decidFuelType = 0;
                                coniferDominance = 0;
                                hardwoodDominance = 0;
                            }
                        }
                    }
                //}
                //Check for fire severity effects of fuel type
                if (SiteVars.FireSeverity != null && SiteVars.FireSeverity[site] > 0)
                {
                    if (SiteVars.TimeOfLastFire != null &&
                        (modelCore.CurrentTime - SiteVars.TimeOfLastFire[site] <= slash.MaxAge))
                    {
                        foreach (string pName in slash.PrescriptionNames)
                        {
                            if (pName.StartsWith("FireSeverity"))
                            {
                                if((pName.Substring((pName.Length - 1), 1)).ToString() == SiteVars.FireSeverity[site].ToString())
                                {
                                    finalFuelType = slash.FuelIndex; //Name;
                                    decidFuelType = 0;
                                    coniferDominance = 0;
                                    hardwoodDominance = 0;
                                }
                            }
                        }
                    }
                }
                //Check for wind severity effects of fuel type
                if (SiteVars.WindSeverity != null && SiteVars.WindSeverity[site] > 0)
                {
                    if (SiteVars.TimeOfLastWind != null &&
                        (modelCore.CurrentTime - SiteVars.TimeOfLastWind[site] <= slash.MaxAge))
                    {
                        foreach (string pName in slash.PrescriptionNames)
                        {
                            if (pName.StartsWith("WindSeverity"))
                            {
                                if ((pName.Substring((pName.Length - 1), 1)).ToString() == SiteVars.WindSeverity[site].ToString())
                                {
                                    finalFuelType = slash.FuelIndex; //Name;
                                    decidFuelType = 0;
                                    coniferDominance = 0;
                                    hardwoodDominance = 0;
                                }
                            }
                        }
                    }
                }
            }

            //Assign Percent Conifer:
            SiteVars.PercentConifer[site] = coniferDominance;
            SiteVars.PercentHardwood[site] = hardwoodDominance;

            SiteVars.FuelType[site] = finalFuelType;
            SiteVars.DecidFuelType[site] = decidFuelType;

            return finalFuelType;

        }

        // If BDA is running, then use that information to calculate the percent of all cohorts
        // that are dead fir cohorts.
        private int CalcPercentDeadFir(ActiveSite site)
        {

            int numDeadFir = 0;

            if(SiteVars.NumberDeadFirCohorts == null) // Is BDA even running?
                return 0;

            int minimumStartTime = System.Math.Max(0, SiteVars.TimeOfLastFire[site]);
            for(int i = minimumStartTime; i <= modelCore.CurrentTime; i++)
            {
                if(modelCore.CurrentTime - i <= deadFirMaxAge)

                    //if(SiteVars.NumberDeadFirCohorts[site][i] > 0)  // Only if a map actually exists
                    //    numDeadFir += SiteVars.NumberDeadFirCohorts[site][i];
                    if(SiteVars.NumberDeadFirCohorts[site].ContainsKey(i))
                        numDeadFir += SiteVars.NumberDeadFirCohorts[site][i];
            }

            int numSiteCohorts = 0;
            int percentDeadFir = 0;

            ISpeciesDataset SpeciesDataset = modelCore.Species;

            foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
                foreach (ICohort cohort in speciesCohorts)
                    numSiteCohorts++;


            percentDeadFir = (int) ( ((double) numDeadFir / (double) (numSiteCohorts + numDeadFir)) * 100.0 + 0.5);


            return System.Math.Min(percentDeadFir, 100);
        }

    }
}
