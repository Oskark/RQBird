using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

public delegate void HighScoreRegisteredDelegate( int position, int leastHighScoreValue );
public delegate void HighScoresObtainedDelegate( List<HighScoreEntry> entries );

public interface IHighScorable 
{
	public void RegisterScoreAndThen( int score, HighScoreRegisteredDelegate onScoreRegistered );
	public void GetHighScores (HighScoresObtainedDelegate onEntriesObtained);
}

[Serializable]
public class HighScoreEntry
{
	public int Score;
	public double ObtainDate;

	public HighScoreEntry( int score, double obtainDate )
	{
		Score = score;
		ObtainDate = obtainDate;
	}
}


[UsedImplicitly]
public class HighScoresManager : IHighScorable
{
	[Inject] private GameplayConfig _gameplayConfig;
	
	private List<HighScoreEntry> _HighScores;

	
	public void RegisterScoreAndThen(int score, HighScoreRegisteredDelegate onScoreRegistered )
	{
		if ( _HighScores == null )
		{
			LoadSavesAsync( () => RegisterScore( score, onScoreRegistered )  );
			return;
		}
		
		RegisterScore( score, onScoreRegistered );
    }
    
	public void GetHighScores( HighScoresObtainedDelegate onEntriesObtained )
	{
		if ( _HighScores == null )
		{
			LoadSavesAsync( () => onEntriesObtained?.Invoke( _HighScores ) );
			return;
		}
		
		onEntriesObtained?.Invoke( _HighScores );
	}

	
	
	private void LoadSavesAsync(Action onInitComplete = null)
	{
		HighScoresSaveLoad.LoadScores().ContinueWith( result =>
		{
			_HighScores = result;
			
			onInitComplete?.Invoke();
		});
	}
	
	private void RegisterScore( int score, HighScoreRegisteredDelegate onScoreRegistered)
	{
		int position;
		int lowestHighScoreValue;
		var highScoresAmount = _gameplayConfig.SavedHighScoresAmount;

		var scorePosition = GetHighScorePosition( score );
        
		var isHighScore = scorePosition >= 0 && scorePosition < highScoresAmount;
		if ( isHighScore )
		{
			_HighScores.Insert( scorePosition, GenerateHighScoreEntry( score ) );

			var isOverScoresLimit = _HighScores.Count > highScoresAmount;
			if (isOverScoresLimit)
			{
				_HighScores.RemoveAt( _HighScores.Count - 1 );
			}
			
			
			HighScoresSaveLoad.SaveScores( _HighScores ).Forget();

			position = scorePosition;
			lowestHighScoreValue = -1;
			
			onScoreRegistered?.Invoke( position, lowestHighScoreValue );
			return;
		}

		position = -1;
		lowestHighScoreValue = _HighScores.Count > 0 ? _HighScores[^1].Score : -1;
		
		onScoreRegistered?.Invoke( position, lowestHighScoreValue );
	}

	
	private static HighScoreEntry GenerateHighScoreEntry( int score )
	{
		return new HighScoreEntry( score, DateTime.Now.Ticks );
	}

	private int GetHighScorePosition( int score )
	{
		for ( int i = 0; i < _HighScores.Count; i++ )
		{
			var currentScore = _HighScores[i].Score;
			if ( score > currentScore )
			{
				return i;
			}
		}

		return _HighScores.Count;
	}
    
}
