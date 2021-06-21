using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpawnable
{
    event Action onDespawn;
}
