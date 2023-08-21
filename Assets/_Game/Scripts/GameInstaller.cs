
using UnityEngine;
using Zenject;

namespace Gameplay.Levels
{

	public interface ISpawnInjectable
	{
		GameObject SpawnInjectableObject( GameObject prefab );
	}
	
	public class GameInstaller : MonoInstaller, ISpawnInjectable
	{
		[SerializeField] private GameplayData _GameplayData;
		[SerializeField] private InputManager _InputManager;
		[SerializeField] private LevelManager _LevelManager;
		[SerializeField] private LevelGenerator _LevelGenerator;

		[SerializeField] private GameInstaller _GameInstaller;
        
		public override void InstallBindings()
		{
			Container.BindInstance( _GameplayData ).AsSingle();
			Container.BindInstance( _InputManager ).AsSingle();
			Container.BindInstance( _LevelManager ).AsSingle();
			Container.BindInstance( _LevelGenerator ).AsSingle();
			
			Container.BindInstance( _GameInstaller ).AsSingle();
			Container.Bind<ISpawnInjectable>().FromInstance( _GameInstaller ).AsSingle();
        }

		public GameObject SpawnInjectableObject( GameObject prefab )
		{
			var p = Container.InstantiatePrefab( prefab );
			Container.InjectGameObject( p );

			return p;
		}
		
		
	}
}
