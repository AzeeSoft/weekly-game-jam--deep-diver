﻿using System;
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
    public float globalDiveSpeedModifier = 0;

    public int creatureMaxSpawnCount = 1;
    public float creatureSpawnInterval = 3;
    public float creatureSpawnIntervalModifier = 1;
    public List<UnderwaterCreatureData> creatures;

    private Randomizer<UnderwaterCreatureData> creatureRandomizer = null;

    public UnderwaterCreatureData GetRandomCreatureData()
    {
        if (creatureRandomizer == null)
        {
            creatureRandomizer = new Randomizer<UnderwaterCreatureData>(creatures);
        }

        return creatureRandomizer.GetRandomItem();
    }
}