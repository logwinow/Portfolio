using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TombstoneLabel : Interactable
{
    public override void Interact()
    {
        GameManager.Instance.Audio.PlayOneShot(GameManager.Instance.tombstoneLabelInteractSound);
        GameManager.Instance.SetVisibleLabelPopup();
        GameManager.Instance.InitializeLabelPopup(GetComponentInParent<GraveDirtController>().Character);
    }
}
