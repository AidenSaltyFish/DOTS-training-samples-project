using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace ECS
{
    public class ObstacleObject : MonoBehaviour
    {
        public int obstacleRingCount;
        public float obstaclesPerRing;
        public float obstacleRadius;

        public int mapSize;
        public int bucketResolution;

        public int instancesPerBatch;

        public Mesh obstacleMesh;
        public Material obstacleMaterial;

        private int _obstacleCount = 0;

        private void Start()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            var entityManager = world.EntityManager;

            // Each frame, the main thread creates a Temp allocator which it deallocates in its entirety at the end of the frame.
            // Temp allocations are only safe to use in the thread and the scope where they were allocated.
            // While you can make Temp allocations within a job, you can't pass main thread Temp allocations into a job.
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
            // EntityCommandBuffer.ParallelWriter ecbpw = ecb.AsParallelWriter();

            // Spawn most of the entities in a Burst job by cloning a pre-created prototype entity,
            // which can be either a Prefab or an entity created at run time like in this sample.
            // This is the fastest and most efficient way to create entities at run time.
            // var spawnObstaclesJob = new SpawnObstaclesJob
            // {
            //     Ecb = ecb.AsParallelWriter(),
            //     ObstacleRingCount = obstacleRingCount,
            //     ObstaclesPerRing = obstaclesPerRing,
            //     ObstacleRadius = obstacleRadius,
            //     MapSize = mapSize,
            //     InstancesPerBatch = instancesPerBatch
            // };
            //
            // var spawnJobHandle = spawnObstaclesJob.Schedule(m_h * m_w, 128);
            // spawnJobHandle.Complete();

            Unity.Mathematics.Random rand = new Unity.Mathematics.Random(12345);
            int curIdx = 0;
            for (int i = 1; i <= obstacleRingCount; i++)
            {
                float ringRadius = (i / (obstacleRingCount + 1f)) * (mapSize * .5f);
                float circumference = ringRadius * 2f * Mathf.PI;
                int maxCount = Mathf.CeilToInt(circumference / (2f * obstacleRadius) * 2f);
                int offset = rand.NextInt(0, maxCount);
                int holeCount = rand.NextInt(1, 3);
                for (int j = 0; j < maxCount; j++)
                {
                    float t = (float)j / maxCount;
                    if ((t * holeCount) % 1f < obstaclesPerRing)
                    {
                        float angle = (j + offset) / (float)maxCount * (2f * Mathf.PI);

                        float2 position = new float2(
                            mapSize * .5f + Mathf.Cos(angle) * ringRadius,
                            mapSize * .5f + Mathf.Sin(angle) * ringRadius);
                        float radius = obstacleRadius;

                        // AddComponent data doesn't set the component data if the component data already exists,
                        // it just returns earlier without modifying the component.
                        // Use AddComponent when the entity doesn't have the component and SetComponent when it does.
                        Entity spawnedEntity = ecb.CreateEntity();
                        ecb.AddComponent(spawnedEntity, new ObstacleData()
                        {
                            Position = position,
                            Radius = radius,
                            Matrix = float4x4.TRS(
                                new float3(position.x, position.y, 0),
                                Quaternion.identity,
                                new float3(radius * 2f, radius * 2f, 1f) / mapSize
                            ),
                            // batchIdx = curIdx / instancesPerBatch
                            Idx = curIdx,
                            PositionOnMap = (int2)(position / mapSize * bucketResolution)
                        });

                        ecb.AddComponent(spawnedEntity, new IndexData()
                        {
                            Idx = curIdx,
                            Ver = 0,
                        });

                        // entityCommandBuffer.SetComponent(spawnedEntity, new ObstacleData()
                        // {
                        //     position = position,
                        //     radius = radius,
                        //     batchIdx = curIdx / config.InstancesPerBatch
                        // });

                        curIdx++;

                        // Debug.DrawRay(obstacle.position / mapSize,-Vector3.forward * .05f,Color.green,10000f);
                    }
                }
            }

            ecb.Playback(entityManager);
            ecb.Dispose();
            // entityManager.DestroyEntity(prototype);

            _obstacleCount = curIdx;

            // The term "unmanaged type" is a little bit misleading: is not a type which is defined in unmanaged code.
            // It's rather a type which doesn't contain references managed by the garbage collector.
            ObstacleRenderSystem obstacleRenderDataSystem =
                World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ObstacleRenderSystem>();

            obstacleRenderDataSystem.OnRender += Render;
        }

        // private void Update()
        // {
        //     int batchCount = Mathf.CeilToInt((float)_obstacleCount / instancesPerBatch);
        //     NativeArray<Matrix4x4> _matrixNativeArray = 
        //         new NativeArray<Matrix4x4>(batchCount * instancesPerBatch, Allocator.Persistent);
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
        //     for (int i = 0; i < batchCount; i++)
        //     {
        //         NativeSlice<Matrix4x4> batchMatrixNativeSlice = 
        //             new NativeSlice<Matrix4x4>(matrixNativeArray, i * batchCount, batchCount);
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

        public void Render(object sender, System.EventArgs e) //, NativeArray<Matrix4x4> matrixNativeArray)
        {
            // NativeArray<Matrix4x4> matrixNativeArray = 
            //     new NativeArray<Matrix4x4>(batchCount * instancesPerBatch, Allocator.Persistent);
            
            NativeArray<Matrix4x4> matrixNativeArray = (e as RenderArgs).MatrixNativeArray;
            // Debug.Log(matrixNativeArray.Length);
            
            int batchCount = Mathf.CeilToInt((float)_obstacleCount / instancesPerBatch);
            
            for (int i = 0; i < batchCount; i++)
            {
                NativeSlice<Matrix4x4> batchMatrixNativeSlice = 
                    new NativeSlice<Matrix4x4>(matrixNativeArray, i * batchCount, instancesPerBatch);
                
                // Debug.Log(batchMatrixNativeSlice.Length);
                
                Graphics.DrawMeshInstanced(
                    obstacleMesh, 
                    0, 
                    obstacleMaterial,
                    batchMatrixNativeSlice.ToArray());
            }

            // matrixNativeArray.Dispose();
        }
    }
    
    [BurstCompile]
    public struct SpawnObstaclesJob : IJobParallelFor
    {
        public int ObstacleRingCount;
        public float ObstaclesPerRing;
        public float ObstacleRadius;

        public int MapSize;
        public int BucketResolution;

        public int InstancesPerBatch;
        
        public EntityCommandBuffer.ParallelWriter Ecb;
        
        public void Execute(int index)
        {
            // ObstacleConfigData config = SystemAPI.GetSingleton<ObstacleConfigData>();
    
            // EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            // EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(state.WorldUpdateAllocator);

            Unity.Mathematics.Random rand = new Unity.Mathematics.Random(12345);
            int curIdx = 0;
            for (int i = 1; i <= ObstacleRingCount; i++) 
            {
                float ringRadius = (i / (ObstacleRingCount + 1f)) * (MapSize * .5f);
                float circumference = ringRadius * 2f * Mathf.PI;
                int maxCount = Mathf.CeilToInt(circumference / (2f * ObstacleRadius) * 2f);
                int offset = rand.NextInt(0, maxCount);
                int holeCount = rand.NextInt(1, 3);
                for (int j = 0; j < maxCount; j++) 
                {
                    float t = (float)j / maxCount;
                    if ((t * holeCount) % 1f < ObstaclesPerRing) 
                    {
                        float angle = (j + offset) / (float)maxCount * (2f * Mathf.PI);
                        
                        float2 position = new float2(
                            MapSize * .5f + Mathf.Cos(angle) * ringRadius, 
                            MapSize * .5f + Mathf.Sin(angle) * ringRadius);
                        float radius = ObstacleRadius;
                        
                        // AddComponent data doesn't set the component data if the component data already exists,
                        // it just returns earlier without modifying the component.
                        // Use AddComponent when the entity doesn't have the component and SetComponent when it does.
                        Entity spawnedEntity = Ecb.CreateEntity(index);
                        Ecb.AddComponent(index, spawnedEntity, new ObstacleData() {
                            Position = position,
                            Radius = radius,
                            Matrix = float4x4.TRS(
                                new float3(position.x, position.y, 0), 
                                Quaternion.identity, 
                                new Vector3(radius * 2f,radius * 2f,1f) / MapSize
                                ),
                            Idx = curIdx / InstancesPerBatch
                        });
                        
                        // entityCommandBuffer.SetComponent(spawnedEntity, new ObstacleData()
                        // {
                        //     position = position,
                        //     radius = radius,
                        //     batchIdx = curIdx / config.InstancesPerBatch
                        // });

                        curIdx++;

                        // Debug.DrawRay(obstacle.position / mapSize,-Vector3.forward * .05f,Color.green,10000f);
                    }
                }
            }
        
            // entityCommandBuffer.Playback(state.EntityManager);
        }
    }

    // [BurstCompile]
    // public partial struct CalcMatricesJob : IJobEntity
    // {
    //     public NativeArray<Matrix4x4> MatrixNativeArray;
    //     
    //     public void Execute(ref ObstacleData data)
    //     {
    //         MatrixNativeArray[data.batchIdx] = data.matrix;
    //     }
    // }
}