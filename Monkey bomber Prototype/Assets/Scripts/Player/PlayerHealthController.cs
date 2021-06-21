using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : HealthController
{
    protected override void Dead()
    {
        print("Player dead");
    }
}
