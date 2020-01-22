using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnderwaterEntitySpawner : MonoBehaviour
{
    public class SpawnArea
    {
        public Vector3 topLeft;
        public Vector3 bottomRight;

        public Vector3 center => (topLeft + bottomRight) / 2f;
    }

    public class SpawnableEntityTimer
    {
        public float nextSpawnTime { get; private set; } = 0;

        private readonly Func<OceanicZone.SpawnableEntity> _getSpawnableEntity;

        public SpawnableEntityTimer(Func<OceanicZone.SpawnableEntity> callback)
        {
            _getSpawnableEntity = callback;
        }

        public void UpdateNextSpawnTime()
        {
            var spawnableEntity = _getSpawnableEntity?.Invoke();

            if (spawnableEntity != null)
            {
                nextSpawnTime = Time.time + spawnableEntity.spawnInterval + Random.Range(
                                    -spawnableEntity.spawnIntervalModifier,
                                    spawnableEntity.spawnIntervalModifier);
            }
        }
    }

    public float creaturesVerticalMovementModifier = 0.3f;
    public Sides spawnAreaPadding;

    private LevelManager levelManager;

    private SpawnableEntityTimer creatureSpawnTimer;
    private SpawnableEntityTimer coinSpawnTimer;
    private SpawnableEntityTimer powerUpSpawnTimer;

    private SpawnArea spawnArea = new SpawnArea();

    void Awake()
    {
        levelManager = LevelManager.Instance;
        levelManager.onNewOceanicZoneEntered += OnNewZoneEntered;
        levelManager.onEndOfCurOceanicZoneReached += OnEndOfCurOceanicZoneReached;

        creatureSpawnTimer = new SpawnableEntityTimer(() => levelManager.curOceanicZone.creatures);
        coinSpawnTimer = new SpawnableEntityTimer(() => levelManager.curOceanicZone.coins);
        powerUpSpawnTimer = new SpawnableEntityTimer(() => levelManager.curOceanicZone.powerUps);
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateSpawnArea();
    }

    void OnDrawGizmos()
    {
        if (spawnArea != null)
        {
            var bottomLeft = new Vector3(spawnArea.topLeft.x, spawnArea.bottomRight.y);
            var topRight = new Vector3(spawnArea.bottomRight.x, spawnArea.topLeft.y);

            Gizmos.color = Color.green;

            Gizmos.DrawLine(spawnArea.topLeft, topRight);
            Gizmos.DrawLine(topRight, spawnArea.bottomRight);
            Gizmos.DrawLine(spawnArea.bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, spawnArea.topLeft);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpawnArea();

        CheckAndSpawnCreatures();
        CheckAndSpawnCoins();
        CheckAndSpawnPowerUps();
    }

    void OnDestroy()
    {
        levelManager.onNewOceanicZoneEntered -= OnNewZoneEntered;
        levelManager.onEndOfCurOceanicZoneReached -= OnEndOfCurOceanicZoneReached;
    }

    void OnNewZoneEntered()
    {
        creatureSpawnTimer.UpdateNextSpawnTime();
        coinSpawnTimer.UpdateNextSpawnTime();
        powerUpSpawnTimer.UpdateNextSpawnTime();
    }

    void OnEndOfCurOceanicZoneReached()
    {
        SpawnEndOfZoneObject(levelManager.curOceanicZone);
    }

    void UpdateSpawnArea()
    {
        var cam = CameraRigManager.Instance.cinemachineBrain.OutputCamera;

        spawnArea.topLeft = cam.ViewportToWorldPoint(new Vector3(0, 0));
        spawnArea.bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0));

        spawnArea.topLeft += new Vector3(-spawnAreaPadding.left, spawnAreaPadding.up, 0);
        spawnArea.bottomRight += new Vector3(spawnAreaPadding.right, -spawnAreaPadding.down, 0);
    }

    void SpawnEndOfZoneObject(OceanicZone oceanicZone)
    {
        if (oceanicZone.endOfZoneObjectPrefab)
        {
            Vector3 spawnPos = spawnArea.center;
            spawnPos.z = 0;
            Instantiate(oceanicZone.endOfZoneObjectPrefab, spawnPos,
                oceanicZone.endOfZoneObjectPrefab.transform.rotation);
        }
    }

    void CheckAndSpawnCreatures()
    {
        if (Time.time > creatureSpawnTimer.nextSpawnTime)
        {
            for (int i = 0;
                i < Random.Range(Mathf.Min(1, levelManager.curOceanicZone.creatures.maxSpawnCount),
                    levelManager.curOceanicZone.creatures.maxSpawnCount);
                i++)
            {
                var creaturePrefab = levelManager.curOceanicZone.creatures.GetRandomPrefab();
                var underwaterEntity = SpawnObject(creaturePrefab).GetComponent<UnderwaterCreature>();

                Vector3 moveDir = DiverModel.Instance.transform.position - underwaterEntity.transform.position;
                moveDir.y += Random.Range(-creaturesVerticalMovementModifier, creaturesVerticalMovementModifier);
                moveDir.z = 0;

                underwaterEntity.SetMoveDirection(moveDir.normalized);
            }

            creatureSpawnTimer.UpdateNextSpawnTime();
        }
    }

    void CheckAndSpawnCoins()
    {
        if (Time.time > coinSpawnTimer.nextSpawnTime)
        {
            for (int i = 0;
                i < Random.Range(Mathf.Min(1, levelManager.curOceanicZone.coins.maxSpawnCount),
                    levelManager.curOceanicZone.coins.maxSpawnCount);
                i++)
            {
                var coinGroupPrefab = levelManager.curOceanicZone.coins.GetRandomPrefab();
                var coin = SpawnObject(coinGroupPrefab).GetComponent<Coin>();
            }

            coinSpawnTimer.UpdateNextSpawnTime();
        }
    }

    void CheckAndSpawnPowerUps()
    {
        if (Time.time > powerUpSpawnTimer.nextSpawnTime)
        {
            for (int i = 0;
                i < Random.Range(Mathf.Min(1, levelManager.curOceanicZone.powerUps.maxSpawnCount),
                    levelManager.curOceanicZone.powerUps.maxSpawnCount);
                i++)
            {
                var powerUpPrefab = levelManager.curOceanicZone.powerUps.GetRandomPrefab();
                var powerUp = SpawnObject(powerUpPrefab).GetComponent<PowerUp>();
            }

            powerUpSpawnTimer.UpdateNextSpawnTime();
        }
    }

    GameObject SpawnObject(GameObject prefab)
    {
        Vector3 spawnPos = new Vector3(Random.Range(spawnArea.topLeft.x, spawnArea.bottomRight.x),
            Random.Range(spawnArea.topLeft.y, spawnArea.bottomRight.y));
        return Instantiate(prefab, spawnPos, prefab.transform.rotation);
    }
}