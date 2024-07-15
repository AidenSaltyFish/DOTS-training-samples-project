using Unity.Entities;
using Unity.Mathematics;

namespace ECS
{
    public struct AntData : IComponentData
    {
        public float2 Position;
        public float FacingAngle;
        public float Speed;
        public bool HoldingResource;
        public float Brightness;

        public float4x4 Matrix;

        public int Index;
    }
}