using Unity.Collections;
using Unity.Entities;

namespace ECS
{
    public partial class PheromoneSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            // RequireForUpdate<WorldConfigData>();
        }

        protected override void OnUpdate()
        {
            // WorldConfigData config = SystemAPI.GetSingleton<WorldConfigData>();

            // BufferTypeHandle<PheromoneBufferComponent> pheroBufferTypeHandle = GetBufferTypeHandle<PheromoneBufferComponent>();
            // foreach (var elements in SystemAPI.Query<DynamicBuffer<PheromoneBufferComponent>>())
            // {
            //     foreach (var element in elements) 
            //     {
            //         Debug.Log(element);
            //     }
            // }

            // DynamicBuffer<PheromoneBufferComponent> pheromoneBuffer = SystemAPI.GetSingletonBuffer<PheromoneBufferComponent>();

            // UpdatePheromonesJob updatePheromonesJob = new UpdatePheromonesJob()
            // {
            //     TrailDecay = config.TrailDecay,
            // };
            //
            // Dependency = updatePheromonesJob.ScheduleParallel(Dependency);
            // Dependency.Complete();
        }
    }

    // public partial struct UpdatePheromonesJob : IJobEntity
    // {
    //     public float TrailDecay;
    //     
    //     public void Execute(ref PheromoneData data)
    //     {
    //         data.Color.r *= TrailDecay;
    //     }
    // }
    
    public partial struct UpdatePheromonesJob : IJobEntity
    {
        [ReadOnly] public BufferLookup<PheromoneBufferComponent> BufferLookup;
        
        public float TrailDecay;
        
        public void Execute()
        {
            
        }
    }
}