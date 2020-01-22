using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI depthText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        coinsText.text = $"{DiverModel.Instance.coinsCollected:###,##0}";
        depthText.text = $"{LevelManager.Instance.curDepth:###,##0} m";
    }
}
