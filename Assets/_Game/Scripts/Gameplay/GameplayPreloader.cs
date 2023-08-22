using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using Zenject;

public class GameplayPreloader : MonoBehaviour
{
    [Inject] private GameplayElementsProvider _Provider;

    private void Start()
    {
        _Provider.PreloadElements( null );
    }
}
