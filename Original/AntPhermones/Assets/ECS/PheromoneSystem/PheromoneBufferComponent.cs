using Unity.Entities;
using UnityEngine;

namespace ECS
{
    public struct PheromoneBufferComponent : IBufferElementData
    {
        public Color Color;
    }
}