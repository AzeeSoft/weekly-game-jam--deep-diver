using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DiverModel : SingletonMonoBehaviour<DiverModel>
{
    public Collider2D collider { get; private set; }

    public float globalDiveSpeed => baseGlobalDiveSpeed + LevelManager.Instance.curOceanicZone.globalDiveSpeedModifier;

    [SerializeField]
    private float baseGlobalDiveSpeed = 1f;

    new void Awake()
    {
        base.Awake();
        collider = GetComponent<Collider2D>();

        LevelManager.Instance.onNewOceanicZoneEntered += OnNewZoneEntered;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        LevelManager.Instance.onNewOceanicZoneEntered -= OnNewZoneEntered;
    }

    void OnNewZoneEntered()
    {

    }
}
