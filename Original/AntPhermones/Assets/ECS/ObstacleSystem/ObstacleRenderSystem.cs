using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ECS
{
    public partial class ObstacleRenderSystem : SystemBase
    {
        public event EventHandler OnRender;
        
        protected override void OnCreate()
        {
            RequireForUpdate<ObstacleConfigData>();
        }

        protected override void OnUpdate()
        {
            ObstacleConfigData config = SystemAPI.GetSingleton<ObstacleConfigData>();

            // EntityQuery query = GetEntityQuery(
            //     ComponentType.ReadOnly<ObstacleData>()
            // );
            
            // int obstacleCount = GetEntityQuery(ComponentType.ReadOnly<ObstacleData>())
            //     .CalculateEntityCount();

            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp);
            
            int obstacleCount = 
                builder.WithAny<ObstacleData>().Build(this).CalculateEntityCount();
            
            int batchCount = Mathf.CeilToInt((float)obstacleCount / config.InstancesPerBatch);
            // You must deallocate TempJob allocations within 4 frames of their creation.
            // For Native- collection types, the disposal safety checks throw an exception if a TempJob allocation lasts longer than 4 frames.
            NativeArray<Matrix4x4> matrixNativeArray =
                new NativeArray<Matrix4x4>(batchCount * config.InstancesPerBatch, Allocator.Persistent);
            
            // Entity Job
            CalcMatricesJob calcMatricesJob = new CalcMatricesJob()
            {
                MatrixNativeArray = matrixNativeArray
            };
            
            // JobHandle matrixJobHandle = calcMatricesJob.ScheduleParallel(new JobHandle());
            // matrixJobHandle.Complete();
            // Run = single thread on main thread
            // Schedule = single thread on worker thread (not on main thread)
            // ScheduleParallel = multi-threaded on worker threads
            Dependency = calcMatricesJob.ScheduleParallel(Dependency);
            Dependency.Complete();

            RenderArgs args = new RenderArgs(matrixNativeArray);

            // Debug.Log("Crap man");
            OnRender?.Invoke(config, args);
            // GameObject.Find("ObstacleObject").GetComponent<ObstacleObject>().Render(matrixNativeArray);

            matrixNativeArray.Dispose();
            builder.Dispose();
        }
    }
    
    // [BurstCompile]
    public partial struct CalcMatricesJob : IJobEntity
    {
        [WriteOnly] [NativeDisableParallelForRestriction]
        public NativeArray<Matrix4x4> MatrixNativeArray;
        
        public void Execute(ref ObstacleData data)
        {
            // int idx = data.batchIdx;
            // Matrix4x4 mat = data.matrix;
            MatrixNativeArray[data.Idx] = data.Matrix;
        }
    }
    
    public class RenderArgs : EventArgs
    {
        public NativeArray<Matrix4x4> MatrixNativeArray;

        public RenderArgs(NativeArray<Matrix4x4> array)
        {
            MatrixNativeArray = array;
        }
    }
}