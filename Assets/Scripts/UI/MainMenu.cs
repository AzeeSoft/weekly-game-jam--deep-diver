using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string playLevelName = "SampleScene";

    public GameObject title;
    public GameObject menu;

    public float titleFadeDuration = 1;
    public float titleFadeDelay = 2;

    public float menuFadeDuration = 1;

    private bool _menuShown = false;

    void Awake()
    {
        title.SetActive(false);
        menu.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        this.WaitAndExecute(() =>
        {
            ShowGraphicObject(title, titleFadeDuration);
        }, titleFadeDelay);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_menuShown && DiverModel.Instance.flashlight.activeSelf)
        {
            _menuShown = true;
            ShowMenu();
        }
    }

    void ShowMenu()
    {
        ShowGraphicObject(menu, menuFadeDuration);
    }

    void ShowGraphicObject(GameObject obj, float duration)
    {
        obj.DOFade(0, 0).AppendCallback(() =>
        {
            obj.SetActive(true);
            obj.DOFade(1, duration).Play();
        }).Play();
    }

    public void Play()
    {
        SceneManager.LoadScene(playLevelName);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }
}
