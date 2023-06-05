using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _startFoodCount = 30;

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SpawnFoodStart;
        StartCoroutine(SpawnOverTime());
    }

    private void SpawnFoodStart()
    {
        NetworkManager.Singleton.OnServerStarted -= SpawnFoodStart;
        NetworkObjectPool.Singleton.RegisterPrefabInternal(_prefab, 30);
        for (int i = 0; i < _startFoodCount; i++)
        {
            SpawnFood();
        }
    }

    private IEnumerator SpawnOverTime()
    {
        while (NetworkManager.Singleton.ConnectedClients.Count > 0)
        {
            yield return new WaitForSeconds(2f);
            SpawnFood();
        }
    }

    private void SpawnFood()
    {
        NetworkObject obj = NetworkObjectPool.Singleton.GetNetworkObject(_prefab, GetRandomPositionOnMap(),
            Quaternion.identity);
        obj.GetComponent<Food>().Prefab = _prefab;
        obj.Spawn(true);
    }

    private Vector3 GetRandomPositionOnMap()
    {
        return new Vector3(Random.Range(-17f, 17f), Random.Range(-9f, 9f), 0f);
    }
}