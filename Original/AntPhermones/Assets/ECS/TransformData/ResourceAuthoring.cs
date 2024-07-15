using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ECS
{
    public class ResourceAuthoring : MonoBehaviour
    {
        public int mapSize;

        private class Baker : Baker<ResourceAuthoring>
        {
            public override void Bake(ResourceAuthoring authoring)
            {
                float resourceAngle = Random.value * 2f * Mathf.PI;
                float2 resourcePosition = 
                    new float2(1f, 1f) * authoring.mapSize * .5f + 
                    new float2(Mathf.Cos(resourceAngle) * authoring.mapSize * 0.475f, Mathf.Sin(resourceAngle) * authoring.mapSize * 0.475f);
                
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TransformData()
                {
                    Position = 
                        new float2(1f, 1f) * authoring.mapSize * .5f + 
                        new float2(Mathf.Cos(resourceAngle) * authoring.mapSize * 0.475f,Mathf.Sin(resourceAngle) * authoring.mapSize * 0.475f),
                    TransformMatrix = float4x4.TRS(
                        new float3(resourcePosition.x, resourcePosition.y, 0) / authoring.mapSize, 
                        Quaternion.identity,
                        new Vector3(4f,4f,.1f) / authoring.mapSize)
                });

                AddComponent(entity, new ResourceTagData()
                {
                    Position = 
                        new float2(1f, 1f) * authoring.mapSize * .5f + 
                        new float2(Mathf.Cos(resourceAngle) * authoring.mapSize * 0.475f,Mathf.Sin(resourceAngle) * authoring.mapSize * 0.475f),
                    TransformMatrix = float4x4.TRS(
                        new float3(resourcePosition.x, resourcePosition.y, 0) / authoring.mapSize, 
                        Quaternion.identity,
                        new Vector3(4f,4f,.1f) / authoring.mapSize)
                });
            }
        }
    }
}