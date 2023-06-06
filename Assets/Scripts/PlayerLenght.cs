using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class PlayerLenght : NetworkBehaviour
{
    [SerializeField] private Tail _tailPrefab;
    
    public NetworkVariable<ushort> Lenght = new(0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    [CanBeNull] public static event System.Action<ushort> ChangedLenghtEvent;

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
            Lenght.OnValueChanged += LengthChangedEvent;

        SetLenght(1);
    }

    public override void OnNetworkDespawn()
    {
        if(!IsServer)
            Lenght.OnValueChanged -= LengthChangedEvent;
    }

    [ContextMenu("Add Length")]
    public void AddLength()
    {
        Lenght.Value += 1;
        LengthChanged();
    }

    private void SetLenght(ushort newValue)
    {
        Lenght.Value = newValue;

        if(IsOwner)
            ChangedLenghtEvent?.Invoke(Lenght.Value);
    }

    private void LengthChanged()
    {
        InstantiateTail();

        if (IsOwner)
        {
            ChangedLenghtEvent?.Invoke(Lenght.Value);
            ClientMusicPlayer.Instance.PlayerEatAudioClip();
        }
    }
    
    private void LengthChangedEvent(ushort previousValue, ushort newValue)
    {
        Debug.Log("LengthChanged Callback");
        LengthChanged();
    }

    private void InstantiateTail()
    {
        if(Lenght.Value == 1)
            return;
        
        Tail newTail = Instantiate(_tailPrefab, transform.position, Quaternion.identity);
        newTail.SpriteRenderer.sortingOrder = -Lenght.Value;
        newTail.NetworkedOwner = transform;
        newTail.FollowTransform = _lastTail;
        _lastTail = newTail.transform;
        Physics2D.IgnoreCollision(newTail.GetComponent<Collider2D>(), _collider2D);
        
        _tails.Add(newTail);
    }
}