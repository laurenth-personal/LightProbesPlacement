# LightProbesPlacement
Unity script for placing lightprobes automatically.

Script to accelerate lightprobes placement in Unity ( tested in 2017.2 )

Based on script shared by ghostmantis games : http://ghostmantis.com/?p=332

This script allows you to place Lightprobes automatically inside a volume, above static geometry with collisions.
If you have a static floor and objects with collisions, it will detect them and place lightprobes above them on a grid with a resolution you can choose.
By default it will add 2 layers of probes above the detected meshes, or you can choose to fill the volume completely.

This script will add a "Lighting" menu in Unity.

# New :
	- volumes hold their respective settings, the menu item just refreshes the lightprobe placement for all the volumes
	- the script will only place lightprobes above static objects, if the script doesn't place any lightprobes, make sure your environment colliders are flagged as static

# How to use it :


- Create / Light / Lightprobe Volume
- Set the size of the box to the size of the area you want to place lightprobes in ( if you have a ceiling it is recommended to set the vertical bounds lower to the ceiling, or it will spawn probes on top of it).
- Set the " Light probe volume settings" : 
    - vertical and horizontal spacing ( one probe every X meters )
    - offset from floor is at which vertical distance from the collision you want to spawn the first layer of probes
    - number of layers is the number of probes that will be placed vertically above the hit collider
    - fill volume enabled will fill the whole height of the volume instead of just doing X number of layer
    - discard inside geometry will test if your probe is inside an object with collisions. This will only work if the top face of your volume is not itself inside an object with collisions.
	- Click the button !

- When you have several volumes setup in your scene and you want to refresh them all :
    - Go to lighting / Refresh lightprobes volumes. This will place again the probes in all your volumes in the scene.

# Improvements I would like to do :

- if you have volumes with high resolution LP grid inside volumes with smaller resolution LP grid, remove the low resolution grid probes before generating the high res grid.

# Troubleshoot :

- if the script doesn't place any lightprobe, make sure your geometric is marked as static, and that it has a collider. Using colliders isn't ideal but I haven't found a good solution that would work without them.
