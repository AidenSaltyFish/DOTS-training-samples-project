using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS
{
    public partial struct ObstacleInitSystem : ISystem
    {
        // private bool _hasInit;
        // private int _batchCount;
        // private int _instancesPerBatch;
        //
        // public void OnCreate(ref SystemState state)
        // {
        //     state.RequireForUpdate<ObstacleConfigData>();
        //
        //     _hasInit = false;
        // }
        //
        // public void OnUpdate(ref SystemState state)
        // {
        //     if (!_hasInit)
        //     {
        //         ObstacleConfigData config = SystemAPI.GetSingleton<ObstacleConfigData>();
        //
        //         // EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        //         EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(state.WorldUpdateAllocator);
        //     
        //         Unity.Mathematics.Random rand = new Unity.Mathematics.Random(12345);
        //         int curIdx = 0;
        //         for (int i = 1; i <= config.ObstacleRingCount; i++) 
        //         {
        //             float ringRadius = (i / (config.ObstacleRingCount+1f)) * (config.MapSize * .5f);
        //             float circumference = ringRadius * 2f * Mathf.PI;
        //             int maxCount = Mathf.CeilToInt(circumference / (2f * config.ObstacleRadius) * 2f);
        //             int offset = rand.NextInt(0, maxCount);
        //             int holeCount = rand.NextInt(1, 3);
        //             for (int j = 0; j < maxCount; j++) 
        //             {
        //                 float t = (float)j / maxCount;
        //                 if ((t * holeCount) % 1f < config.ObstaclesPerRing) 
        //                 {
        //                     float angle = (j + offset) / (float)maxCount * (2f * Mathf.PI);
        //                     
        //                     float2 position = new float2(
        //                         config.MapSize * .5f + Mathf.Cos(angle) * ringRadius, 
        //                         config.MapSize * .5f + Mathf.Sin(angle) * ringRadius);
        //                     float radius = config.ObstacleRadius;
        //                     
        //                     // AddComponent data doesn't set the component data if the component data already exists,
        //                     // it just returns earlier without modifying the component.
        //                     // Use AddComponent when the entity doesn't have the component and SetComponent when it does.
        //                     Entity spawnedEntity = entityCommandBuffer.CreateEntity();
        //                     entityCommandBuffer.AddComponent(spawnedEntity, new ObstacleData() {
        //                         position = position,
        //                         radius = radius,
        //                         matrix = float4x4.TRS(
        //                             new float3(position.x, position.y, 0), 
        //                             Quaternion.identity, 
        //                             new float3(radius * 2f,radius * 2f,1f) / config.MapSize
        //                             ),
        //                         batchIdx = curIdx / config.InstancesPerBatch
        //                     });
        //                     
        //                     // entityCommandBuffer.SetComponent(spawnedEntity, new ObstacleData()
        //                     // {
        //                     //     position = position,
        //                     //     radius = radius,
        //                     //     batchIdx = curIdx / config.InstancesPerBatch
        //                     // });
        //     
        //                     curIdx++;
        //     
        //                     // Debug.DrawRay(obstacle.position / mapSize,-Vector3.forward * .05f,Color.green,10000f);
        //                 }
        //             }
        //         }
        //     
        //         entityCommandBuffer.Playback(state.EntityManager);
        //         
        //         _hasInit = true;
        //         _batchCount = curIdx + 1;
        //         _instancesPerBatch = config.InstancesPerBatch;
        //     }
        //     
        //     NativeArray<Matrix4x4> matrixNativeArray = 
        //         new NativeArray<Matrix4x4>(_batchCount * _instancesPerBatch, Allocator.Temp);
        //
        //     Material obstacleMaterial = new Material(Shader.Find("Standard"));
        //     
        //     // Entity Job
        //     CalcMatricesJob calcMatricesJob = new CalcMatricesJob()
        //     {
        //         MatrixNativeArray = matrixNativeArray
        //     };
        //     
        //     // JobHandle matrixJobHandle = calcMatricesJob.ScheduleParallel(new JobHandle());
        //     // matrixJobHandle.Complete();
        //     calcMatricesJob.ScheduleParallel();
        //
        //     for (int i = 0; i < _batchCount; i++)
        //     {
        //         NativeSlice<Matrix4x4> batchMatrixNativeSlice = 
        //             new NativeSlice<Matrix4x4>(matrixNativeArray, i * _batchCount, _batchCount);
        //         
        //         Graphics.DrawMeshInstanced(
        //             obstacleMesh, 
        //             0, 
        //             obstacleMaterial, 
        //             batchMatrixNativeSlice.ToArray());
        //     }
        //
        //     matrixNativeArray.Dispose();
        // }
    }
}
