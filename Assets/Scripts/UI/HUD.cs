using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI depthText;

    public float zoneEnteredScreenDisplayDuration = 2f;
    public float zoneEnteredScreenFadeDuration = 1f;
    public GameObject zoneEnteredScreen;
    public TextMeshProUGUI zoneEnteredText;

    public Image healthBarImage;
    public TextMeshProUGUI coinsText;

    public Image damageIndicatorImage;
    public float damageIndicatorMaxOpacity = 0.7f;
    public float damageIndicatorAnimationDuration = 0.3f;

    public GameObject winScreen;
    public GameObject loseScreen;
    public float screenTransitionDuration = 0.3f;

    void Awake()
    {
        LevelManager.Instance.onNewOceanicZoneEntered += ShowZoneEnteredScreen;
        LevelManager.Instance.onOceanFloorReached += ShowWinScreen;
    }

    // Start is called before the first frame update
    void Start()
    {
        DiverModel.Instance.health.OnDamageTaken.AddListener(IndicateDamage);
        DiverModel.Instance.health.OnHealthDepleted.AddListener(ShowLoseScreen);
    }

    // Update is called once per frame
    void Update()
    {
        depthText.text = $"{LevelManager.Instance.curDepth:###,##0} m";
        coinsText.text = $"{DiverModel.Instance.coinsCollected:###,##0}";

        healthBarImage.fillAmount = DiverModel.Instance.health.normalizedHealth;
    }

    void OnDestroy()
    {
        LevelManager.Instance.onNewOceanicZoneEntered -= ShowZoneEnteredScreen;
        LevelManager.Instance.onOceanFloorReached -= ShowWinScreen;
        DiverModel.Instance.health.OnDamageTaken.RemoveListener(IndicateDamage);
        DiverModel.Instance.health.OnHealthDepleted.RemoveListener(ShowLoseScreen);
    }

    public void ShowZoneEnteredScreen()
    {
        if (LevelManager.Instance.curOceanicZone.name.Length <= 0)
        {
            return;
        }

        zoneEnteredText.text = LevelManager.Instance.curOceanicZone.name;

        var preSequence = zoneEnteredScreen.DOFade(0, 0);
        var showSequence = zoneEnteredScreen.DOFade(1f, zoneEnteredScreenFadeDuration);
        var hideSequence = zoneEnteredScreen.DOFade(0f, zoneEnteredScreenFadeDuration);

        preSequence.AppendCallback(() =>
        {
            zoneEnteredScreen.SetActive(true);
            showSequence.Play();
        });

        showSequence.AppendCallback(() => { this.WaitAndExecute(() => { hideSequence.Play(); }, zoneEnteredScreenDisplayDuration); });

        hideSequence.AppendCallback(() => { zoneEnteredScreen.SetActive(false); });

        preSequence.Play();
    }

    void IndicateDamage()
    {
        damageIndicatorImage.DOKill();

        var tween = damageIndicatorImage.DOFade(damageIndicatorMaxOpacity, damageIndicatorAnimationDuration);
        tween.OnComplete(() =>
        {
            damageIndicatorImage.DOKill();
            damageIndicatorImage.DOFade(0, damageIndicatorAnimationDuration).Play();
        });

        tween.Play();
    }

    public void PlayAgain()
    {
        LevelManager.Instance.RestartCurrentScene();
    }

    public void GoToMainMenu()
    {
        LevelManager.Instance.GoToMainMenu();
    }

    void ShowWinScreen()
    {
        ShowScreen(winScreen);
        Time.timeScale = 0f;
    }

    void ShowLoseScreen()
    {
        ShowScreen(loseScreen);
        Time.timeScale = 0f;
    }

    void ShowScreen(GameObject screen)
    {
        var preSeq = screen.DOFade(0, 0);
        var fadeInSeq = screen.DOFade(1f, screenTransitionDuration);

        preSeq.AppendCallback(() =>
        {
            screen.SetActive(true);
            fadeInSeq.Play();
        });
        preSeq.Play();
    }
}