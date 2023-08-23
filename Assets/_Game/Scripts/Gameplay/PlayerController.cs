using System;
using Gameplay;
using Gameplay.Levels;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _Rigidbody;
    [SerializeField] private CapsuleCollider _collider;
    
    [Inject] private InputManager _inputManager;
    [Inject] private LevelManager _LevelManager;
    [Inject] private GameplayData _gameplayData;
    
    public event Action OnPlayerHitSegment;
    
    private bool _isPaused = false;
    
    private float? _lastPositionX = null;
    private SignalBus _signalBus;

    private float _leftBound;
    private float _rightBound;
    public Collider Collider => _collider;

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
            _isPaused = true;
            _Rigidbody.isKinematic = false; // Player can fall on failure
            
            return;
        }

        var  isDuringCountdown = newState.CurrentState == GameState.Countdown;
        if ( isDuringCountdown )
        {
            InitLevelData();
            SetPause( isPaused: true );
            return;
        }
        
        var isPlaying = newState.CurrentState == GameState.Play;
        
        SetPause( isPlaying == false );
    }

    private void InitLevelData()
    {
        var levelBounds = _LevelManager.GetLevelBounds();

        var colliderWidth  = _collider.bounds.size.x;
        
        _leftBound  = levelBounds.left  + colliderWidth;
        _rightBound = levelBounds.right - colliderWidth;
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
        var jumpWasPressed = _inputManager.WasJump;
        if (jumpWasPressed)
        {
            var wasFalling = _Rigidbody.velocity.y < 0;
            if (wasFalling)
            {
                ResetFallingVelocity();
            }
            
            PerformJump();
        }

        var slideChange = _inputManager.SlideChange;
        var slideWasPresent = slideChange != 0;
        
        if (slideWasPresent)
        {
            PerformSlide( slideChange );
        }
        else
        {
            RemoveLastPosition();
        }
    }

    private void PerformSlide( float slideChange )
    {
        if ( _lastPositionX == null ) _lastPositionX = _Rigidbody.position.x;

        // Where are we in the level? [0..1]
        var currentPos = Mathf.InverseLerp( _leftBound, _rightBound, _lastPositionX.Value );
        // Where we should be in the level?
        var targetPos = currentPos + (slideChange * _gameplayData.PlayerHorizontalSpeed);
        // Clamp to [0..1]
        targetPos = Mathf.Clamp( targetPos, -1f, 1 );
        // Change to local pos
        var newPosition = Mathf.Lerp( _leftBound, _rightBound, targetPos );
        // Move
        _Rigidbody.position = _Rigidbody.position.With( x: newPosition );
    }
    
    private void RemoveLastPosition()
    {
        _lastPositionX = null;
    }
    
    private void PerformJump()
    {
        _Rigidbody.AddForce( Vector3.up * _gameplayData.PlayerJumpStrength, ForceMode.VelocityChange );
    }

    private void ResetFallingVelocity()
    {
        _Rigidbody.velocity = _Rigidbody.velocity.With( y: 0 );
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
