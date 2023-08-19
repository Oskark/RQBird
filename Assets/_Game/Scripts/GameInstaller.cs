
using UnityEngine;
using Zenject;

namespace Gameplay.Levels
{
	public class GameInstaller : MonoInstaller
	{

		[SerializeField] private LevelManager _LevelManager;
		
		public override void InstallBindings()
		{
			Container.Bind<ILevelManagerable>().To<LevelManager>().FromComponentsInHierarchy(  ).AsTransient();
			// Container.BindInstance( _LevelManager ).AsSingle();
		}
		
		
	}
}
