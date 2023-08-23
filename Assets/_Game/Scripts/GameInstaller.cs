
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
		// [SerializeField] private InputManager _InputManager;
		[SerializeField] private LevelManager _LevelManager;
		[SerializeField] private LevelGenerator _LevelGenerator;

		[SerializeField] private GameInstaller _GameInstaller;
        
		[Inject] private static GameInstaller _Instance;
		
		public override void InstallBindings()
		{
			Container.BindInstance( _LevelManager ).AsSingle();
			Container.BindInstance( _LevelGenerator ).AsSingle();
			
			Container.BindInstance( _GameInstaller ).AsSingle();
			
			Container.Bind<ISpawnInjectable>().FromInstance( _GameInstaller ).AsSingle();
			Container.Bind<ILevelSegmentSpawner>().WithId( "FloorSpawner" ).To<FloorSpawner>().FromNew( ).AsSingle();
			Container.Bind<ILevelSegmentSpawner>().WithId( "ObstacleSpawner" ).To<ObstacleSpawner>().FromNew( ).AsSingle();
			
			Container.BindInterfacesAndSelfTo<InputManager>().FromNew().AsSingle();

			// SignalBusInstaller.Install( Container );
			//
			// Container.DeclareSignal<GameplayStateChangedSignal>();
			
			_Instance = this;
		}
        
		
		public static GameObject SpawnStatic( GameObject p )
		{
			return _Instance.SpawnInjectableObject( p );
		} 

		
		public GameObject SpawnInjectableObject( GameObject prefab )
		{
			var p = Container.InstantiatePrefab( prefab );
			// Container.InjectGameObject( p );

			return p;
		}
		
		
	}
}
