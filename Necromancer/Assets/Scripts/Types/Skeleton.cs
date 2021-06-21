using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Skeleton
{
    public Skeleton (Character c) : this()
    {
        shieldType = ShieldType.None;
        subjectClassifier = SubjectClassifierType.None;
        weaponType = WeaponType.None;
        meleeWeapon = MeleeWeaponType.None;
        arhcerWeapon = ArhcerWeaponType.None;
        mageWeapon = MageWeaponType.None;
        this.character = c;
        weapon = 0;

        if (c.healthType == HealthType.High)
            pose = 4;
        else if (c.healthType == HealthType.Middle)
            pose = Random.Range(2, 4);
        else
            pose = 1;

        switch(c.careerType)
        {
            case CareerType.Smith:
                subjectClassifier = (SubjectClassifierType)(Random.Range(-1, 1));
                break;
            case CareerType.Melee:
                meleeWeapon = (MeleeWeaponType)(Random.Range(-1, 3));
                if (meleeWeapon != MeleeWeaponType.None)
                {
                    weaponType = WeaponType.Melee;
                    weapon = (int)meleeWeapon;
                }
                shieldType = (ShieldType)(Random.Range(-1, 2));
                break;
            case CareerType.Archer:
                arhcerWeapon = (ArhcerWeaponType)(Random.Range(-1, 2));
                if (arhcerWeapon != ArhcerWeaponType.None)
                {
                    weaponType = WeaponType.Archer;
                    weapon = (int)arhcerWeapon;
                }
                shieldType = (ShieldType)(Random.Range(-1, 1));
                break;
            case CareerType.Mage:
                mageWeapon = (MageWeaponType)(Random.Range(-1, 2));
                if (mageWeapon != MageWeaponType.None)
                {
                    weaponType = WeaponType.Mage;
                    weapon = (int)mageWeapon;
                }
                break;
            default:
                break;
        }
    }

    public Character character;

    public int pose;
    public int weapon;
    public ShieldType shieldType;
    public SubjectClassifierType subjectClassifier;
    public WeaponType weaponType;
    public MeleeWeaponType meleeWeapon;
    public ArhcerWeaponType arhcerWeapon;
    public MageWeaponType mageWeapon;
}
