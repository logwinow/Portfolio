using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GraveDirtController : MonoBehaviour
{
    [SerializeField]
    float timeBeforeDissolve = 1f;
    [SerializeField]
    float durationDissolve = 1f;
    [SerializeField]
    int health = 3;
    [SerializeField]
    GameObject dirtSplitPrefab;
    public GameObject dirtWhole;
    [SerializeField]
    Transform tombstonePoint;
    [SerializeField]
    float explosionForce = 4f;

    Character character;
    public Character Character
    {
        get
        {
            if (character == null)
            {
                DefineCharacter();
            }

            return character;
        }
    }


    Material mat;
    MeshRenderer meshRend;
    bool isInit = false;
    bool characterDefined = false;

    private void Init()
    {
        meshRend = dirtWhole.gameObject.GetComponent<MeshRenderer>();
        mat = new Material(meshRend.material);
        meshRend.material = mat;

        isInit = true;
    }

    public void DefineCharacter()
    {
        if (!characterDefined)
        {
            character = GameManager.Instance.TakeCharacter();

            characterDefined = true;
        }
    }

    private void Start()
    {
        int num = Random.Range(1, 8);
        if (num == 6)
        {
            if (GameManager.Instance.noNamedGraveCount == 2)
                num = Random.Range(1, 7);
            else
                GameManager.Instance.noNamedGraveCount++;
        }

        GameObject go = Instantiate(Resources.Load<GameObject>("Tombstones/Tombstone " + num));
        go.transform.position = tombstonePoint.position;
        go.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * go.transform.rotation;
        go.transform.parent = transform;
    }
    private IEnumerator DissolveThresholdAdjustment(GameObject splittedDirt)
    {
        MeshRenderer[] meshR = splittedDirt.GetComponentsInChildren<MeshRenderer>();
        float timer = 0;

        foreach (var mr in meshR)
        {
            mr.gameObject.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, splittedDirt.transform.position, 0);
        }

        while ((timer += Time.deltaTime) < timeBeforeDissolve)
        {
            yield return new WaitForEndOfFrame();
        }

        timer = 0;

        while ((timer += Time.deltaTime) < durationDissolve)
        {
            for (int i = 0; i < meshR.Length; i++)
            {
                meshR[i].material.SetFloat("_Threshold", timer / durationDissolve);
            }
            yield return new WaitForEndOfFrame();
        }

        Destroy(meshR[0].transform.parent.gameObject);
    }

    public void OnDig()
    {
        if (!isInit)
            Init();

        health--;
        mat.SetFloat("_Step", 0.33f * (3 - health));

        if (health == 0)
        {
            GameManager.Instance.Audio.PlayOneShot(GameManager.Instance.dirtDestroiedSound);
            GetComponent<BoxCollider>().enabled = true;
            DirtSplitting();
        }
    }

    private void DirtSplitting()
    {
        GameObject go = Instantiate(dirtSplitPrefab);
        go.transform.position = dirtWhole.transform.position;
        go.transform.rotation = transform.rotation;
        go.transform.parent = this.transform;

        Destroy(dirtWhole);

        Rigidbody[] meshR = go.GetComponentsInChildren<Rigidbody>();

        StartCoroutine(DissolveThresholdAdjustment(go));
    }
}
