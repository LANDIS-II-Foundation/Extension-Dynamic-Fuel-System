Title:			README_DynamicFuels2.0.txt
Project:		LANDIS-II Landscape Change Model
Project Component:	Extension-Dynamic-Fuels
Component Deposition:	https://github.com/LANDIS-II-Foundation/Extension-Dynamic-Fuels
Author:			LANDIS-II Foundation
Origin Date:		30 Apr 2017
Final Date:		


The purpose of the Extension-Dynamic-Fuels repository is SOLELY to produce the 
Landis.Extension.DynamicFuels.dll, a necessary library for the "Dynamic Fire Fuel System".  

The "Dynamic Fire Fuel System" requires two libraries:
1) Landis.Extension.DynamicFire.dll
2) Landis.Extension.DynamicFuels.dll

The Landis.Extension.DynamicFuels.dll must be built from this repository and then ADDED to the 
/src/bin/debug/ directory of the Extension-Dynamic-Fire repository in order to install 
BOTH Landis.Extension.DynamicFuels.dll and Landis.Extension.DynamicFire.dll as the 
"Dynamic Fire Fuel System" extension w/ one installer, "Dynamic Fire Fuel System 3.0.iss".
The Dynamic Fire Fuel System 3.0.iss installer is located in the Extension-Dynamic-Fire repository.

