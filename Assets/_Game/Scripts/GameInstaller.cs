
using UnityEngine;
using Zenject;

namespace Gameplay.Levels
{

	public class GameInstaller : MonoInstaller
	{
		[SerializeField] private PlayerController _PlayerController;
		[SerializeField] private LevelManager _LevelManager;
		[SerializeField] private LevelGenerator _LevelGenerator;

		[SerializeField] private GameInstaller _GameInstaller;
        
		[Inject] private static GameInstaller _Instance;
		
		public override void InstallBindings()
		{
			Container.BindInstance( _PlayerController ).AsSingle();
			Container.BindInstance( _LevelManager ).AsSingle();
			Container.BindInstance( _LevelGenerator ).AsSingle();
			
			Container.BindInstance( _GameInstaller ).AsSingle();
			
			Container.Bind<ILevelSegmentSpawner>().WithId( "FloorSpawner" ).To<FloorSpawner>().FromNew( ).AsSingle();
			Container.Bind<ILevelSegmentSpawner>().WithId( "ObstacleSpawner" ).To<ObstacleSpawner>().FromNew( ).AsSingle();
			
			Container.BindInterfacesAndSelfTo<InputManager>().FromNew().AsSingle();
            
			_Instance = this;
		}
        
		
		public static GameObject SpawnStatic( GameObject p )
		{
			return _Instance.SpawnInjectableObject( p );
		} 

		
		public GameObject SpawnInjectableObject( GameObject prefab )
		{
			var p = Container.InstantiatePrefab( prefab );

			return p;
		}
		
		
	}
}
