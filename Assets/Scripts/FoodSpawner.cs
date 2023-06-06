using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoodSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _startFoodCount = 30;
    [SerializeField] private int _maxFoodOnScene = 50;
    [SerializeField] private float _spawnNewFoodTimer = 2f;

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SpawnFoodStart;
    }
    
    private void SpawnFoodStart()
    {
        NetworkManager.Singleton.OnServerStarted -= SpawnFoodStart;
        NetworkObjectPool.Singleton.RegisterPrefabInternal(_prefab, _startFoodCount);
        for (int i = 0; i < _startFoodCount; i++)
        {
            SpawnFood();
        }
        
        StartCoroutine(SpawnOverTime());
    }

    private IEnumerator SpawnOverTime()
    {
        while (NetworkManager.Singleton.ConnectedClients.Count > 0)
        {
            yield return new WaitForSeconds(_spawnNewFoodTimer);
            if(NetworkObjectPool.Singleton.GetCurrentPrefabCount(_prefab) < _maxFoodOnScene)
                SpawnFood();
        }
    }

    private void SpawnFood()
    {
        NetworkObject obj = NetworkObjectPool.Singleton.GetNetworkObject(_prefab, GetRandomPositionOnMap(),
            Quaternion.identity);
        obj.GetComponent<Food>().Prefab = _prefab;
        
        //Чтобы дважды не передавать объект с сервера на клиент
        // if(!obj.IsSpawned)
        obj.Spawn(true);
    }

    private Vector3 GetRandomPositionOnMap()
    {
        return new Vector3(Random.Range(-17f, 17f), Random.Range(-9f, 9f), 0f);
    }
}