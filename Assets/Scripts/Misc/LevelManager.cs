using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;
using UnityEngine.SceneManagement;

public class LevelManager : SingletonMonoBehaviour<LevelManager>
{
    public float globalDiveSpeed => DiverModel.Instance.globalDiveSpeed;
    public bool hasReachedOceanFloor { get; private set; } = false;

    public int curDepth => (int) ((environmentRoot.position.y - originalEnvironmentRootPos.y) * depthScalePerUnit);

    public OceanicZone curOceanicZone => oceanicZones[curOceanicZoneIndex];
    public OceanicZone prevOceanicZone => hasPrevOceanicZone ? oceanicZones[curOceanicZoneIndex - 1] : null;
    public OceanicZone nextOceanicZone => hasNextOceanicZone ? oceanicZones[curOceanicZoneIndex + 1] : null;

    public bool hasPrevOceanicZone => curOceanicZoneIndex > 0;
    public bool hasNextOceanicZone => curOceanicZoneIndex < oceanicZones.Count - 1;

    public float curZoneProgress =>
        Mathf.Clamp01(HelperUtilities.Remap(curDepth, curOceanicZoneStartDepth, curOceanicZoneEndDepth, 0, 1));

    public Color curOceanicColor
    {
        get
        {
            var curZoneColor = curOceanicZone.color;

            if (hasNextOceanicZone)
            {
                var nextZoneColor = nextOceanicZone.color;
                return Color.Lerp(curZoneColor, nextZoneColor, curZoneProgress);
            }

            return curZoneColor;
        }
    }

    public float curGlobalLightIntensity
    {
        get
        {
            var curZoneLightIntensity = curOceanicZone.globalLightIntensity;

            if (hasNextOceanicZone)
            {
                var nextZoneLightIntensity = nextOceanicZone.globalLightIntensity;
                return Mathf.Lerp(curZoneLightIntensity, nextZoneLightIntensity, curZoneProgress);
            }

            return curZoneLightIntensity;
        }
    }

    public float depthScalePerUnit = 1f;
    public string mainMenuSceneName = "MainMenu";
    public Light2D globalLight2D;
    public Transform environmentRoot;

    public List<OceanicZone> oceanicZones;

    [SerializeField] [ReadOnly] private int curOceanicZoneIndex = 0;

    private int curOceanicZoneStartDepth = 0;
    private int curOceanicZoneEndDepth => curOceanicZoneStartDepth + curOceanicZone.depth;

    private Vector3 originalEnvironmentRootPos = Vector3.zero;

    private readonly Dictionary<string, UnderwaterCreatureData>
        _allUnderwaterCreatureData = new Dictionary<string, UnderwaterCreatureData>();

    public event Action onEndOfCurOceanicZoneReached;
    public event Action onNewOceanicZoneEntered;
    public event Action onOceanFloorReached;

    new void Awake()
    {
        base.Awake();

        Time.timeScale = 1f;

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
        UpdateGlobalLight();
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
            if (!curOceanicZone.hasEnded)
            {
                curOceanicZone.hasEnded = true;
                onEndOfCurOceanicZoneReached?.Invoke();
            }

            if (hasNextOceanicZone)
            {
                curOceanicZoneStartDepth = curOceanicZoneEndDepth;
                curOceanicZoneIndex++;

                onNewOceanicZoneEntered?.Invoke();
            }
        }
    }

    void UpdateGlobalLight()
    {
        globalLight2D.intensity = curGlobalLightIntensity;
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

    public void OceanFloorReached()
    {
        hasReachedOceanFloor = true;
        onOceanFloorReached?.Invoke();
    }

    public void RestartCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}