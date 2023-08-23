using UnityEngine;
using Zenject;

public class MainMenuInstaller : MonoInstaller
{
    [SerializeField] private GameplayPreloader _GameplayPreloader;
    
    public override void InstallBindings()
    {
        Container.BindInstance( _GameplayPreloader ).AsSingle();
    }
}
