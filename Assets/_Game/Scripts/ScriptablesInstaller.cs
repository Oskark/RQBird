using Gameplay;
using Gameplay.Levels;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

[CreateAssetMenu(fileName = "ScriptableObjectInstaller", menuName = "Installers/ScriptableObjectInstaller")]
public class ScriptablesInstaller : ScriptableObjectInstaller<ScriptablesInstaller>
{
    [SerializeField] private AssetReference _ElementsContainerRef;
    [SerializeField] private HighScoresManager _HighScoresManager;
    
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

        Container.Bind<ScriptablesInstaller>().FromScriptableObject( this ).AsSingle();
        Container.Bind<GameplayElementsProvider>( ).FromNew().AsSingle();
        Container.BindInstance( _ElementsContainerRef ).WithId( "ElementsContainerRef" );

        Container.BindInstance( _HighScoresManager ).AsSingle();
        
        _alreadyInstalled = true;
    }

    public GameObject SpawnInjectableObject( GameObject gameObject )
    {
        var p = Container.InstantiatePrefab( gameObject );
        Container.InjectGameObject( p );

        return p;
    }
}
