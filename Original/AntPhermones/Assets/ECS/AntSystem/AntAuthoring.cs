using Unity.Entities;
using UnityEngine;

namespace ECS
{
    public class AntAuthoring : MonoBehaviour
    {
        public int antCount;
        public int antSize;
        public float antSpeed;
        public float antAccel;
        
        public float trailAddSpeed;

        public float pheromoneSteerStrength;
        public float wallSteerStrength;
        public float goalSteerStrength;
        public float randomSteering;
        public float outwardStrength;
        public float inwardStrength;
        
        public int mapSize;

        public int instancesPerBatch;

        private class Baker : Baker<AntAuthoring>
        {
            public override void Bake(AntAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new AntConfigData()
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
                    MapSize = authoring.mapSize,
                    InstancesPerBatch = authoring.instancesPerBatch,
                    TrailAddSpeed = authoring.trailAddSpeed
                });
            }
        }
    }
    
    public struct AntConfigData : IComponentData
    {
        public int AntCount;
        public int AntSize;
        public float AntSpeed;
        public float AntAccel;
        
        public float TrailAddSpeed;
        
        public float PheromoneSteerStrength;
        public float WallSteerStrength;
        public float GoalSteerStrength;
        public float RandomSteering;
        public float OutwardStrength;
        public float InwardStrength;
        
        public int MapSize;

        public int InstancesPerBatch;
    }
}