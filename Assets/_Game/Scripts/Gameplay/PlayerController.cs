using System;
using Gameplay;
using Gameplay.Levels;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _Rigidbody;
    
    [Inject] private InputManager _inputManager;
    [Inject] private LevelManager _LevelManager;
    [Inject] private GameplayData _gameplayData;
    
    
    public event Action OnPlayerHitSegment;
    
    private bool _isPaused = false;
    
    private float? _lastPositionX = null;
    private SignalBus _signalBus;
    
    [Inject]
    public void Construct( SignalBus signalBus )
    {
        _signalBus = signalBus;
        _signalBus.Subscribe<GameplayStateChangedSignal>( OnGameStateChanged );
    }

    private void OnDestroy()
    {
        _signalBus?.Unsubscribe<GameplayStateChangedSignal>( OnGameStateChanged );
    }

    private void OnGameStateChanged( GameplayStateChangedSignal newState )
    {
        var isFinished = newState.CurrentState == GameState.Result;
        if ( isFinished )
        {
            _isPaused = false;
            _Rigidbody.isKinematic = false; // Player can fall on failure
            
            return;
        }
        
        var isPlaying = newState.CurrentState == GameState.Play;
        
        SetPause( isPlaying == false );
    }

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
        float ceilingHeight = _gameplayData.CeilingHeight;
        
        var  isTooHigh = transform.position.y >= ceilingHeight;
        if ( isTooHigh )
        {
            _Rigidbody.velocity = _Rigidbody.velocity.With( y: 0 );
            _Rigidbody.position = _Rigidbody.position.With( y: ceilingHeight );
        }
    }

    void HandleInput()
    {
        if (_inputManager.WasJump)
        {
            if (_Rigidbody.velocity.y < 0) _Rigidbody.velocity = _Rigidbody.velocity.With( y: 0 );
            
            _Rigidbody.AddForce(Vector3.up * _gameplayData.PlayerJumpStrength, ForceMode.VelocityChange);
        }

        var slideChanged = _inputManager.SlideChange;
        if (slideChanged != 0)
        {
            if (_lastPositionX == null) _lastPositionX = _Rigidbody.position.x;
            
            var levelBounds = _LevelManager.GetLevelBounds();
            var currentPos = Mathf.InverseLerp( levelBounds.left, levelBounds.right, _lastPositionX.Value );
            var targetPos = currentPos + (slideChanged * _gameplayData.PlayerHorizontalSpeed);

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
