# Exporting STL files from AutoCad

## Introduction

[AutoCAD](https://www.autodesk.com/products/autocad/overview?term=1-YEAR&tab=subscription) is used to create precise 2D drawings. It is my go-to application to create 2D geometry for laser cutting. 

<p align = "center">
<img src="images/acrylic-parts.png" hright="300"><img src="images/acrylic-parts_autocad.png" height="300">
</p>
<p align = "center">
  <sub>Fig.1 Laser-cut acrylic parts (left), same set of parts in AutoCAD (right)</sub>
</p>

AutoCAD can also be utilized to produce complex and precise 3D drawings for architecture and engineering. For many of my own projects I need to create 3D assets from existing 2D drawings. One particular purpose are 3D visualizations of IoT projects, before any laser cut is made. 

<p align="center">
  <img src="images/Amaria-rough.png" height="400"><img src="images/Amaria-V1-basic-structure.jpg" height="400">
</p>
<p align = "center">
  <sub>Fig.2 Partial assembly visualization (left), physical assembly (right)</sub>
</p>

In this case the 2D line drawing of a specific object in AutoCAD has to be transformed into a 3D object. This sounds like a relatively straight forward procedure; in a dedicated 3D application, such as Blender, one would use an *extrude* function to add "thickness" to a surface. Of course AutoCAD also has an *extrude* command and applied to a <strong>properly</strong> constructed 2D object it will yield useable results.

<p align="center">
  <img src="images/star_star_extruded.png" width="600">
</p>
<p align = "center">
  <sub>Fig.3 2D object (left), successfully extruded into 3D (right)</sub>
</p>

The process sounds logical and straight-forward, but I found shapes often do not extrude properly or at all. This has been a problem with AutoCAD files I received from another source. Upon "exploding" (unjoining) shapes they are often composed of subsections of many different types (*polylines*, *splines*, *arcs*, etc). Sometimes I have been able to find the single offending primitive and replace it with a *polyline*, enabling successful extrusion. Often it was quicker to draw a new *polyline* the existing shape as a template - this is a relatively quick process with a AutoCAD's snap settings.

## Rebuilding a shape using *polyline* (*pline*)


### Isolate individual connected parts and check for extrusion

<p align="center">
  <img src="images/AC-STL2.png" width="400">
</p>
<p align="center">
  <sub>The example shape is a 3-pronged star with a round hole at the center. If the star shape and the circle in the center are grouped/joined use *ungroup* or *explode* to dissolve.</sub>
</p>

Now we can verify that the center circle extrudes correctly. Not so the star shape.


### Use layers to set up template

Make a copy of your complete shape before continuing, and paste it somewhere in your workspace. I often find it easier to use a copy of my object rather than depending on AutoCAD's undo function, in case something goes wrong.

Assign the shape you intend to use as a template (here, the star shape) to another layer - my template layer uses white to draw the shape. Assign the circle shape to another layer, which you can make invisible.


### Use *polyline (pline)* over template

Use the *polyline/pline* command to draw a contiuous polyline following the template. Click to place control points - AutoCAD's snap settings will guide. Keep in mind that some of the larger arc segments will at first be represented by a shorter straight line.


### Verify that closed *pline* can extrude

<p align="center">
  <img src="images/AC-STL3.png" width="400">
</p>
<p align="center">
  <sub>Here a continuous polyline connecting all points.</sub>
</p>

<p align="center">
  <img src="images/AC-STL4.png" width="400">
</p>
<p align="center">
  <sub>Verify that the newly created polyline shape can extrude.</sub>
</p>

Do not apply the extrude. Make a copy of the rough polyline shape and paste it somewhere in your workspace.


### Convert straight segments to arcs

<p align="center">
  <img src="images/AC-STL5.png" width="400">
</p>
<p align="center">
  <sub>Using the appropriate handles transform straight segments into arcs then match shape of arc to template as needed.</sub>
</p>

<p align="center">
  <img src="images/AC-STL6.png" width="400">
</p>
<p align="center">
  <sub>Matching the template, with all required arcs.</sub>
</p>


### Verify that we can extrude

### Use (boolean) *subtract* command

## Troubleshooting

Sometimes even rebuild shapes still refuse to extrude. If this happens, scale the shape x10 and try again. After successful extrusion scale back down by x0.1.

## Export STL of extruded object







Closed polylines are the only 2D primitives which can be extruded properly with top and bottom surfaces.
Primitives such as arcs, lines will extrude to connected walls, but even if closed will not produce top and bottom surface
splines are not extruded in AutoCAD. 2D objects that are composed of different types of 2D primitives will not extrude correctly.
It may help to scale up objects by a factor of 10 before extrusion in AutoCAD. They can be scaled back down in the target application.
Such mixed 2D objects are best "redrawn" using polylines.

The trick is that the 2D source object must be composed of <strong>polylines</strong> (also called <strong>plines</strong>). 
