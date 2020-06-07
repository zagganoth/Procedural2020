# Brand new procedurally generated top down RPG
This is my attempt at a top-down procedurally generated game. It is currently very early in development.
# Currently has the following features
- Procedural Generation
  - Infinite world generated (currently) using perlin noise
  - Efficient chunk generation using Unity Jobs
  - Multiple biomes implemented as ScriptableObjects
  - Efficient serialization and loading of world save data using Protobuf
  - Support for structure saving and generation
- Robust inventory system
  - Item Auto-Pickup and redistribution in inventory
  - Dragging and dropping across all inventories
  - New items can be easily created by picking components and assigning values to them directly in-editor
- Building
  - Any item with a PlaceableComponent can be placed
  - Custom editor support for defining wall tiles, as unity Rule Tiles fell short
  - Intended support for designing custom structures - including outward appearance
  - Demo of current progress
  ![Gif2](../master/Captures/buildDemo.gif)
- Combat
  - Uses animator statemachines for extensible enemy behaviour
  - Support for many weapon types (currently has just bow and sword)
  - Small Demo: 
  ![Gif](../master/Captures/smallDemo.gif)
# Planned Features
- Crafting and Resource Gathering
  - Researching recipes using patterns and intuition
    - As you collect more items, crafting recipes that involve the item may reveal part of themselves
    - Once you have researched an item, quick craft it
- Reactive World
  - As you do more for the world, it continually gets more hostile
  - More details to come 
