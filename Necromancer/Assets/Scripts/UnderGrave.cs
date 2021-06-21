using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderGrave : Interactable
{
    Skeleton skeleton;
    bool initialized = false;
    public bool IsUnnamedGrave { get; set; }

    public override void Interact()
    {
        if (!initialized)
        {
            skeleton = new Skeleton(GetComponentInParent<GraveDirtController>().Character);

            initialized = true;
        }

        if (GameManager.Instance.IsCurrentShovel)
        {
            GameManager.Instance.Audio.PlayOneShot(GameManager.Instance.tombstoneGraveInteractSound[Random.Range(0, GameManager.Instance.tombstoneGraveInteractSound.Length)]);

            GameManager.Instance.SetVisibleUnderGravePopup();
            
            GameManager.Instance.InitializeUnderGravePopup(skeleton);
        }
        else if (GameManager.Instance.UnitsCount != 10)
        {
            if (skeleton.character.id == 10)
            {
                GameManager.Instance.Audio.PlayOneShot(GameManager.Instance.calebSound, .5f);
            }

            GameManager.Instance.AddUnitIcon(skeleton);

            GameManager.Instance.SetCursor(CursorType.Default);

            GameObject go = Instantiate(Resources.Load<GameObject>("SoulSphere"));
            go.transform.position = transform.position + Vector3.up * 5f;

            Destroy(this);
        }
    }
}
