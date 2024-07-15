using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ECS
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class AntSystem : SystemBase
    {
	    private EntityQuery _obstacleDataQuery;
	    private EntityQuery _pheromoneDataQuery;
	    private EntityQuery _antDataQuery;
	    
	    protected override void OnCreate()
	    {
		    base.OnCreate();
		    RequireForUpdate<AntConfigData>();

		    // _obstacleDataQuery = new EntityQueryBuilder(Allocator.TempJob).WithAll<ObstacleData, IndexData>().Build(this);
		    // _pheromoneDataQuery = new EntityQueryBuilder(Allocator.TempJob).WithAll<PheromoneData, IndexData>().Build(this);

		    _obstacleDataQuery = GetEntityQuery(typeof(ObstacleData));
		    _pheromoneDataQuery = GetEntityQuery(typeof(PheromoneData));
		    _antDataQuery = GetEntityQuery(typeof(AntData));
	    }

	    private NativeArray<ObstacleData> _obstacleDataArray;
	    private NativeArray<PheromoneData> _pheromoneDataArray;

	    protected override void OnUpdate()
        { 
	        WorldConfigData config = SystemAPI.GetSingleton<WorldConfigData>();
	        
	        ResourceTagData resourceData = SystemAPI.GetSingleton<ResourceTagData>();
	        ColonyTagData colonyData = SystemAPI.GetSingleton<ColonyTagData>();
	        
	        // Unity.Mathematics.Random rand = new Unity.Mathematics.Random(1234567);

	        // _antDataQuery = new EntityQueryBuilder(Allocator.TempJob).WithAll<AntData>().Build(this);

	        _obstacleDataArray =
		        CollectionHelper.CreateNativeArray<ObstacleData, RewindableAllocator>(
			        _obstacleDataQuery.CalculateEntityCount(), ref World.UpdateAllocator);

	        ObstacleArrayJob obstacleArrayJob = new ObstacleArrayJob()
	        {
				ObstacleDataArray = _obstacleDataArray
	        };

	        JobHandle oHandle = obstacleArrayJob.ScheduleParallel(_obstacleDataQuery, new JobHandle());
	        
	        _pheromoneDataArray =
		        CollectionHelper.CreateNativeArray<PheromoneData, RewindableAllocator>(
			        _pheromoneDataQuery.CalculateEntityCount(), ref World.UpdateAllocator);
	        
	        PheromoneArrayJob pheromoneArrayJob = new PheromoneArrayJob()
	        {
		        PheromoneDataArray = _pheromoneDataArray
	        };

	        JobHandle pHandle = pheromoneArrayJob.ScheduleParallel(_pheromoneDataQuery, new JobHandle());
	        JobHandle cHandle = JobHandle.CombineDependencies(oHandle, pHandle);
	        cHandle.Complete();
	        
	        AntJob antJob = new AntJob()
	        {
		        Config = config,
		        ResourcePosition = resourceData,
		        ColonyPosition = colonyData,
		        ObstacleDataArray = _obstacleDataArray,
		        PheromoneDataArray = _pheromoneDataArray,
		        // PheromoneQuery = _pheromoneDataQuery,
		        // ObstacleQuery = _obstacleDataQuery
	        };

	        Dependency = antJob.ScheduleParallel(_antDataQuery, cHandle);
	        Debug.Log(_pheromoneDataArray.Length);
        }
        
        private bool Linecast(float2 point1, float2 point2) 
        {
	        // float dx = point2.x - point1.x;
	        // float dy = point2.y - point1.y;
	        // float dist = Mathf.Sqrt(dx * dx + dy * dy);
	        //
	        // int stepCount = Mathf.CeilToInt(dist*.5f);
	        // for (int i=0;i<stepCount;i++) {
		       //  float t = (float)i / stepCount;
		       //  if (GetObstacleBucket(point1.x+dx*t,point1.y+dy*t).Length>0) 
		       //  {
			      //   return true;
		       //  }
	        // }

	        return false;
        }
    }

    [BurstCompile]
    public partial struct AntJob : IJobEntity
    {
	    public WorldConfigData Config;
	    public ResourceTagData ResourcePosition;
	    public ColonyTagData ColonyPosition;

	    [ReadOnly]
	    public NativeArray<PheromoneData> PheromoneDataArray;
	    // public QueryEnumerable<RefRO<ObstacleData>> ObstacleDatas;
	    // [ReadOnly]
	    public NativeArray<ObstacleData> ObstacleDataArray;

	    // public EntityQuery PheromoneQuery;
	    // public EntityQuery ObstacleQuery;
	    
	    // public TransformData ResourcePosition;
	    // public TransformData ColonyPosition;
	    
	    public void Execute([EntityIndexInQuery] int entityIndexInQuery, ref AntData ant)
	    {
		    Unity.Mathematics.Random rand = new Unity.Mathematics.Random(12345);
		    
		    float targetSpeed = Config.AntSpeed;
	   
			ant.FacingAngle += rand.NextFloat(-Config.RandomSteering, Config.RandomSteering);
   
			// float pheroSteering = PheromoneSteering(ant, 3f);
			
			// Phero Steering
			float pheroSteeringOutput = 0;
			float pheroSteeringDistance = 3f;
			
			// Wall Steering
			int wallSteeringOutput = 0;
			float wallSteeringDistance = 1.5f;
   
			for (int i = -1; i <= 1; i += 2) 
			{
				float angle = ant.FacingAngle + i * Mathf.PI * 0.25f;
				
				float pheromoneX = ant.Position.x + Mathf.Cos(angle) * pheroSteeringDistance;
				float pheromoneY = ant.Position.y + Mathf.Sin(angle) * pheroSteeringDistance;
   
				if (pheromoneX < 0 || pheromoneY < 0 || pheromoneX >= Config.MapSize || pheromoneY >= Config.MapSize) 
				{
   
				}
				else 
				{
					// int index = PheromoneIndex((int)testX,(int)testY);
					// float value = pheromones[index].r;
					// output += value*i;
					
					// foreach (PheromoneData pheromoneData in PheromoneDataArray)
					// {
					// 	// if (pheromoneData.Index != (int)pheromoneX + (int)pheromoneY * Config.MapSize)
					// 	// 	continue;
					// 	//
					// 	// pheroSteeringOutput += pheromoneData.Color.r * i;
					// 	
					// 	pheroSteeringOutput += 
					// 		pheromoneData.Color.r * pheromoneData.Index * Convert.ToInt32(ant.Index != (int)pheromoneX + (int)pheromoneY * Config.MapSize);
					// }

					pheroSteeringOutput +=
						PheromoneDataArray[(int)pheromoneX + (int)pheromoneY * Config.MapSize].Color.r *
						PheromoneDataArray[(int)pheromoneX + (int)pheromoneY * Config.MapSize].Index;

					// PheromoneSteeringJob pheromoneColorJob = new PheromoneSteeringJob()
					// {
					// 	PosX = (int)pheromoneX,
					// 	PosY = (int)pheromoneY,
					// 	MapSize = Config.MapSize
					// };
					//
					// pheromoneColorJob.ScheduleParallel(PheromoneQuery);
				}
				
				float wallX = ant.Position.x + Mathf.Cos(angle) * wallSteeringDistance;
				float wallY = ant.Position.y + Mathf.Sin(angle) * wallSteeringDistance;
				
				if (wallX < 0 || wallY < 0 || wallX >= Config.MapSize || wallY >= Config.MapSize) 
				{
					
				} 
				else 
				{
					// int value = GetObstacleBucket(testX, testY).Length;
					int value = 0;
					foreach (ObstacleData obstacle in ObstacleDataArray)
					{
						// if (Vector2.SqrMagnitude(new float2(wallX, wallY) - obstacle.Position) > obstacle.Radius)
						// 	continue;
						//
						// value++;
						
						value += Convert.ToInt32(
							Vector2.SqrMagnitude(new float2(wallX, wallY) - obstacle.Position) > obstacle.Radius);
					}
   		
					if (value > 0)
						wallSteeringOutput -= i;

					// WallSteeringParallelJob wallSteeringParallelJob = new WallSteeringParallelJob()
					// {
					// 	PosX = (int)wallX,
					// 	PosY = (int)wallY,
					// 	ObstacleDataArray = ObstacleDataArray
					// };
					//
					// wallSteeringParallelJob.Schedule(ObstacleDataArray.Length, 128);

					// WallSteeringJob wallSteeringJob = new WallSteeringJob()
					// {
					// 	PosX = (int)wallX,
					// 	PosY = (int)wallY
					// };
					//
					// wallSteeringJob.ScheduleParallel();

					// if (value > 0)
					// 	wallSteeringOutput -= i;
				}
			}
			
			// float pheroSteering = Mathf.Sign(pheroSteeringOutput);
			// int wallSteering = wallSteeringOutput;
			//
			// ant.FacingAngle += pheroSteering * Config.PheromoneSteerStrength;
			// ant.FacingAngle += wallSteering * Config.WallSteerStrength;
   //
			// targetSpeed *= 1f - (Mathf.Abs(pheroSteering) + Mathf.Abs(wallSteering)) / 3f;
   //
			// ant.Speed += (targetSpeed - ant.Speed) * Config.AntAccel;
   //
			// float2 targetPos;
			// // int index1 = i / instancesPerBatch;
			// // int index2 = i % instancesPerBatch;
			// if (ant.HoldingResource == false) 
			// {
			// 	targetPos = ResourcePosition.Position;
			//
			// 	// antColors[index1][index2] += ((Vector4)searchColor * ant.brightness - antColors[index1][index2])*.05f;
			// } 
			// else 
			// {
			// 	targetPos = ColonyPosition.Position;
			// 	// antColors[index1][index2] += ((Vector4)carryColor * ant.brightness - antColors[index1][index2]) * .05f;
			// }
			//
			// if (!Linecast(ant.Position, targetPos)) 
			// {
			// 	Color color = Color.green;
			// 	float targetAngle = Mathf.Atan2(targetPos.y - ant.Position.y,targetPos.x - ant.Position.x);
			// 	if (targetAngle - ant.FacingAngle > Mathf.PI) 
			// 	{
			// 		ant.FacingAngle += Mathf.PI * 2f;
			// 		color = Color.red;
			// 	} 
			// 	else if (targetAngle - ant.FacingAngle < -Mathf.PI) 
			// 	{
			// 		ant.FacingAngle -= Mathf.PI * 2f;
			// 		color = Color.red;
			// 	}
			// 	else 
			// 	{
			// 		if (Mathf.Abs(targetAngle - ant.FacingAngle) < Mathf.PI * 0.5f)
			// 			ant.FacingAngle += (targetAngle - ant.FacingAngle) * Config.GoalSteerStrength;
			// 	}
   //
			// 	//Debug.DrawLine(ant.position/mapSize,targetPos/mapSize,color);
			// }
			//
			// if (Vector2.SqrMagnitude(ant.Position - targetPos) < 4f * 4f) {
			// 	ant.HoldingResource = !ant.HoldingResource;
			// 	ant.FacingAngle += Mathf.PI;
			// }
   //
			// float vx = Mathf.Cos(ant.FacingAngle) * ant.FacingAngle;
			// float vy = Mathf.Sin(ant.FacingAngle) * ant.FacingAngle;
			// float ovx = vx;
			// float ovy = vy;
   //
			// if (ant.Position.x + vx < 0f || ant.Position.x + vx > Config.MapSize) {
			// 	vx = -vx;
			// } else {
			// 	ant.Position.x += vx;
			// }
			// if (ant.Position.y + vy < 0f || ant.Position.y + vy > Config.MapSize) {
			// 	vy = -vy;
			// } else {
			// 	ant.Position.y += vy;
			// }
   //
			// float dx, dy, dist;
   //
			// // Obstacle[] nearbyObstacles = GetObstacleBucket(ant.ValueRO.Position);
			// //
			// // for (int j=0;j<nearbyObstacles.Length;j++) {
			// // 	Obstacle obstacle = nearbyObstacles[j];
			// // 	dx = ant.ValueRO.Position.x - obstacle.position.x;
			// // 	dy = ant.ValueRO.Position.y - obstacle.position.y;
			// // 	float sqrDist = dx * dx + dy * dy;
			// // 	if (sqrDist < obstacleConfig.ObstacleRadius * obstacleConfig.ObstacleRadius) {
			// // 		dist = Mathf.Sqrt(sqrDist);
			// // 		dx /= dist;
			// // 		dy /= dist;
			// // 		ant.ValueRW.Position.x = obstacle.position.x + dx * obstacleConfig.ObstacleRadius;
			// // 		ant.ValueRW.Position.y = obstacle.position.y + dy * obstacleConfig.ObstacleRadius;
			// //
			// // 		vx -= dx * (dx * vx + dy * vy) * 1.5f;
			// // 		vy -= dy * (dx * vx + dy * vy) * 1.5f;
			// // 	}
			// // }
			//
			// // float2 antPos = ant.ValueRO.Position;
			// // int posX = (int)(ant.ValueRO.Position.x / obstacleConfig.MapSize * obstacleConfig.BucketResolution);
			// // int posY = (int)(ant.ValueRO.Position.y / obstacleConfig.MapSize * obstacleConfig.BucketResolution);
			// foreach (RefRO<ObstacleData> obstacle in SystemAPI.Query<RefRO<ObstacleData>>())
			// {
			// 	if (Vector2.SqrMagnitude(ant.Position - obstacle.ValueRO.Position) > obstacle.ValueRO.Radius)
			// 		continue;
			// 	
			// 	dx = ant.Position.x - obstacle.ValueRO.Position.x;
			// 	dy = ant.Position.y - obstacle.ValueRO.Position.y;
			// 	float sqrDist = dx * dx + dy * dy;
			// 	if (sqrDist < Config.ObstacleRadius * Config.ObstacleRadius)
			// 	{
			// 		dist = Mathf.Sqrt(sqrDist);
			// 		dx /= dist;
			// 		dy /= dist;
			// 		ant.Position.x = obstacle.ValueRO.Position.x + dx * Config.ObstacleRadius;
			// 		ant.Position.y = obstacle.ValueRO.Position.y + dy * Config.ObstacleRadius;
   //
			// 		vx -= dx * (dx * vx + dy * vy) * 1.5f;
			// 		vy -= dy * (dx * vx + dy * vy) * 1.5f;
			// 	}
			// 	// for (int x = Mathf.FloorToInt((obstacle.ValueRO.position.x - obstacle.ValueRO.radius) / obstacleConfig.MapSize * obstacleConfig.BucketResolution); 
			// 	//      x <= Mathf.FloorToInt((obstacle.ValueRO.position.x + obstacle.ValueRO.radius) /  obstacleConfig.MapSize * obstacleConfig.BucketResolution); 
			// 	//      x++) 
			// 	// {
			// 	// 	if (x < 0 || x >= obstacleConfig.BucketResolution)
			// 	// 		continue;
			// 	// 	
			// 	// 	for (int y = Mathf.FloorToInt((obstacle.ValueRO.position.y - obstacle.ValueRO.radius) / obstacleConfig.MapSize * obstacleConfig.BucketResolution); 
			// 	// 	     y <= Mathf.FloorToInt((obstacle.ValueRO.position.y + obstacle.ValueRO.radius) / obstacleConfig.MapSize * obstacleConfig.BucketResolution); 
			// 	// 	     y++) 
			// 	// 	{
			// 	// 		if (y < 0 || y >= obstacleConfig.BucketResolution)
			// 	// 			continue;
			// 	// 		
			// 	// 		// tempObstacleBuckets[x,y].Add(obstacles[i]);
			// 	// 	}
			// 	// }
			// }
   //
			// float inwardOrOutward = -Config.OutwardStrength;
			// float pushRadius = Config.MapSize * .4f;
			// if (ant.HoldingResource) 
			// {
			// 	inwardOrOutward = Config.InwardStrength;
			// 	pushRadius = Config.MapSize;
			// }
			// dx = ColonyPosition.Position.x - ant.Position.x;
			// dy = ColonyPosition.Position.y - ant.Position.y;
			// dist = Mathf.Sqrt(dx * dx + dy * dy);
			// inwardOrOutward *= 1f-Mathf.Clamp01(dist / pushRadius);
			// vx += dx / dist * inwardOrOutward;
			// vy += dy / dist * inwardOrOutward;
   //
			// if (ovx != vx || ovy != vy) 
			// {
			// 	ant.FacingAngle = Mathf.Atan2(vy,vx);
			// }
   //
			// //if (ant.holdingResource == false) {
			// //float excitement = 1f-Mathf.Clamp01((targetPos - ant.position).magnitude / (mapSize * 1.2f));
			// float excitement = .3f;
			// if (ant.HoldingResource) 
			// {
			// 	excitement = 1f;
			// }
			// excitement *= ant.Speed / Config.AntSpeed;
			//
			// // TODO: Rewrite this piece
			// // DropPheromones(ant.ValueRO.Position, excitement);
			//
			// int dropPheromonesX = Mathf.FloorToInt(ant.Position.x);
			// int dropPheromonesY = Mathf.FloorToInt(ant.Position.y);
			// if (dropPheromonesX < 0 || dropPheromonesY < 0 || dropPheromonesX >= Config.MapSize || dropPheromonesY >= Config.MapSize) 
			// {
			// 	
			// }
			// else
			// {
			// 	// int index = PheromoneIndex(x,y);
			// 	// TODO: Time.fixedDeltaTime is not the actual update interval (50Hz vs 60Hz)
			// 	foreach (RefRW<PheromoneData> pheromoneData in SystemAPI.Query<RefRW<PheromoneData>>())
			// 	{
			// 		if (pheromoneData.ValueRO.Index != (int)dropPheromonesX + (int)dropPheromonesY * Config.MapSize)
			// 			continue;
			// 	
			// 		pheromoneData.ValueRW.Color.r += 
			// 			(Config.TrailAddSpeed * excitement * SystemAPI.Time.fixedDeltaTime) * (1f - pheromoneData.ValueRO.Color.r);
			// 		if (pheromoneData.ValueRO.Color.r > 1f) 
			// 		{
			// 			pheromoneData.ValueRW.Color.r = 1f;
			// 		}
			// 	}
			// 	//}
			// }
   //
			// // TODO: Where the heck is translation?
			// // loat4x4 matrix = GetRotationMatrix(ant.ValueRO.FacingAngle);
			//
			// // float thisFacingAngle = ant.ValueRO.FacingAngle;
			// // thisFacingAngle /= Mathf.PI * 2f;
			// // thisFacingAngle -= Mathf.Floor(thisFacingAngle);
			//
			//
			// // GetRotationMatrix() Method
			// // Matrix4x4 GetRotationMatrix(float angle) {
			// // 	angle /= Mathf.PI * 2f;
			// // 	angle -= Mathf.Floor(angle);
			// // 	angle *= rotationResolution;
			// // 	return rotationMatrixLookup[((int)angle)%rotationResolution];
			// // }
			//
			// // matrix.c0.w = ant.ValueRO.Position.x / config.MapSize;
			// // matrix.c1.w = ant.ValueRO.Position.y / config.MapSize;
			// // matrices[i / instancesPerBatch][i % instancesPerBatch] = matrix;
			//
			// // ant.ValueRW.Matrix = matrix;
   //
			// ant.Matrix.c0.w = ant.Position.x / Config.MapSize;
			// ant.Matrix.c1.w = ant.Position.y / Config.MapSize;
	    }
    }
    
    [BurstCompile]
    public partial struct PheromoneArrayJob : IJobEntity
    {
	    public NativeArray<PheromoneData> PheromoneDataArray;
	    
	    public void Execute([EntityIndexInQuery] int entityIndexInQuery, in PheromoneData pheromoneData)
	    {
		    PheromoneDataArray[entityIndexInQuery] = pheromoneData;
	    }
    }
    
    [BurstCompile]
    public partial struct ObstacleArrayJob : IJobEntity
    {
	    public NativeArray<ObstacleData> ObstacleDataArray;
	    
	    // Class EntityIndexInQuery
	    // Specifies that this int parameter is used as a way to get the packed entity index inside the current query.
	    // Usage: An int parameter found inside the execute method of an IJobEntity.
	    public void Execute([EntityIndexInQuery] int entityIndexInQuery, in ObstacleData obstacleData)
	    {
		    ObstacleDataArray[entityIndexInQuery] = obstacleData;
	    }
    }

    [BurstCompile]
    public partial struct PheromoneSteeringJob : IJobEntity
    {
	    public int MapSize;
	    public int PosX;
	    public int PosY;
	    public int InputIdx;

	    public NativeArray<AntData> Ant;
	    
	    public void Execute(ref PheromoneData pheromone, ref IndexData index)
	    {
		    // if (index.Idx != (int)PosX + (int)PosY * MapSize)
			   //  return;
					 //
		    // pheroSteeringOutput += pheromone.Color.r * InputIdx;
		    
		    // pheroSteeringOutput += pheromone.Color.r * InputIdx * Convert.ToInt32(index.Idx != (int)PosX + (int)PosY * MapSize);
		    float a = pheromone.Color.r * InputIdx * Convert.ToInt32(index.Idx != (int)PosX + (int)PosY * MapSize);
	    }
    }

    [BurstCompile]
    public partial struct WallSteeringJob : IJobEntity
    {
	    public int PosX;
	    public int PosY;
	    
	    public void Execute(ref ObstacleData obstacle, ref IndexData index)
	    {
		    // if (Vector2.SqrMagnitude(new float2(PosX, PosY) - obstacle.Position) > obstacle.Radius)
			   //  return;
		    
		    // value++;
		    
		    // value += Convert.ToInt32(Vector2.SqrMagnitude(new float2(PosX, PosY) - obstacle.Position) > obstacle.Radius);
		    float a = Convert.ToInt32(Vector2.SqrMagnitude(new float2(PosX, PosY) - obstacle.Position) > obstacle.Radius);
	    }
    }
    
    [BurstCompile]
    public struct WallSteeringParallelJob : IJobParallelFor
    {
	    public int PosX;
	    public int PosY;
	    
	    [ReadOnly]
	    public NativeArray<ObstacleData> ObstacleDataArray;
	    
	    public void Execute(int index)
	    {
		    float a = Convert.ToInt32(
			    Vector2.SqrMagnitude(
				    new float2(PosX, PosY) - ObstacleDataArray[index].Position) > ObstacleDataArray[index].Radius);
		    
		    // Debug.Log("HIhihi");
	    }
    }
}