using Unity.Entities;

namespace ECS
{
    public struct IndexData : IComponentData
    {
        public int Idx;
        public int Ver;
    }
}