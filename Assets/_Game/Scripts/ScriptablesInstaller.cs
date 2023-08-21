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
        Container.BindInstance( _ElementsProvider ).AsSingle();
    }
}
