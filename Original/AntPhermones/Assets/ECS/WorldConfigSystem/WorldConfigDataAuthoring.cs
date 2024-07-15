using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS
{
    public class WorldConfigDataAuthoring : MonoBehaviour
    {
        
        #region ANT DATA

        public int antCount;
        public int antSize;
        public float antSpeed;
        public float antAccel;
        
        public float pheromoneSteerStrength;
        public float wallSteerStrength;
        public float goalSteerStrength;
        public float randomSteering;
        public float outwardStrength;
        public float inwardStrength;

        #endregion

        #region PHERO DATA

        public float trailAddSpeed;
        public float trailDecay;

        #endregion
        
        #region OBSTACLES

        public int obstacleRingCount;
        public float obstaclesPerRing;
        public float obstacleRadius;

        #endregion

        #region WORLD DATA

        public int mapSize;
        public int bucketResolution;

        #endregion

        #region GPU INSTANCING

        public int instancesPerBatch;

        #endregion
        
        // public static WorldConfigDataAuthoring Instance { get; private set; }
        //
        // private void Awake() 
        // { 
        //     // If there is an instance, and it's not me, delete myself.
        //
        //     if (Instance != null && Instance != this) 
        //         Destroy(this); 
        //     else 
        //         Instance = this; 
        // }
        
        private class Baker : Baker<WorldConfigDataAuthoring>
        {
            public override void Bake(WorldConfigDataAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new WorldConfigData()
                {
                    AntCount = authoring.antCount,
                    AntSize = authoring.antSize,
                    AntSpeed = authoring.antSpeed,
                    AntAccel = authoring.antAccel,
                    
                    PheromoneSteerStrength = authoring.pheromoneSteerStrength,
                    WallSteerStrength = authoring.wallSteerStrength,
                    GoalSteerStrength = authoring.goalSteerStrength,
                    RandomSteering = authoring.randomSteering,
                    OutwardStrength = authoring.outwardStrength,
                    InwardStrength = authoring.inwardStrength,
                    
                    TrailAddSpeed = authoring.trailAddSpeed,
                    TrailDecay = authoring.trailDecay,
                    
                    ObstacleRingCount = authoring.obstacleRingCount,
                    ObstaclesPerRing = authoring.obstaclesPerRing,
                    ObstacleRadius = authoring.obstacleRadius,
                    
                    MapSize = authoring.mapSize,
                    BucketResolution = authoring.bucketResolution,
                    
                    InstancesPerBatch = authoring.instancesPerBatch,
                    
                    // ObstacleObject = GetEntity(authoring.obstacleObject, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}