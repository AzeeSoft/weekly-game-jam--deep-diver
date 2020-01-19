using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticBG : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.color = LevelManager.Instance.curOceanicColor;
    }
}
