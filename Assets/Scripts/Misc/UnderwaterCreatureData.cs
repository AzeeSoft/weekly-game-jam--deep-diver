using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Underwater Creature", menuName = "Game Data/Underwater Creature")]
public class UnderwaterCreatureData : ScriptableObject
{
    [TextArea] public string description;
    public List<GameObject> prefabs;

    private Randomizer<GameObject> prefabsRandomizer;

    public void Awake()
    {
        prefabsRandomizer = new Randomizer<GameObject>(prefabs);
    }

    public GameObject GetRandomPrefab()
    {
        return prefabsRandomizer.GetRandomItem();
    }
}