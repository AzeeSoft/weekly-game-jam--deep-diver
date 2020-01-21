﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DiverModel : SingletonMonoBehaviour<DiverModel>
{
    public Health health { get; private set; }
    public Collider2D collider { get; private set; }
    public int coinsCollected { get; private set; } = 0;

    public float globalDiveSpeed => LevelManager.Instance.hasReachedOceanFloor
        ? 0
        : baseGlobalDiveSpeed + LevelManager.Instance.curOceanicZone.globalDiveSpeedModifier;

    public GameObject flashlight;

    [SerializeField] private float baseGlobalDiveSpeed = 1f;

    public float flashlightMaxGlobalLightIntensity = 0.4f;

    public CameraShakeProps cameraShakeProps;

    private float _origFlashlightIntensity = 1;

    new void Awake()
    {
        base.Awake();
        health = GetComponent<Health>();
        collider = GetComponent<Collider2D>();

        LevelManager.Instance.onNewOceanicZoneEntered += OnNewZoneEntered;
        LevelManager.Instance.onOceanFloorReached += OnOceanFloorReached;

    }

    // Start is called before the first frame update
    void Start()
    {
        health.OnDamageTaken.AddListener(() =>
        {
            var tween = CameraRigManager.Instance.cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.transform
                .DOShakePosition(
                    cameraShakeProps.camShakeDuration, cameraShakeProps.camShakeStrength,
                    cameraShakeProps.camShakeVibrato, cameraShakeProps.camShakeRandomness);
            tween.Play();
        });
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFlashlight();
    }

    void OnDestroy()
    {
        LevelManager.Instance.onNewOceanicZoneEntered -= OnNewZoneEntered;
        LevelManager.Instance.onOceanFloorReached -= OnOceanFloorReached;
    }

    void OnNewZoneEntered()
    {
    }

    void OnOceanFloorReached()
    {

    }

    public void UpdateCoins(int deltaCoins)
    {
        coinsCollected += deltaCoins;
    }

    void UpdateFlashlight()
    {
        flashlight.SetActive(LevelManager.Instance.globalLight2D.intensity <= flashlightMaxGlobalLightIntensity);
    }

    public void FoundDeepSeaMysteryObject(DeepSeaMysteryEntity deepSeaMysteryEntity)
    {
        LevelManager.Instance.OceanFloorReached();
    }
}