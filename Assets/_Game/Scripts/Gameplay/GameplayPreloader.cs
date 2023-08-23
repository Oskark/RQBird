using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gameplay;
using UnityEngine;
using Zenject;

public class GameplayPreloader : MonoBehaviour
{
    [Inject] private GameplayElementsProvider _Provider;

	private bool _alreadyPreloaded = false;
	private Action _notifyOnFinish;
	
    public void BeginPreloading(  )
	{
		if (_alreadyPreloaded)
		{
			return;
		}

		PreloadAsync(  ).Forget();
	}

	public void MakeSurePreloadCompletedAndThen( Action onPreloaded )
	{
		if (_alreadyPreloaded)
		{
			onPreloaded?.Invoke();
			return;
		}

		_notifyOnFinish += onPreloaded;
	}
    
    private async UniTask PreloadAsync(  )
    {
		await UniTask.RunOnThreadPool( _Provider.PreloadElements );

		_alreadyPreloaded = true;
		
		_notifyOnFinish?.Invoke();
		_notifyOnFinish = null;
	}
}
