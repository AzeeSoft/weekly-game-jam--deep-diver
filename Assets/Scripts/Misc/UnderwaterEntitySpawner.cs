using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class UnderwaterEntitySpawner : MonoBehaviour
{
    public struct SpawnArea
    {
        public Vector3 topLeft;
        public Vector3 bottomRight;
    }

    public float creaturesVerticalMovementModifier = 0.3f;
    public Sides spawnAreaPadding;

    private LevelManager levelManager;

    private float nextCreatureSpawnTime = 0;
    private SpawnArea spawnArea = new SpawnArea();

    void Awake()
    {
        levelManager = LevelManager.Instance;
        levelManager.onNewOceanicZoneEntered += OnNewZoneEntered;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateSpawnArea();
    }

    void OnDrawGizmos()
    {
        var bottomLeft = new Vector3(spawnArea.topLeft.x, spawnArea.bottomRight.y);
        var topRight = new Vector3(spawnArea.bottomRight.x, spawnArea.topLeft.y);

        Gizmos.color = Color.green;

        Gizmos.DrawLine(spawnArea.topLeft, topRight);
        Gizmos.DrawLine(topRight, spawnArea.bottomRight);
        Gizmos.DrawLine(spawnArea.bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, spawnArea.topLeft);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpawnArea();

        CheckAndSpawnCreatures();
    }

    void OnDestroy()
    {
        levelManager.onNewOceanicZoneEntered -= OnNewZoneEntered;
    }

    void OnNewZoneEntered()
    {
        UpdateNextCreatureSpawnTime();
    }

    void UpdateNextCreatureSpawnTime()
    {
        nextCreatureSpawnTime += levelManager.curOceanicZone.creatureSpawnInterval + Random.Range(
                                     -levelManager.curOceanicZone.creatureSpawnIntervalModifier,
                                     levelManager.curOceanicZone.creatureSpawnIntervalModifier);
    }

    void UpdateSpawnArea()
    {
        var cam = CameraRigManager.Instance.cinemachineBrain.OutputCamera;

        spawnArea.topLeft = cam.ViewportToWorldPoint(new Vector3(0, 0));
        spawnArea.bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0));

        spawnArea.topLeft += new Vector3(-spawnAreaPadding.left, spawnAreaPadding.up, 0);
        spawnArea.bottomRight += new Vector3(spawnAreaPadding.right, -spawnAreaPadding.down, 0);
    }

    void CheckAndSpawnCreatures()
    {
        if (Time.time > nextCreatureSpawnTime)
        {
            for (int i = 0; i < Random.Range(1, Mathf.Max(1, levelManager.curOceanicZone.creatureMaxSpawnCount)); i++)
            {
                var creaturePrefab = levelManager.curOceanicZone.GetRandomCreatureData().GetRandomPrefab();
                var underwaterEntity = SpawnObject(creaturePrefab).GetComponent<UnderwaterEntity>();

                Vector3 moveDir = DiverModel.Instance.transform.position - underwaterEntity.transform.position;
                moveDir.y += Random.Range(-creaturesVerticalMovementModifier, creaturesVerticalMovementModifier);
                moveDir.z = 0;

                underwaterEntity.SetMoveDirection(moveDir.normalized);
            }

            UpdateNextCreatureSpawnTime();
        }
    }

    GameObject SpawnObject(GameObject prefab)
    {
        Vector3 spawnPos = new Vector3(Random.Range(spawnArea.topLeft.x, spawnArea.bottomRight.x),
            Random.Range(spawnArea.topLeft.y, spawnArea.bottomRight.y));
        return Instantiate(prefab, spawnPos, prefab.transform.rotation);
    }
}