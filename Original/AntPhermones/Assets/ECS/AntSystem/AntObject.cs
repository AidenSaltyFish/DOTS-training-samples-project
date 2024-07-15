using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace ECS
{
    public class AntObject : MonoBehaviour
    {
        public int antCount;
        public int antSize;
        
        public int mapSize;

        public Mesh antMesh;
        public Material antMaterial;

        public int instancesPerBatch;

        private void Start()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            var entityManager = world.EntityManager;
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            SpawnAntJob spawnAntJob = new SpawnAntJob()
            {
                Ecb = ecb.AsParallelWriter(),
                MapSize = mapSize,
                AntSize = antSize
            };

            JobHandle handle = spawnAntJob.Schedule(antCount, 128);
            handle.Complete();
            
            ecb.Playback(entityManager);
            ecb.Dispose();
            
            AntRenderSystem antRenderSystem =
                World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<AntRenderSystem>();

            antRenderSystem.OnRender += Render;
        }
        
        public void Render(object sender, System.EventArgs e) //, NativeArray<Matrix4x4> matrixNativeArray)
        {
            // NativeArray<Matrix4x4> matrixNativeArray = 
            //     new NativeArray<Matrix4x4>(batchCount * instancesPerBatch, Allocator.Persistent);
            
            NativeArray<Matrix4x4> matrixNativeArray = (e as RenderArgs).MatrixNativeArray;
            // Debug.Log(matrixNativeArray.Length);

            int batchCount = 1; // Mathf.CeilToInt((float)_obstacleCount / instancesPerBatch);
            
            for (int i = 0; i < batchCount; i++)
            {
                NativeSlice<Matrix4x4> batchMatrixNativeSlice = 
                    new NativeSlice<Matrix4x4>(matrixNativeArray, i * batchCount, instancesPerBatch);
                
                // Debug.Log(batchMatrixNativeSlice.Length);
                
                Graphics.DrawMeshInstanced(
                    antMesh, 
                    0, 
                    antMaterial,
                    batchMatrixNativeSlice.ToArray());
            }

            // matrixNativeArray.Dispose();
        }
    }

    [BurstCompile]
    public struct SpawnAntJob : IJobParallelFor
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public int MapSize;

        public float AntSize;
        
        public void Execute(int index)
        {
            Unity.Mathematics.Random rand = new Unity.Mathematics.Random(12345);
            
            Entity spawnedEntity = Ecb.CreateEntity(index);
            
            float2 position = new float2(
                rand.NextFloat(-5f, 5f) + MapSize * 0.5f,
                rand.NextFloat(-5f, 5f) + MapSize * 0.5f);

            float facingAngle = rand.NextFloat(0, 1f) * Mathf.PI * 2f;
            
            Ecb.AddComponent(index, spawnedEntity, new AntData()
            {
                Position = position,
                FacingAngle = facingAngle,
                Speed = 0f,
                HoldingResource = false,
                Brightness = rand.NextFloat(0.75f, 1.25f),
                
                Matrix = float4x4.TRS(
                    new float3(position.x, position.y, 0), 
                    Quaternion.Euler(0f,0f,facingAngle), 
                    AntSize),
                
                Index = index
            });
        }
    }

    // public struct AntData : IComponentData
    // {
    //     public float2 Position;
    //     public float FacingAngle;
    //     public float Speed;
    //     public bool HoldingResource;
    //     public float Brightness;
    //
    //     public float4x4 Matrix;
    //
    //     public int Index;
    // }
}