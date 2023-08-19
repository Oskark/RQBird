
using System;
using TMPro;
using UnityEngine;

public class HighScoreDisplayerEntry : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _NumberText;
	[SerializeField] private TextMeshProUGUI _Score;
	[SerializeField] private TextMeshProUGUI _Time;
    
	public void SetData(int number, HighScoreEntry entry)
	{
		_NumberText.text = number.ToString();
		_Score.text = entry.Score.ToString();
		_Time.text = TimeSpan.FromTicks( (long) entry.ObtainDate ).ToString( );	
	}
}
