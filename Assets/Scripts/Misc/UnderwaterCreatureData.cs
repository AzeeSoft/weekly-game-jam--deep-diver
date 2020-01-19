using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicTools.ButtonInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Underwater Creature", menuName = "Game Data/Underwater Creature")]
public class UnderwaterCreatureData : ScriptableObject
{
    [ReadOnly] public string ID = Guid.NewGuid().ToString();
    [TextArea] public string description;
    public List<GameObject> prefabs;

    [SerializeField] [Button("Generate New ID", "generateNewID")]
    private bool _btnGenerateNewId;

    private Randomizer<GameObject> prefabsRandomizer = null;

    public GameObject GetRandomPrefab()
    {
        if (prefabsRandomizer == null)
        {
            prefabsRandomizer = new Randomizer<GameObject>(prefabs);
        }

        return prefabsRandomizer.GetRandomItem();
    }

    void generateNewID()
    {
        ID = Guid.NewGuid().ToString();
    }
}