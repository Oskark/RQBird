
using UnityEngine;
using Zenject;

namespace Gameplay.Levels
{

	public class GameInstaller : MonoInstaller
	{
		[SerializeField] private PlayerController _PlayerController;

		[SerializeField] private GameInstaller _GameInstaller;
        
		[Inject] private static GameInstaller _Instance;
		
		public override void InstallBindings()
		{
			Container.BindInstance( _PlayerController ).AsSingle();
			
			Container.BindInstance( _GameInstaller ).AsSingle();
            
			Container.BindInterfacesAndSelfTo<InputManager>().FromNew().AsSingle();
            
			_Instance = this;
		}
        
		
		public static GameObject SpawnStatic( GameObject p )
		{
			return _Instance.SpawnInjectableObject( p );
		}

		public static void Resolve( LevelSegment segment )
		{
			_Instance.Container.Rebind<LevelSegment>().FromInstance( segment ).AsSingle();
		}

		
		public GameObject SpawnInjectableObject( GameObject prefab )
		{
			var p = Container.InstantiatePrefab( prefab );

			return p;
		}
		
		
	}
}
