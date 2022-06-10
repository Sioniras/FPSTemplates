# Nighthawk FPS Templates
This is a collection of a few basic scripts that are useful when starting to learn game development with Unity.
They were originally developed to be used in a small game development course for kids, following this article series (danish only):

https://nighthawk.dk/article/introduktion-til-spiludvikling-med-unity

Please note that these scripts are not optimized for performance.

## Player script

There is no player script in the FPS Templates - it is recommended to use the first person player script from the Unity Standard Assets.
See the box on this page (text in danish, but there are links):

https://nighthawk.dk/article/i-gang-med-unity#playerscript

## Scripts

Here is an overview of some of the scripts - there is currently no complete documentation available.

### Game Controller

This is the main script controlling the game. **There should always be exactly one instance of this script in any scene using the FPS Templates.**
It is recommended to create an empty GameObject at the root of the scene which has only this script. Keep it at the top of the scene hierarchy to make it easy to find.

The instance of this script has all the settings for the FPS Templates.

### Door

The Door script was originally written for doors, but **can be used for so much more!**
It is basically just a script that can make things move or rotate, and can also be used for platforms rising from the ground or the walls,
opening the lid of a chest, an elevator, and so much more.

Doors may require keys to open, or can be opened by triggers - and it is when combined with triggers that the Door script really offers lots of possibilities!

Tip: A door consisting of several parts (e.g. sci-fi doors sliding to the sides) should use the Trigger script for opening,
since that allows all pieces to move together.

### Activator

This script allows a trigger to activate or deactivate a GameObject.

This could for example be used when having a key-pickup in a chest:
while the chest is closed, the key GameObject is not active in the scene and will not be picked when the player interacts with the chest in order to open it.
When using a trigger to open the chest (e.g. a Door script), the key could be activated by the same trigger, such that the player can pickup the key afterwards.

The Activator script can also be used to activate/deactivate traps, or to make part of the level (walls, floor, etc.) disappear and and other walls etc. appear,
thus changing the level layout.

### Trigger

The Trigger script makes it possible to interact with one or more Door and Activator scripts at the same time.

### UI scripts

The scripts in the UI folder are used together with the UI prefab.
