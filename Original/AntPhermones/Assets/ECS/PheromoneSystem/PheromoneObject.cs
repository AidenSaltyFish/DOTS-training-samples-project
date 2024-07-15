using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace ECS
{
    public class PheromoneObject : MonoBehaviour
    {
        public int mapSize;

        private void Start()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            var entityManager = world.EntityManager;

            // Each frame, the main thread creates a Temp allocator which it deallocates in its entirety at the end of the frame.
            // Temp allocations are only safe to use in the thread and the scope where they were allocated.
            // While you can make Temp allocations within a job, you can't pass main thread Temp allocations into a job.
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
            // EntityCommandBuffer.ParallelWriter ecbpw = ecb.AsParallelWriter();
            
            // var entity = entityManager.CreateEntity();
            // DynamicBuffer<PheromoneBufferComponent> dynamicBuffer = ecb.AddBuffer<PheromoneBufferComponent>(entity);
            //
            // for (int i = 0; i < mapSize * mapSize; ++i)
            // {
            //     dynamicBuffer.Add(new PheromoneBufferComponent()
            //     {
            //         Color = Color.clear
            //     });
            // }
            
            // // for (int i = 0; i < mapSize * mapSize; ++i)
            // // {
            // //     Entity spawnedEntity = ecb.CreateEntity();
            // //     ecb.AddComponent(spawnedEntity, new PheromoneData()
            // //     {
            // //         Color = Color.clear,
            // //         Index = i
            // //     });
            // // }
            
            SpawnPheromonesJob spawnPheromonesJob = new SpawnPheromonesJob()
            {
                Ecb = ecb.AsParallelWriter(),
            };
            JobHandle handle = spawnPheromonesJob.Schedule(mapSize * mapSize, 128);
            handle.Complete();
            
            ecb.Playback(entityManager);
            ecb.Dispose();
        }
    }
    
    [BurstCompile]
    public struct SpawnPheromonesJob : IJobParallelFor
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        
        public void Execute(int index)
        {
            Entity spawnedEntity = Ecb.CreateEntity(index);
            Ecb.AddComponent(index, spawnedEntity, new PheromoneData()
            {
                Color = Color.clear,
                Index = index
            });
        }
    }
    
    // public struct PheromoneData : IComponentData
    // {
    //     public Color Color;
    //     public int Index;
    // }
}