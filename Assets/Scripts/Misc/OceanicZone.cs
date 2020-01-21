using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class OceanicZone
{
    public string name;
    public int depth;
    public Color color;
    public float globalLightIntensity = 1;
    public float globalDiveSpeedModifier = 0;

    [ReadOnly] public bool hasEnded = false;

    public GameObject endOfZoneObjectPrefab;

    [Header("Creatures")] public int creatureMaxSpawnCount = 1;
    public float creatureSpawnInterval = 3;
    public float creatureSpawnIntervalModifier = 1;
    public List<GameObject> creaturePrefabs;

    [Header("Coins")] public int coinsMaxSpawnCount = 1;
    public float coinSpawnInterval = 3;
    public float coinSpawnIntervalModifier = 1;
    public List<GameObject> coinGroupPrefabs;

    private Randomizer<GameObject> creaturePrefabsRandomizer = null;
    private Randomizer<GameObject> coinGroupPrefabsRandomizer = null;

    public GameObject GetRandomCreaturePrefab()
    {
        if (creaturePrefabsRandomizer == null)
        {
            creaturePrefabsRandomizer = new Randomizer<GameObject>(creaturePrefabs);
        }

        return creaturePrefabsRandomizer.GetRandomItem();
    }

    public GameObject GetRandomCoinGroupPrefab()
    {
        if (coinGroupPrefabsRandomizer == null)
        {
            coinGroupPrefabsRandomizer = new Randomizer<GameObject>(coinGroupPrefabs);
        }

        return coinGroupPrefabsRandomizer.GetRandomItem();
    }
}