using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;
    [SerializeField]
    public GameObject mainPlayer;
    public float3 offset;
    private EntityManager manager;
    public float3 playerPos;
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;

    }
    private void Start()
    {
        /*EntityQuery playerQ = manager.CreateEntityQuery(typeof(PlayerMovementInputComponent));
        var players = playerQ.ToEntityArray(Unity.Collections.Allocator.TempJob);
        mainPlayer = players[0];
        players.Dispose();*/
    }
    private void LateUpdate()
    {
        if (mainPlayer == null) return;

        playerPos = mainPlayer.transform.position;//manager.GetComponentData<Translation>(mainPlayer).Value;
        transform.position = playerPos + offset;
    }
}
