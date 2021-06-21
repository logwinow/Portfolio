using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using  System.Reflection;

public class SuitLoadController : MonoBehaviour
{
    [SerializeField] private RopeController _ropeController;
    [SerializeField] private GameObject _firstSuit;
    [SerializeField] private GameObject _secondSuit;
    [SerializeField] private Texture _firstSuitRopeTex;
    [SerializeField] private Texture _secondSuitRopeTex;
    [SerializeField] private SectionID _selectedSuitID;
    [SerializeField] private CameraSmoothFollow _smoothFollow;
    [SerializeField] private DepthController _depthController;

    private void Awake()
    {
        GameSaver.onLoadAfterCheck += OnLoad;
    }

    private void OnDestroy()
    {
        GameSaver.onLoadAfterCheck -= OnLoad;
    }

    private void OnLoad()
    {
        var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        var lineRend = typeof(RopeController)
            .GetField("lineRend", bindingFlags)!
            .GetValue(_ropeController) as LineRenderer;
        GameObject selectedSuit = _firstSuit;

        switch (GameManager.Instance.GetParameter(_selectedSuitID).Value)
        {
            case 1:
                lineRend!.material.SetTexture("_MainTex", _firstSuitRopeTex);
                selectedSuit = _firstSuit;
                break;
            case 2:
                lineRend!.material.SetTexture("_MainTex", _secondSuitRopeTex);
                selectedSuit = _secondSuit;;
                break;
            default:
                Debug.LogWarning("Incorrect selected suit index");
                break;
        }
        
        typeof(RopeController).GetField("playerDockRb", bindingFlags)!.SetValue(_ropeController,
            selectedSuit.GetComponentsInChildren<Rigidbody2D>(true).First(rb => rb.isKinematic));
        
        _ropeController.gameObject.SetActive(true);
        selectedSuit.SetActive(true);

        _smoothFollow.StopSmoothing();

        var camTarg = (Transform)typeof(PlayerController)
                .GetField("_cameraTarget", bindingFlags)!
            .GetValue(selectedSuit.GetComponent<PlayerController>());
        _smoothFollow.StartSmoothing(camTarg);
        _smoothFollow.Target = camTarg;
        
        typeof(DepthController).GetField("_target", bindingFlags)!
                .SetValue(_depthController, camTarg);
    }
}
