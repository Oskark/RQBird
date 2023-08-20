using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


public interface IHighScorable 
{
	public void RegisterScore( int score, out int position, out int lowestHighScoreValue );
	
	public int GetHighScorePosition( int score );
	public int GetLowestHighScoreValue();
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

[CreateAssetMenu( fileName = "HighScoresManager", menuName = "OneTimeScripts/HighScoresManager" )]
public class HighScoresManager : ScriptableObject, IHighScorable
{

	[SerializeField] private int _MaxHighScores = 10;
	
	private List<HighScoreEntry> _HighScores;

	public void Init(Action onInitComplete = null)
	{
		LoadSavesAsync( onInitComplete );
	}

	private void LoadSavesAsync(Action onInitComplete = null)
	{
		HighScoresSaveLoad.LoadScores().ContinueWith( result =>
		{
			_HighScores = result;
			
			onInitComplete?.Invoke();
		});
	}

	public void RegisterScore( int score, out int position, out int lowestHighScoreValue )
	{
		var highScorePosition = GetHighScorePosition( score );
		
		var isHighScore = highScorePosition >= 0 && highScorePosition < _MaxHighScores;
		if ( isHighScore )
		{
			_HighScores.Insert( highScorePosition, new HighScoreEntry( score, DateTime.Now.Ticks ) );

			var isOverScoresLimit = _HighScores.Count > _MaxHighScores;
			if (isOverScoresLimit)
			{
				_HighScores.RemoveAt( _HighScores.Count - 1 );
			}
			
			
			HighScoresSaveLoad.SaveScores( _HighScores ).Forget();

			position = highScorePosition;
			lowestHighScoreValue = -1;
			return;
		}

		position = -1;
		lowestHighScoreValue = _HighScores.Count > 0 ? _HighScores[^1].Score : -1;
	}

	public int GetHighScorePosition( int score )
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

	public int GetLowestHighScoreValue()
	{
		if (_HighScores.Count == 0) return -1;
		
		return _HighScores[^1].Score;
	}

	public List<HighScoreEntry> GetHighScores()
	{
		return _HighScores;
	}
}
