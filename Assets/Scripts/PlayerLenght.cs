using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class PlayerLenght : NetworkBehaviour
{
    [SerializeField] private Tail _tailPrefab;
    
    public NetworkVariable<ushort> Length = new(0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    [CanBeNull] public static event System.Action<ushort> ChangedLengthEvent;

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
            Length.OnValueChanged += LengthChangedEvent;
        
        SpawnTailForOtherPlayersInSessionOnStart();

        SetLenght(1);
    }

    public override void OnNetworkDespawn()
    {
        if(!IsServer)
            Length.OnValueChanged -= LengthChangedEvent;
    }

    private void SpawnTailForOtherPlayersInSessionOnStart()
    {
        if (IsOwner) return;
        for (int i = 0; i < Length.Value - 1; ++i)
            InstantiateTail();
    }

    [ContextMenu("Add Length")]
    public void AddLength()
    {
        Length.Value += 1;
        LengthChanged();
    }

    private void SetLenght(ushort newValue)
    {
        Length.Value = newValue;

        if(IsOwner)
            ChangedLengthEvent?.Invoke(Length.Value);
    }

    private void LengthChanged()
    {
        InstantiateTail();

        if (IsOwner)
        {
            ChangedLengthEvent?.Invoke(Length.Value);
            ClientMusicPlayer.Instance.PlayerEatAudioClip();
        }
    }
    
    private void LengthChangedEvent(ushort previousValue, ushort newValue)
    {
        LengthChanged();
    }

    private void InstantiateTail()
    {
        if(Length.Value == 1)
            return;
        
        Tail newTail = Instantiate(_tailPrefab, transform.position, Quaternion.identity);
        newTail.SpriteRenderer.sortingOrder = -Length.Value;
        newTail.NetworkedOwner = transform;
        newTail.FollowTransform = _lastTail;
        _lastTail = newTail.transform;
        Physics2D.IgnoreCollision(newTail.GetComponent<Collider2D>(), _collider2D);
        
        _tails.Add(newTail);
    }
}