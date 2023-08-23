
	using System;
	using System.Collections.Generic;
	using Gameplay;
	using UnityEngine;
	using Zenject;

	public class HighScoresDisplayer : MonoBehaviour
	{
		[Header("Neccessary refs")]
		[SerializeField] private MainMenuController _MainMenuController;

		[SerializeField] private Transform _SpawnContainer;
		[SerializeField] private HighScoreDisplayerEntry _HighScoreEntryPrefab;

		[Inject] private IHighScorable _highScoresManager;
		[Inject] private GameplayConfig _gameplayConfig;
		
		private readonly List<HighScoreDisplayerEntry> _spawnedInstances = new List<HighScoreDisplayerEntry>();
		
		public void Show()
		{
            _highScoresManager.GetHighScores( onEntriesObtained: DisplayScores );
		}

		private void OnDisable()
		{
			CleanUp();
		}

		private void CleanUp()
		{
			foreach ( var spawnedInstance in _spawnedInstances )
			{
				Destroy( spawnedInstance.gameObject );
			}
			
			_spawnedInstances.Clear();
		}
        
		private void DisplayScores( List<HighScoreEntry> highScores )
		{
			var maxDisplayedAmount = _gameplayConfig.SavedHighScoresAmount;
			
			for (int i = 0; i < highScores.Count; i++)
			{
				if ( i >= maxDisplayedAmount )
				{
					break;
				}
				
				var highScoreEntry = highScores[i];
				var spawnedInstance = Instantiate( _HighScoreEntryPrefab, _SpawnContainer );
				spawnedInstance.SetData( i + 1, highScoreEntry );
				
				_spawnedInstances.Add( spawnedInstance );
			}
		}

		public void Evt_BackToMenu()
		{
			CleanUp();
			
			_MainMenuController.Evt_OpenMainMenu();
		}

	}
