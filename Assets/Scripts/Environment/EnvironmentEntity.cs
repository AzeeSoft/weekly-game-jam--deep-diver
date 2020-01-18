using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentEntity : MonoBehaviour
{
    protected void Awake()
    {
        LevelManager.Instance.AddToEnvironmentRoot(transform);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
