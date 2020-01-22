using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class DiverModel : SingletonMonoBehaviour<DiverModel>
{
    public Health health { get; private set; }
    public Collider2D collider { get; private set; }
    public int coinsCollected { get; private set; } = 0;
    public bool isShieldActive => shield.color.a > 0;

    public float globalDiveSpeed => LevelManager.Instance.hasReachedOceanFloor
        ? 0
        : baseGlobalDiveSpeed + LevelManager.Instance.curOceanicZone.globalDiveSpeedModifier;

    public GameObject flashlight;
    public SpriteRenderer shield;

    [SerializeField] private float baseGlobalDiveSpeed = 1f;

    public float flashlightMaxGlobalLightIntensity = 0.4f;
    public float shieldFadeDuration = 3f;

    public CameraShakeProps cameraShakeProps;
    public AudioClip hitSound;

    private float _origFlashlightIntensity = 1;

    private float timeSinceShieldActivated;
    private float curShieldDuration;

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
            SoundEffectsManager.Instance.Play(hitSound);

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
        UpdateShield();
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

    void UpdateShield()
    {
        var newScale = shield.transform.localScale;
        newScale.x = Mathf.Sign(transform.localScale.x) * Mathf.Abs(newScale.x);

        shield.transform.localScale = newScale;

        if (isShieldActive)
        {
            timeSinceShieldActivated += Time.deltaTime;

            if (!DOTween.IsTweening(shield))
            {
                var newColor = shield.color;
                newColor.a = Mathf.Clamp01(HelperUtilities.Remap(timeSinceShieldActivated, 0, curShieldDuration, 1, 0));
                shield.color = newColor;
            }
        }

        health.canTakeDamage = !isShieldActive;
    }

    public void FoundDeepSeaMysteryObject(DeepSeaMysteryEntity deepSeaMysteryEntity)
    {
        LevelManager.Instance.OceanFloorReached();
    }

    public void ActivateShield(float duration)
    {
        timeSinceShieldActivated = 0;
        curShieldDuration = duration;

        shield.DOKill();
        shield.DOFade(1, shieldFadeDuration).Play();
    }
}