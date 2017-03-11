//  Copyright 2007-2010 USFS Portland State University, Northern Research Station, University of Wisconsin
//  Authors:  Robert M. Scheller, Brian R. Miranda

using System.Collections.Generic;
using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.DynamicFuels
{
    //This slash type is used for all disturbance fuel types
    public interface IDisturbanceType
    {
        int FuelIndex {get;set;}
        int MaxAge {get;set;}
        List<string> PrescriptionNames{get;set;}
    }

    /// <summary>
    /// A forest type.
    /// </summary>
    public class DisturbanceType
        : IDisturbanceType
    {
        private int fuelIndex;
        private int maxAge;
        private List<string> prescriptionNames;

        //---------------------------------------------------------------------

        /// <summary>
        /// Index
        /// </summary>
        public int FuelIndex
        {
            get {
                return fuelIndex;
            }
            set {
                if (value <= 1 || value > 100)
                    throw new InputValueException(value.ToString(),"Value must be > 1 and <= 100.");
                fuelIndex = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Maximum cohort age.
        /// </summary>
        public int MaxAge
        {
            get {
                return maxAge;
            }
            set {
                if (value <= 0)
                    throw new InputValueException(value.ToString(),"Value must be > 0.");
                maxAge = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// A prescription name
        /// </summary>
        public List<string> PrescriptionNames
        {
            get {
                return prescriptionNames;
            }
            set {
                if (value != null)
                    prescriptionNames = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        public DisturbanceType()
        {
            prescriptionNames = new List<string>();
        }
        //---------------------------------------------------------------------
/*
        public DisturbanceType(int fuelIndex,
                            int maxAge,
                            List<string>  prescriptionNames)
        {
            this.fuelIndex = fuelIndex;
            this.maxAge = maxAge;
            this.prescriptionNames = prescriptionNames;
        }*/
    }
}
