using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class OceanicZone
{
    [Serializable]
    public class SpawnableEntity
    {
        public int maxSpawnCount = 1;
        public float spawnInterval = 3;
        public float spawnIntervalModifier = 1;
        public List<GameObject> prefabs;

        private Randomizer<GameObject> prefabsRandomizer = null;

        public GameObject GetRandomPrefab()
        {
            if (prefabsRandomizer == null)
            {
                prefabsRandomizer = new Randomizer<GameObject>(prefabs);
            }

            return prefabsRandomizer.GetRandomItem();
        }
    }

    public string name;
    public int depth;
    public Color color;
    public float globalLightIntensity = 1;
    public float globalDiveSpeedModifier = 0;

    [ReadOnly] public bool hasEnded = false;

    public GameObject endOfZoneObjectPrefab;

    public SpawnableEntity creatures;
    public SpawnableEntity coins;
    public SpawnableEntity powerUps;
}