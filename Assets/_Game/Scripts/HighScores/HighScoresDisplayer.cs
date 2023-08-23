
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using Zenject;

	public class HighScoresDisplayer : MonoBehaviour
	{

		[Header("Settings")]
		[SerializeField] private int _MaxDisplayedAmount = 10;
		
		[Header("Neccessary refs")]
		[SerializeField] private MainMenuController _MainMenuController;

		[SerializeField] private Transform _SpawnContainer;
		[SerializeField] private HighScoreDisplayerEntry _HighScoreEntryPrefab;

		[Inject] private IHighScorable _HighScoresManager;
		
		
		private readonly List<HighScoreDisplayerEntry> _spawnedInstances = new List<HighScoreDisplayerEntry>();
		
		public void Show()
		{
            _HighScoresManager.GetHighScores( onEntriesObtained: DisplayScores );
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
			for (int i = 0; i < highScores.Count; i++)
			{
				if ( i >= _MaxDisplayedAmount )
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
