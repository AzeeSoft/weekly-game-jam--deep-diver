using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MainMenuPage : MonoBehaviour
{
    public float fadeDuration = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        var preSeq = gameObject.DOFade(0, 0);
        var showSeq = gameObject.DOFade(1, fadeDuration);

        preSeq.AppendCallback(() =>
        {
            gameObject.SetActive(true);
            showSeq.Play();
        });

        preSeq.Play();
    }

    public void Hide()
    {
        gameObject.SetActive(true);

        var preSeq = gameObject.DOFade(1, 0);
        var hideSeq = gameObject.DOFade(0, fadeDuration);

        preSeq.AppendCallback(() =>
        {
            hideSeq.Play();
        });

        hideSeq.AppendCallback(() =>
        {
            gameObject.SetActive(false);
        });

        preSeq.Play();
    }
}
