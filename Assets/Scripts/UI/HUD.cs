using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
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

    void Awake()
    {
        LevelManager.Instance.onNewOceanicZoneEntered += ShowZoneEnteredScreen;
    }

    // Start is called before the first frame update
    void Start()
    {
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
    }

    public void ShowZoneEnteredScreen()
    {
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
}