# 4025C_VR, V3 prospective features
 
## New Features Overview

- Main Area (MA), Assembly Area (AA)
- Teleport between areas using a "trigger" item
- Assemblies can be taken from AA to MA and vice versa
- Assemblies are "condensed" into code. In code form they are transported between areas, can be saved/loaded and re-assembled
- Save/load assemblies

## Main Area (MA)

<p align="center">
  <img src="images/4025-VR-V3-MA0.jpg" height="300">
</p>
<p align = "center">
  <sub>Fig.1 4025C-VR main work area</sub>
</p>

The Main Area (MA) is the default location for the VR user. After the application has launched the user is standing at one of the work tables. On the table are objects in the correct size relative to the user. The trigger object can be triggered and will teleport the user to the Assembly Area (AA).  

<p align="center">
  <img src="images/4025-VR-V3-TriggerObject.jpg" height="300">
</p>
<p align = "center">
  <sub>Fig.2 Trigger object</sub>
</p>

## Assembly Area (AA)

<p align="center">
  <img src="images/4025C-VR-V3-AssemblyArea.jpg" height="300">
</p>
<p align = "center">
  <sub>Fig.3 Assembly Area</sub>
</p>

After the user clicked the trigger object in MA we are teleported to the AA. In AA we find a library of components, scaled to 10x their normal size. In regular size many of these components would be very hard to work with - especially the small spherical connector nodes (they are 5mm in diameter in MA and 5cm in AA). Here assembly of structures works like in V2 of the application.

### Specifics

In V2 there was a single connector list (conList) for the whole Unity scene. This list contained all connector nodes deployed in the scene at any given time. Operations would add newly Instatiated nodes of remove deleted ones from the list as needed.

V3 will use the following mechanism:






