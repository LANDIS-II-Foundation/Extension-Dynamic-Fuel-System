//  Copyright 2007-2010 USFS Portland State University, Northern Research Station, University of Wisconsin
//  Authors:  Robert M. Scheller, Brian R. Miranda

using System.Collections.Generic;

namespace Landis.Extension.DynamicFuels
{
    /// <summary>
    /// The parameters for the plug-in.
    /// </summary>
    public interface IInputParameters
    {
        /// <summary>
        /// Timestep (years)
        /// </summary>
        int Timestep
        {
            get;set;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Hardwood Maximum (percent)
        /// </summary>
        int HardwoodMax
        {
            get;set;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Dead fir maximum age (years)
        /// </summary>
        int DeadFirMaxAge
        {
            get;set;
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Fuel coefficients for species
        /// </summary>
        double[] FuelCoefficients
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The Fuel types that cells will be classified into.
        /// </summary>
        List<IFuelType> FuelTypes
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Disturbances that can force the conversion of a fuel type into another.
        /// </summary>
        List<IDisturbanceType> DisturbanceTypes
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Template for the filenames for Fuel maps.
        /// </summary>
        string MapFileNames
        {
            get;set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Template for the filenames for percent conifer maps.
        /// </summary>
        string PctConiferFileName
        {
            get;set;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for percent dead fir maps.
        /// </summary>
        string PctDeadFirFileName
        {
            get;set;
        }

    }
}
