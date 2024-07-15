using Unity.Entities;
using Unity.Mathematics;

namespace ECS
{
    public struct TransformData : IComponentData
    {
        public float2 Position;
        public float4x4 TransformMatrix;
    }
    
    public struct ResourceTagData : IComponentData
    {
        public float2 Position;
        public float4x4 TransformMatrix;
    }
    
    public struct ColonyTagData : IComponentData
    {
        public float2 Position;
        public float4x4 TransformMatrix;
    }
}