using System;
using Gameplay;
using Gameplay.Levels;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _Rigidbody;

    [SerializeField] private InputManager _InputManager;

    public event Action OnPlayerHitSegment;
    
    private void Update()
    {
        HandleInput();
        HandleBoundaries();
    }
    
    private void OnCollisionEnter( Collision other )
    {
        if ( other.gameObject.CompareTag( "LevelSegment" ) )
        {
            OnPlayerHitSegment?.Invoke();
        }
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
        if (_InputManager.WasJump)
        {
            _Rigidbody.AddForce(Vector3.up * 15f, ForceMode.VelocityChange);
        }

        var slideChanged = _InputManager.SlideChange;
        if (slideChanged < 0)
        {
            if ( _Rigidbody.velocity.x > 0 ) ResetHorizontalVelocity();
            
            _Rigidbody.AddForce(Vector3.left * 5);
        }
        
        if (slideChanged > 0)
        {
            if ( _Rigidbody.velocity.x < 0 ) ResetHorizontalVelocity();
            _Rigidbody.AddForce(Vector3.right * 5);
        }
        
    }

    private void ResetHorizontalVelocity()
    {
        _Rigidbody.velocity = _Rigidbody.velocity.With( x: 0 );
    }
}
