using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS
{
    public class ColonyAuthoring : MonoBehaviour
    {
        public int mapSize;
        
        private class Baker : Baker<ColonyAuthoring>
        {
            public override void Bake(ColonyAuthoring authoring)
            {
                float2 colonyPosition = Vector2.one * authoring.mapSize * 0.5f;
                float4x4 colonyMatrix = Matrix4x4.TRS(
                    new float3(colonyPosition.x, colonyPosition.y, 0) / authoring.mapSize, 
                    Quaternion.identity, 
                    new Vector3(4f,4f,.1f) / authoring.mapSize);
                
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TransformData()
                {
                    Position = colonyPosition,
                    TransformMatrix = colonyMatrix
                });

                AddComponent(entity, new ColonyTagData()
                {
                    Position = colonyPosition,
                    TransformMatrix = colonyMatrix
                });
            }
        }
    }
}