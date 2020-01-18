using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class OceanicZone
{
    public string name;
    public float startDepth;
    public float endDepth;

    public List<UnderwaterCreatureData> creatures;
}