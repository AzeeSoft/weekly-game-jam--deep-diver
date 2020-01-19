using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : SingletonMonoBehaviour<LevelManager>
{
    public float globalDiveSpeed => DiverModel.Instance.globalDiveSpeed;
    public int curDepth => (int) ((environmentRoot.position.y - originalEnvironmentRootPos.y) * depthScalePerUnit);
    public OceanicZone curOceanicZone => oceanicZones[curOceanicZoneIndex];
    public OceanicZone nextOceanicZone => hasNextOceanicZone ? oceanicZones[curOceanicZoneIndex + 1] : null;
    public bool hasNextOceanicZone => curOceanicZoneIndex < oceanicZones.Count - 1;

    public float curZoneProgress =>
        Mathf.Clamp01(HelperUtilities.Remap(curDepth, curOceanicZoneStartDepth, curOceanicZoneEndDepth, 0, 1));

    public Color curOceanicColor
    {
        get
        {
            var curZoneColor = curOceanicZone.color;
            var nextZoneColor = Color.black;

            if (hasNextOceanicZone)
            {
                nextZoneColor = nextOceanicZone.color;
            }

            return Color.Lerp(curZoneColor, nextZoneColor, curZoneProgress);
        }
    }

    public float depthScalePerUnit = 1f;
    public Transform environmentRoot;

    public List<OceanicZone> oceanicZones;

    [SerializeField] [ReadOnly] private int curOceanicZoneIndex = 0;

    private int curOceanicZoneStartDepth = 0;
    private int curOceanicZoneEndDepth => curOceanicZoneStartDepth + curOceanicZone.depth;

    private Vector3 originalEnvironmentRootPos = Vector3.zero;

    private readonly Dictionary<string, UnderwaterCreatureData>
        _allUnderwaterCreatureData = new Dictionary<string, UnderwaterCreatureData>();

    public event Action onNewOceanicZoneEntered;

    new void Awake()
    {
        base.Awake();

        if (!environmentRoot)
        {
            Debug.LogError("Environment Root is not set");
        }

        originalEnvironmentRootPos = environmentRoot.position;

        RefreshAllUnderwaterCreatureData();
    }

    // Start is called before the first frame update
    void Start()
    {
        onNewOceanicZoneEntered?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnvironment();
        UpdateCurrentZone();
    }

    void UpdateEnvironment()
    {
        var targetPos = environmentRoot.position + (Vector3.up * globalDiveSpeed);
        environmentRoot.position = Vector3.Lerp(environmentRoot.position, targetPos, Time.deltaTime);
    }

    void UpdateCurrentZone()
    {
        if (curDepth > curOceanicZoneEndDepth)
        {
            if (hasNextOceanicZone)
            {
                curOceanicZoneStartDepth = curOceanicZoneEndDepth;
                curOceanicZoneIndex++;

                onNewOceanicZoneEntered?.Invoke();
            }
        }
    }

    public void AddToEnvironmentRoot(Transform objTransform)
    {
        objTransform.SetParent(environmentRoot);
    }

    void RefreshAllUnderwaterCreatureData()
    {
        _allUnderwaterCreatureData.Clear();

        var allUnderwaterCreatureList = HelperUtilities.GetAllResources<UnderwaterCreatureData>();
        foreach (var underwaterCreatureData in allUnderwaterCreatureList)
        {
            if (!_allUnderwaterCreatureData.ContainsKey(underwaterCreatureData.ID))
            {
                _allUnderwaterCreatureData.Add(underwaterCreatureData.ID, underwaterCreatureData);
            }
            else
            {
                Debug.LogError($"Duplicate Underwater Creature ID found: {underwaterCreatureData.ID}");
                Debug.LogError(
                    $"Conflicting data: {_allUnderwaterCreatureData[underwaterCreatureData.ID].name}, {underwaterCreatureData.name}");
            }
        }
    }
}