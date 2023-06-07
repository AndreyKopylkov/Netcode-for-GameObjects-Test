using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Food : NetworkBehaviour
{
    [SerializeField] private Collider2D _collider;
    
    public GameObject Prefab;

    private bool _isEaten;

    private void OnEnable()
    {
        _collider.enabled = true;
        _isEaten = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(_isEaten)
            return;
            
        if (!other.CompareTag("Player"))
            return;
        
        if(!NetworkManager.Singleton.IsServer)
            return;

        if (other.TryGetComponent(out PlayerLenght playerLenght))
        {
            playerLenght.AddLength();   
        }
        // else if(other.TryGetComponent(out Tail tail))
        // {
        //     tail.NetworkedOwner.GetComponent<PlayerLenght>().AddLength();
        // }
        
        _collider.enabled = false;
        _isEaten = true;
        NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, Prefab);
        // if(IsSpawned)
        //     NetworkObject.Despawn();
    }
}
