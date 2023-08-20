using System;
using Gameplay;
using Gameplay.Levels;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _Rigidbody;
    [SerializeField] private LevelManager _LevelManager;
    [SerializeField] private InputManager _InputManager;

    public event Action OnPlayerHitSegment;

    private bool _isPaused = false;
    
    private void Update()
    {
        if ( _isPaused ) return;
        
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
    }

    private float? _lastPositionX = null;
    void HandleInput()
    {
        // TODO: Obtain inputs from device
        if (_InputManager.WasJump)
        {
            if (_Rigidbody.velocity.y < 0) _Rigidbody.velocity = _Rigidbody.velocity.With( y: 0 );
            
            _Rigidbody.AddForce(Vector3.up * 10f, ForceMode.VelocityChange);
        }

        var slideChanged = _InputManager.SlideChange;
        if (slideChanged != 0)
        {
            if (_lastPositionX == null) _lastPositionX = _Rigidbody.position.x;
            
            var levelBounds = _LevelManager.GetLevelBounds();
            var currentPos = Mathf.InverseLerp( levelBounds.left, levelBounds.right, _lastPositionX.Value );
            var targetPos = currentPos + slideChanged;

            targetPos = Mathf.Clamp( targetPos, -1f, 1 );
            
            _Rigidbody.position = _Rigidbody.position.With( x: Mathf.Lerp( levelBounds.left, levelBounds.right, targetPos ) );
        }
        else
        {
            _lastPositionX = null;
        }
    }

    private void ResetHorizontalVelocity()
    {
        _Rigidbody.velocity = _Rigidbody.velocity.With( x: 0 );
    }

    public void SetPause( bool isPaused )
    {
        _isPaused = isPaused;

        _Rigidbody.isKinematic = isPaused;
    }
}
