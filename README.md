# LightProbesPlacement
Unity script for placing lightprobes automatically.

Script to accelerate lightprobes placement in Unity ( tested in 2017.2 )

Based on script shared by ghostmantis games : http://ghostmantis.com/?p=332

This script allows you to place Lightproves automatically inside a volume, above geometry with collisions.
If you have a floor with collisions, it will detect it and place lightprobes above it on a grid with a resolution you can choose.
By default it will add 2 layers of probes above the detected meshes, or you can fill the volume completely.

This script will add a "Lighting" menu in Unity.

# How to use it :


create an empty gameobject
add a box collider to it
check the box "Is Trigger" ( we don't want to collide with it, just use the volume )
Set the size of the box to the size of the area you want to place lightprobes in ( if you have a ceiling set the vertical bounds lower to the ceiling, or it will spawn probes on top of it).
Go to lighting / Create Light Probes in Volume
Set the parameters : vertical and horizontal spacing ( one probe every X meters ), offset from floor is at which vertical distance from the collision you want to spawn the first layer of probes, fill volume enabled will fill the whole height of the volume, fill volume disabled will only spawn 2 layers of probes
Click the button !


# Improvements I would like to do :


- when detecting a collision only spawn lightprobes above static meshes (lightmap static ?)
- move the settings to a script on the volume, and use the lighting menu item to place lightprobes for all the volumes
- if you have volumes with high resolution LP grid inside volumes with smaller resolution LP grid, remove the low resolution grid probes before generating the high res grid.
