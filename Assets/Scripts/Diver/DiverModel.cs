using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DiverModel : SingletonMonoBehaviour<DiverModel>
{
    public Health health { get; private set; }
    public Collider2D collider { get; private set; }
    public int coinsCollected { get; private set; } = 0;

    public float globalDiveSpeed => baseGlobalDiveSpeed + LevelManager.Instance.curOceanicZone.globalDiveSpeedModifier;

    public GameObject flashlight;

    [SerializeField] private float baseGlobalDiveSpeed = 1f;

    public float flashlightMaxGlobalLightIntensity = 0.4f;

    public CameraShakeProps cameraShakeProps;

    new void Awake()
    {
        base.Awake();
        health = GetComponent<Health>();
        collider = GetComponent<Collider2D>();

        LevelManager.Instance.onNewOceanicZoneEntered += OnNewZoneEntered;
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
    }

    void OnNewZoneEntered()
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
}