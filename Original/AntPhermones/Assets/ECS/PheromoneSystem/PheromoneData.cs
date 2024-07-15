using Unity.Entities;
using UnityEngine;

namespace ECS
{
    public struct PheromoneData : IComponentData
    {
        public Color Color;
        public int Index;
    }
}