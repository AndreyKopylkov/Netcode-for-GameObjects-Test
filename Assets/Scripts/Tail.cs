using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tail : MonoBehaviour
{
    public Transform FollowTransform;
    public Transform NetworkedOwner;
    public SpriteRenderer SpriteRenderer;
    
    [SerializeField] private float _delayTime = 0.1f;
    [SerializeField] private float _distance = 0.3f;
    [SerializeField] private float _moveStep = 10f;

    private Vector3 _targetPosition;

    private void Update()
    {
        _targetPosition = FollowTransform.position - FollowTransform.forward * _distance;
        _targetPosition += (transform.position - _targetPosition) * _delayTime;
        _targetPosition.z = 0f;

        transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _moveStep);
    }
}
