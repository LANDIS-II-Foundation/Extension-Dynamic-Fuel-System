using System;
using System.Collections.Generic;
using System.Linq;
//using System.Data;
using System.Text;
using Landis.Library.Metadata;
using Landis.Core;
using Edu.Wisc.Forest.Flel.Util;
using System.IO;
using Flel = Edu.Wisc.Forest.Flel;

namespace Landis.Extension.DynamicFuels
{
    public static class MetadataHandler
    {

        public static ExtensionMetadata Extension { get; set; }
        public static void InitializeMetadata(string mapNameTemplate, string pctConiferMapNameTemplate, string pctDeadFirMapNameTemplate)
        {

            ScenarioReplicationMetadata scenRep = new ScenarioReplicationMetadata()
            {
                RasterOutCellArea = PlugIn.ModelCore.CellArea,
                TimeMin = PlugIn.ModelCore.StartTime,
                TimeMax = PlugIn.ModelCore.EndTime,
            };

            Extension = new ExtensionMetadata(PlugIn.ModelCore)
            //Extension = new ExtensionMetadata()
            {
                Name = PlugIn.ExtensionName,
                TimeInterval = PlugIn.ModelCore.CurrentTime,
                ScenarioReplicationMetadata = scenRep
            };

            //---------------------------------------            
            //          map outputs:         
            //---------------------------------------

            OutputMetadata mapOut_fuel= new OutputMetadata()
            {
                Type = OutputType.Map,
                Name = "Fuel_Map",
                FilePath = MapNames.ReplaceTemplateVars(mapNameTemplate, PlugIn.ModelCore.CurrentTime),
                Map_DataType = MapDataType.Continuous,
                Visualize = true,
                //Map_Unit = "categorical",
            };
            Extension.OutputMetadatas.Add(mapOut_fuel);

            OutputMetadata mapOut_pctCon = new OutputMetadata()
            {
                Type = OutputType.Map,
                Name = "Percent_Connifer",
                FilePath = MapNames.ReplaceTemplateVars(pctConiferMapNameTemplate, PlugIn.ModelCore.CurrentTime),
                Map_DataType = MapDataType.Continuous,
                Visualize = true,
                //Map_Unit = "categorical",
            };
            Extension.OutputMetadatas.Add(mapOut_pctCon);

            OutputMetadata mapOut_pctFir = new OutputMetadata()
            {
                Type = OutputType.Map,
                Name = "Percent_Dead_Fir",
                FilePath = MapNames.ReplaceTemplateVars(pctDeadFirMapNameTemplate, PlugIn.ModelCore.CurrentTime),
                Map_DataType = MapDataType.Continuous,
                Visualize = true,
                //Map_Unit = "categorical",
            };
            Extension.OutputMetadatas.Add(mapOut_pctFir);

            //---------------------------------------
            MetadataProvider mp = new MetadataProvider(Extension);
            mp.WriteMetadataToXMLFile("Metadata", Extension.Name, Extension.Name);
        }
        public static void CreateDirectory(string path)
        {
            //Require.ArgumentNotNull(path);
            path = path.Trim(null);
            if (path.Length == 0)
                throw new ArgumentException("path is empty or just whitespace");

            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
            {
                Flel.Util.Directory.EnsureExists(dir);
            }

            //return new StreamWriter(path);
            return;
        }
    }
}
