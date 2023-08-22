using Gameplay;
using Gameplay.Levels;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ScriptableObjectInstaller", menuName = "Installers/ScriptableObjectInstaller")]
public class ScriptablesInstaller : ScriptableObjectInstaller<ScriptablesInstaller>
{
    [SerializeField] private GameplayElementsProvider _ElementsProvider;
    
    public override void InstallBindings()
    {
        

        Container.Bind<ScriptablesInstaller>().FromScriptableObject( this ).AsSingle();
        Container.BindInstance( _ElementsProvider ).AsSingle();
    }

    public GameObject SpawnInjectableObject( GameObject gameObject )
    {
        var p = Container.InstantiatePrefab( gameObject );
        Container.InjectGameObject( p );

        return p;
    }
}
