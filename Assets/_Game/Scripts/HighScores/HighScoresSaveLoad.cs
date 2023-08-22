
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Cysharp.Threading.Tasks;
	using Newtonsoft.Json;
	using UnityEngine;

	[CreateAssetMenu( fileName = "HighScoresSaveLoad", menuName = "OneTimeScripts/HighScoresSaveLoad" )]
	public class HighScoresSaveLoad : ScriptableObject
	{

		private static string FilePath => Application.persistentDataPath + "/HighScores.json";
		
		
		public static async UniTask<bool> SaveScores(List<HighScoreEntry> scores)
		{
			Debug.Log($"Saving high scores"  );

			var filePath = FilePath;
			var serialized = JsonConvert.SerializeObject( scores );

			try
			{
				await File.WriteAllTextAsync( filePath, serialized ).AsUniTask(  );
			}
			catch ( Exception e )
			{
				Debug.LogError( e.Message );
                
				return false;
			}

			return true;
		}

		public static async UniTask<List<HighScoreEntry>> LoadScores()
		{
			var filePath = FilePath;
			
			var  noSaveFile = File.Exists( filePath ) == false;
			if ( noSaveFile )
			{
				Debug.LogWarning($"No save file found, returning empty array"  );
				return new List<HighScoreEntry>();
			}

			var content = await File.ReadAllTextAsync( filePath );
			
			var  contentIsEmpty = string.IsNullOrEmpty( content );
			if ( contentIsEmpty )
			{
				Debug.LogWarning($"Successfully loaded, but content is empty, returning empty array"  );
				return new List<HighScoreEntry>();
			}


			UniTask.ReturnToMainThread();

			var loadedContent = JsonConvert.DeserializeObject<List<HighScoreEntry>>( content );
            return loadedContent;
		}

		[ContextMenu( "Test Load" )]
		public void Test_Load()
		{
			LoadScores().ContinueWith( result =>
			{
				Debug.Log($"Obtained {result.Count} entries"  );
				for (int i = 0; i < result.Count; i++)
				{
					Debug.Log($"Entry {i}: {result[i].Score} - {result[i].ObtainDate}"  );
				}
			} );
		}

		[ContextMenu( "Test Save" )]
		public void Test_Save()
		{
			var randomContent = new List<HighScoreEntry>();
			var amountOfDatas = 20;

			for ( int i = 0; i < amountOfDatas; i++ )
			{
				randomContent.Add( new HighScoreEntry(amountOfDatas - i, DateTime.Now.Ticks) );
			}

			SaveScores( randomContent ).ContinueWith( result =>
			{
				Debug.Log($"Result is: {result}"  );
			} );
		}
		
		[ContextMenu("Remove all saves")]
		public void Test_RemoveAllSaves()
		{
			var filePath = FilePath;
			if (File.Exists(filePath)) File.Delete( filePath );
		}
		
		
	}
