
using UnityEngine;
using Zenject;

namespace Gameplay.Levels
{
	public class GameInstaller : MonoInstaller
	{
		[SerializeField] private GameplayData _GameplayData;
		[SerializeField] private InputManager _InputManager;
		[SerializeField] private LevelManager _LevelManager;

		[SerializeField] private GameInstaller _GameInstaller;

		// private PrefabFactory<LevelSegment> _SegmentFactory = new PrefabFactory<LevelSegment>();
		
		public override void InstallBindings()
		{
			Container.BindInstance( _GameplayData ).AsSingle();
			Container.BindInstance( _InputManager ).AsSingle();
			Container.BindInstance( _LevelManager ).AsSingle();
			
			Container.BindInstance( _GameInstaller ).AsSingle();

			
		}

		public GameObject SpawnInjectableObject( GameObject prefab )
		{
			var p = Container.InstantiatePrefab( prefab );
			Container.InjectGameObject( p );

			return p;
		}
		
		
	}
}
