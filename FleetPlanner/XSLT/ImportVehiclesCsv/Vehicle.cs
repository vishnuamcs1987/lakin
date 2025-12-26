using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportVehiclesCsv
{
    public class VehicleRecord
    {
        // String identifiers
        public string VehicleId { get; set; }        // VEH_ID (e.g., "faa")
        public string VehicleNo { get; set; }        // VEH_VEHICLENO (e.g., "103059")

        // Categorization
        public string TypeCategory { get; set; }     // VEH_TYPE_CATEGORY
        public string VehicleType { get; set; }      // VEH_TYPE

        // Financial and Costing (Nullable because of "NULL" in sample)
        public decimal? KmCost { get; set; }         // VEH_KMCOST
        public string CostProfile { get; set; }      // VEH_VEHICLE_COST_PROFILE

        // Route/Entity IDs (Nullable)
        public int? StartId { get; set; }            // EQY_START_ID
        public int? EndId { get; set; }              // EQY_END_ID

        // Capacities (Numeric)
        public double Capacity1 { get; set; }        // VEH_CAPACITY1
        public double Capacity2 { get; set; }        // VEH_CAPACITY2
        public double Capacity3 { get; set; }        // VEH_CAPACITY3

        // Qualifications (Nullable)
        public int? QualificationId { get; set; }    // VEH_QUALIFICATION_ID
        public int? QfQualificationId { get; set; }  // QF_QUALIFICATION_ID
    }
}
