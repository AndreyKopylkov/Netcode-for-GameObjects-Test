using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerLenght : NetworkBehaviour
{
    [SerializeField] private Tail _tailPrefab;
    
    public NetworkVariable<ushort> Lenght = new(1,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private List<Tail> _tails;
    private Transform _lastTail;
    private Collider2D _collider2D;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _tails = new List<Tail>();
        _lastTail = transform;
        _collider2D = GetComponent<Collider2D>();
        if(!IsServer) 
            Lenght.OnValueChanged += LengthChanged;
    }
    
    [ContextMenu("Add Length")]
    public void AddLength()
    {
        Lenght.Value += 1;
        InstantiateTail();
    }

    private void LengthChanged(ushort previousValue, ushort newValue)
    {
        Debug.Log("LengthChanged Callback");
        InstantiateTail();
    }

    private void InstantiateTail()
    {
        Tail newTail = Instantiate(_tailPrefab, transform.position, Quaternion.identity);
        newTail.SpriteRenderer.sortingOrder = -Lenght.Value;
        newTail.NetworkedOwner = transform;
        newTail.FollowTransform = _lastTail;
        _lastTail = newTail.transform;
        Physics2D.IgnoreCollision(newTail.GetComponent<Collider2D>(), _collider2D);
        
        _tails.Add(newTail);
    }
}
