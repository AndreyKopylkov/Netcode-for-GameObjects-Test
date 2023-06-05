using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Food : NetworkBehaviour
{
    public GameObject Prefab;
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        if(!NetworkManager.Singleton.IsServer)
            return;

        if (other.TryGetComponent(out PlayerLenght playerLenght))
        {
            playerLenght.AddLength();   
        }
        else if(other.TryGetComponent(out Tail tail))
        {
            tail.NetworkedOwner.GetComponent<PlayerLenght>().AddLength();
        }
        
        NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, Prefab);
        // NetworkObject.Despawn();
    }
}
