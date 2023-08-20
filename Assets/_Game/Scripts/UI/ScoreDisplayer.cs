using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ScoreDisplayer : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _Text;
	[SerializeField] private Gameplay.Levels.LevelManager _LevelManager;

	private StringBuilder _StringBuilder = new StringBuilder();
	
	private void Update()
	{
		_StringBuilder.Clear();
		_StringBuilder.Append( Mathf.FloorToInt(_LevelManager.DistancePassed) );
		
		_Text.text = (_StringBuilder.ToString());
	}
}