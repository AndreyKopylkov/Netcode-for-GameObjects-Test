using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float _speed = 5;
    [SerializeField] private PlayerLenght _playerLength;

    private Camera _camera;
    private Vector3 _mouseInput;
    private Vector3 mouseWorldCoordinates;

    private readonly ulong[] _targetClients = new ulong[1];

    [CanBeNull] public static event System.Action GameOverEvent;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        Initialize();
    }

    private void Initialize()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if(!IsOwner)
            return;
        
        if(!Application.isFocused)
            return;
        
        GetInput();
        if (mouseWorldCoordinates != transform.position)
        {
            Movement();
            Rotation();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision");

        if(!other.gameObject.CompareTag("Player"))
            return;
        if(!IsOwner)
            return;

        if (other.gameObject.TryGetComponent(out PlayerLenght targetPlayerLenght))
        {
            Debug.Log("Head Collision");

            PlayerData player1 = new PlayerData()
            {
                Id = OwnerClientId,
                Length = _playerLength.Lenght.Value
            };
            PlayerData player2 = new PlayerData()
            {
                Id = targetPlayerLenght.OwnerClientId,
                Length = targetPlayerLenght.Lenght.Value
            };
            DetermineCollisionWinnerServerRpc(player1, player2);
        }
        else if (other.gameObject.TryGetComponent(out Tail tail))
        {
            Debug.Log("Tail Collision");
            WinInformationServerRpc(tail.NetworkedOwner.GetComponent<PlayerController>().OwnerClientId,
                OwnerClientId);
        }
    }

    // [ServerRpc(RequireOwnership = false)]
    [ServerRpc]
    private void DetermineCollisionWinnerServerRpc(PlayerData player1, PlayerData player2)
    {
        if (player1.Length > player2.Length)
        {
            WinInformationServerRpc(player1.Id, player2.Id);
        }
        else
        {
            WinInformationServerRpc(player2.Id, player1.Id);
        }
    }

    [ServerRpc]
    private void WinInformationServerRpc(ulong winner, ulong loser)
    {
        _targetClients[0] = winner;
        
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = _targetClients
            }
        };
        
        AtePlayerClientRpc();

        _targetClients[0] = loser;
        clientRpcParams.Send.TargetClientIds = _targetClients;
        GameOverClientRpc();
    }

    [ClientRpc]
    private void AtePlayerClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if(!IsOwner)
            return;
        
        Debug.Log("You Ate a Player!");
    }

    [ClientRpc]
    private void GameOverClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if(!IsOwner)
            return;
        
        Debug.Log("You Lose!");
        GameOverEvent?.Invoke();
        NetworkManager.Singleton.Shutdown();
    }
    
    private void GetInput()
    {
        _mouseInput.x = Input.mousePosition.x;
        _mouseInput.y = Input.mousePosition.y;
        _mouseInput.z = _camera.nearClipPlane;
        mouseWorldCoordinates = _camera.ScreenToWorldPoint(_mouseInput);
    }

    private void Movement()
    {
        mouseWorldCoordinates.z = 0f;
        transform.position = Vector3.MoveTowards(transform.position, mouseWorldCoordinates,
            _speed * Time.deltaTime);
    }

    private void Rotation()
    {
        mouseWorldCoordinates = _camera.ScreenToWorldPoint(_mouseInput);
        Vector3 targetDirection = mouseWorldCoordinates - transform.position;
        targetDirection.z = 0;
        transform.up = targetDirection;
    }
}