using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SingletonMonoBehaviour<LevelManager>
{
    public float globalDiveSpeed => DiverModel.Instance.globalDiveSpeed;

    public Transform environmentRoot;

    new void Awake()
    {
        base.Awake();

        if (!environmentRoot)
        {
            Debug.LogError("Environment Root is not set");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnvironment();
    }

    void UpdateEnvironment()
    {
        var targetPos = environmentRoot.position + (Vector3.up * globalDiveSpeed);
        environmentRoot.position = Vector3.Lerp(environmentRoot.position, targetPos, Time.deltaTime);
    }

    public void AddToEnvironmentRoot(Transform objTransform)
    {
        objTransform.SetParent(environmentRoot);
    }
}
