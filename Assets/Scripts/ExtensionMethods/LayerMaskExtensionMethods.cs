using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class LayerMaskExtensionMethods
{
    public static int ToLayerInt(this LayerMask layerMask)
    {
        return Mathf.RoundToInt(Mathf.Log(layerMask.value, 2));
    }

    public static bool Contains(this LayerMask mask, int layerIndex)
    {
        return (mask.value & (1 << layerIndex)) != 0;
    }
}
