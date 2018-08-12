//  Authors:  Robert M. Scheller, Brian R. Miranda

using Landis.Utilities;

namespace Landis.Extension.DynamicFuels
{

    //NOTE:  M2, M4, and O1b excluded for this list.  This is because these types are
    //dependent upon season (leaf on or off), which is derived in the new fire extension.
    public enum BaseFuelType {Conifer, ConiferPlantation, Deciduous, NoFuel, Open, Slash};

    public interface IFuelType
    {
        int FuelIndex {get;set;}
        BaseFuelType BaseFuel {get;set;}
        int MinAge {get;set;}
        int MaxAge {get;set;}

        //---------------------------------------------------------------------

        /// <summary>
        /// Multiplier for a species
        /// </summary>
        int this[int speciesIndex]
        {
            get;set;
        }
    }

    /// <summary>
    /// A fuel type.
    /// </summary>
    public class FuelType
        : IFuelType
    {
        private int fuelIndex;
        private BaseFuelType baseFuel;
        private int minAge;
        private int maxAge;
        private int[] multipliers;

        //---------------------------------------------------------------------
        /// <summary>
        /// An index to the fuel type
        /// </summary>

        public int FuelIndex
        {
            get {
                return fuelIndex;
            }
            set {
                if (value < 1 || value > 100)
                    throw new InputValueException(value.ToString(),"Value must be between 1 and 100.");
                fuelIndex = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// The base fuel type - these are roughly based on the Canadian Fire Prediction system.
        /// </summary>

        public BaseFuelType BaseFuel
        {
            get {
                return baseFuel;
            }
            set {
                baseFuel = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Minimum cohort age.
        /// </summary>
        public int MinAge
        {
            get {
                return minAge;
            }
            set {
                if (value < 0)
                    throw new InputValueException(value.ToString(),"Value must be = or > 0.");
                minAge = value;
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
                if (value < 0)
                    throw new InputValueException(value.ToString(), "Value must be = or > 0.");
                maxAge = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Multiplier for a species
        /// </summary>
        public int this[int speciesIndex]
        {
            get {
                return multipliers[speciesIndex];
            }
            set {
                multipliers[speciesIndex] = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        public FuelType(int speciesCount)
        {
            multipliers = new int[speciesCount];
        }
        //---------------------------------------------------------------------

/*        public FuelType(
                            int fuelIndex,
                            BaseFuelType baseFuel,
                            int minAge,
                            int maxAge,
                          int[]  multipliers)
        {
            this.fuelIndex = fuelIndex;
            this.baseFuel = baseFuel;
            this.minAge = minAge;
            this.maxAge = maxAge;
            this.multipliers = multipliers;
        }*/
    }
}
