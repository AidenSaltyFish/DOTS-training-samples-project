using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS 
{
    public class ObstacleAuthoring : MonoBehaviour
    {
        public int ObstacleRingCount;
        public float ObstaclesPerRing;
        public float ObstacleRadius;
        
        public int MapSize;
        public int BucketResolution;
        
        public int InstancesPerBatch;

        // public GameObject obstacleObject;
        
        private class Baker : Baker<ObstacleAuthoring>
        {
            public override void Bake(ObstacleAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new ObstacleConfigData()
                {
                    ObstacleRingCount = authoring.ObstacleRingCount,
                    ObstaclesPerRing = authoring.ObstaclesPerRing,
                    ObstacleRadius = authoring.ObstacleRadius,
                    
                    MapSize = authoring.MapSize,
                    BucketResolution = authoring.BucketResolution,
                    
                    InstancesPerBatch = authoring.InstancesPerBatch,
                    
                    // ObstacleObject = GetEntity(authoring.obstacleObject, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    public struct ObstacleConfigData : IComponentData
    {

        #region OBSTACLES

        public int ObstacleRingCount;
        public float ObstaclesPerRing;
        public float ObstacleRadius;

        #endregion

        #region BUCKET

        public int MapSize;
        public int BucketResolution;
        // public int x;
        // public int y;

        #endregion

        #region GPU INSTANCING

        public int InstancesPerBatch;
        public int DataIdx;

        #endregion

        // #region GAMEOBJECT
        //
        // public GameObject ObstacleObject;
        //
        // #endregion

    }
    
    // public struct ObstacleData : IComponentData
    // {
    //     
    //     #region OBSTACLE
    //
    //     public float2 position;
    //     public float radius;
    //
    //     #endregion
    //
    //     #region GPU INSTANCING
    //
    //     public float4x4 matrix;
    //     public int batchIdx;
    //
    //     #endregion
    //     
    // }
}
