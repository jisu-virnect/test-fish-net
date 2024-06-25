using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// InstanceFinder를 통한 Broadcast 채팅
/// </summary>
public class CubePositionBroadcast : NetworkBehaviour
{
    public List<Transform> cubePositions = new List<Transform>();
    public int transformIndex;

    private void OnEnable()
    {
        //InstanceFinder.ClientManager.RegisterBroadcast<PositionIndex>(OnServerPositionBroadcast);
        InstanceFinder.ServerManager.RegisterBroadcast<PositionIndex>(OnClientPositionBroadcast);
    }
    private void OnDisable()
    {
        //InstanceFinder.ClientManager.UnregisterBroadcast<PositionIndex>(OnServerPositionBroadcast);
        InstanceFinder.ServerManager.UnregisterBroadcast<PositionIndex>(OnClientPositionBroadcast);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            int nextIndex = transformIndex + 1;

            if(nextIndex >= cubePositions.Count)
            {
                nextIndex = 0;
            }
            if (InstanceFinder.IsServer)
            {
                InstanceFinder.ServerManager.Broadcast(new PositionIndex() { tIndex = nextIndex });
            }
            else if (InstanceFinder.IsClient)
            {
                InstanceFinder.ClientManager.Broadcast(new PositionIndex() { tIndex = nextIndex });
            }
        }

        transform.position = cubePositions[transformIndex].position;
    }
    private void OnServerPositionBroadcast(PositionIndex index, Channel channel)
    {
        transformIndex = index.tIndex;
    }

    private void OnClientPositionBroadcast(NetworkConnection networkConnection, PositionIndex index, Channel channel)
    {
        InstanceFinder.ServerManager.Broadcast(index);
    }
}

public struct PositionIndex : IBroadcast
{
    public int tIndex;
}