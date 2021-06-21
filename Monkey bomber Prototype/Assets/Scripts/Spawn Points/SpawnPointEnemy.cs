using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointEnemy : SpawnPoint
{
    protected override void Set()
    {
        Set(EnemiesManager.Instance.GetFromPool());
    }
}
