using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerTool
{
    public static string LayerToName(int index)
    {
        return LayerMask.LayerToName(index);
    }
}