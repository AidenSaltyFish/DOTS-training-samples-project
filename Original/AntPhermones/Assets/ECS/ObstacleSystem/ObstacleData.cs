using Unity.Entities;
using Unity.Mathematics;

namespace ECS
{
    public struct ObstacleData : IComponentData
    {
        
        #region OBSTACLE

        public float2 Position;
        public float Radius;

        #endregion

        #region GRID

        public int2 PositionOnMap;

        #endregion

        #region GPU INSTANCING

        public float4x4 Matrix;
        public int Idx;

        #endregion
        
    }
}