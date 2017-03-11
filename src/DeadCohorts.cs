//  Copyright 2007-2010 USFS Portland State University, Northern Research Station, University of Wisconsin
//  Authors:  Robert M. Scheller, Brian R. Miranda

namespace Landis.Extension.DynamicFuels
{
    public class DeadCohorts
    {
        public int time;
        public int numCohorts;
        
        public DeadCohorts(int time, int numCohorts)
        {
            this.time = time;
            this.numCohorts = numCohorts;
        }
    }
}
