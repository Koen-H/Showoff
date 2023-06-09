using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Video;

public class TelevisionManager : NetworkBehaviour
{
    [SerializeField] private List<VideoClip> clips;
    [SerializeField] private VideoPlayer player;

    public override void OnNetworkSpawn()
    {
        if (IsServer) SetVideoClientRpc(Random.Range(0,clips.Count));
    }

    [ClientRpc]
    void SetVideoClientRpc(int index)
    {
        player.clip = clips[index];
    }
}
