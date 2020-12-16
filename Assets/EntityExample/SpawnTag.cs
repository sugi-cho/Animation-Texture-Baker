// tags for spawning things
//-------------------------------------------//

using Unity.Entities;

// any entities found with this tag will be spawned
[GenerateAuthoringComponent]
public struct SpawnTag : IComponentData { }

// any entity that is spawned will be given this tag
// (it's used for deleting spawns)
public struct SpawnedTag : IComponentData { }