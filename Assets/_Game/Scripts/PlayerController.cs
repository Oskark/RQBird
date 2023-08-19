using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Levels;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _Rigidbody;

    private void Update()
    {
        HandleInput();
        HandleBoundaries();
    }

    private void HandleBoundaries()
    {
        // TODO: Obtain from proper place
        const float TOO_HIGH = 10f;
        
        var  isTooHigh = transform.position.y >= TOO_HIGH;
        if ( isTooHigh )
        {
            _Rigidbody.velocity = _Rigidbody.velocity.With( y: 0 );
            transform.position = transform.position.With( y: TOO_HIGH );
        }

        const float TOO_LEFT_OR_RIGHT = 3f;
        var isTooLeftOrRight = Mathf.Abs(transform.position.x) >= TOO_LEFT_OR_RIGHT;
        if ( isTooLeftOrRight )
        {
            _Rigidbody.velocity = _Rigidbody.velocity.With( x: 0 );
            transform.position = transform.position.With( x: Mathf.Sign(transform.position.x) * TOO_LEFT_OR_RIGHT );
        }
    }

    void HandleInput()
    {
        // TODO: Obtain inputs from device
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _Rigidbody.AddForce(Vector3.up * 15f, ForceMode.VelocityChange);
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            if (_Rigidbody.velocity.x > 0) _Rigidbody.velocity = new Vector3(0, _Rigidbody.velocity.y, _Rigidbody.velocity.z);
            
            _Rigidbody.AddForce(Vector3.left * 5);
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            if (_Rigidbody.velocity.x < 0) _Rigidbody.velocity = new Vector3(0, _Rigidbody.velocity.y, _Rigidbody.velocity.z);
            _Rigidbody.AddForce(Vector3.right * 5);
        }
        
    }
}
