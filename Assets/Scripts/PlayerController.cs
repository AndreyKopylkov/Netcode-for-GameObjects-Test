using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float _speed = 5;
    
    private Camera _camera;
    private Vector3 _mouseInput;

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
        Movement();
        Rotation();
    }

    private void GetInput()
    {
        _mouseInput.x = Input.mousePosition.x;
        _mouseInput.y = Input.mousePosition.y;
        _mouseInput.z = _camera.nearClipPlane;
    }

    private void Movement()
    {
        Vector3 mouseWorldCoordinates = _camera.ScreenToWorldPoint(_mouseInput);
        transform.position = Vector3.MoveTowards(transform.position, mouseWorldCoordinates,
            _speed * Time.deltaTime);
    }

    private void Rotation()
    {
        Vector3 mouseWorldCoordinates = _camera.ScreenToWorldPoint(_mouseInput);
        if(mouseWorldCoordinates != transform.position)
        {
            Vector3 targetDirection;
            targetDirection = mouseWorldCoordinates - transform.position;
            targetDirection.z = 0;
            transform.up = targetDirection;
        }
    }
}
