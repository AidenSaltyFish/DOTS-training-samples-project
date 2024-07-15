using Unity.Entities;

namespace ECS
{
    public struct WorldConfigData : IComponentData
    {

        #region ANT DATA

        public int AntCount;
        public int AntSize;
        public float AntSpeed;
        public float AntAccel;
        
        public float PheromoneSteerStrength;
        public float WallSteerStrength;
        public float GoalSteerStrength;
        public float RandomSteering;
        public float OutwardStrength;
        public float InwardStrength;

        #endregion

        #region PHERO DATA

        public float TrailAddSpeed;
        public float TrailDecay;

        #endregion
        
        #region OBSTACLES

        public int ObstacleRingCount;
        public float ObstaclesPerRing;
        public float ObstacleRadius;

        #endregion

        #region WORLD DATA

        public int MapSize;
        public int BucketResolution;

        #endregion

        #region GPU INSTANCING

        public int InstancesPerBatch;

        #endregion
        
    }
}