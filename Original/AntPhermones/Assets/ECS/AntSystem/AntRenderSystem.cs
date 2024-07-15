using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ECS
{
    public partial class AntRenderSystem : SystemBase
    {
        public event EventHandler OnRender;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<AntConfigData>();
        }

        protected override void OnUpdate()
        {
            AntConfigData config = SystemAPI.GetSingleton<AntConfigData>();

            // EntityQuery query = GetEntityQuery(
            //     ComponentType.ReadOnly<ObstacleData>()
            // );
            
            // int obstacleCount = GetEntityQuery(ComponentType.ReadOnly<ObstacleData>())
            //     .CalculateEntityCount();

            // EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp);
            
            // int antCount = 
            //     builder.WithAny<AntData>().Build(this).CalculateEntityCount();

            int antCount = config.AntCount;
            
            int batchCount = Mathf.CeilToInt((float)antCount / config.InstancesPerBatch);
            // You must deallocate TempJob allocations within 4 frames of their creation.
            // For Native- collection types, the disposal safety checks throw an exception if a TempJob allocation lasts longer than 4 frames.
            NativeArray<Matrix4x4> matrixNativeArray =
                new NativeArray<Matrix4x4>(batchCount * config.InstancesPerBatch, Allocator.TempJob);
            
            // Entity Job
            CalcAntMatricesJob calcMatricesJob = new CalcAntMatricesJob()
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
            // builder.Dispose();
        }
        
        [BurstCompile]
        public partial struct CalcAntMatricesJob : IJobEntity
        {
            [WriteOnly] [NativeDisableParallelForRestriction]
            public NativeArray<Matrix4x4> MatrixNativeArray;
        
            public void Execute(ref AntData data)
            {
                MatrixNativeArray[data.Index] = data.Matrix;
            }
        }
    }
}