using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField]
    protected DragonBonesAnimationController animationController;

    public DragonBonesAnimationController DBAnimationController => animationController;
}
