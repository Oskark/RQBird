using Gameplay;
using Gameplay.Levels;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using Zenject;

[CreateAssetMenu(fileName = "ScriptableObjectInstaller", menuName = "Installers/ScriptableObjectInstaller")]
public class PersistentsInstaller : ScriptableObjectInstaller<PersistentsInstaller>
{

    [FormerlySerializedAs( "_GameplayData" ),SerializeField] private GameplayConfig _GameplayConfig;
    
    [SerializeField] private AssetReference _ElementsContainerRef;
    
    private static bool _alreadyInstalled = false;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetStatics()
    {
        _alreadyInstalled = false;
    }
    
    public override void InstallBindings()
    {
        if ( _alreadyInstalled ) return;
        
        SignalBusInstaller.Install( Container );

        Container.DeclareSignal<GameplayStateChangedSignal>();
        Container.DeclareSignal<RestartGameSignal>();
        Container.DeclareSignal<ExitGameplaySignal>();

        Container.Bind<PersistentsInstaller>().FromScriptableObject( this ).AsSingle();
        
        Container.BindInstance( _ElementsContainerRef ).WithId( "ElementsContainerRef" );
        Container.BindInstance( _GameplayConfig ).AsSingle();

        Container.BindInterfacesAndSelfTo<LevelManager>().FromNew().AsSingle();
        Container.BindInterfacesAndSelfTo<LevelGenerator>().FromNew().AsSingle();
        Container.BindInterfacesAndSelfTo<GameplayElementsProvider>( ).FromNew().AsSingle();

        Container.Bind<IHighScorable>().To<HighScoresManager>().FromNew().AsSingle();
        
        Container.Bind<ILevelSegmentSpawner>().WithId( LevelGenerator.FLOOR_SPAWNER_ID ).To<FloorSpawner>().FromNew( ).AsSingle();
        Container.Bind<ILevelSegmentSpawner>().WithId( LevelGenerator.OBSTACLE_SPAWNER_ID ).To<ObstacleSpawner>().FromNew( ).AsSingle();

        _alreadyInstalled = true;
    }

    public GameObject SpawnInjectableObject( GameObject gameObject )
    {
        var p = Container.InstantiatePrefab( gameObject );
        Container.InjectGameObject( p );

        return p;
    }
}
