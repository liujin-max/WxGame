using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ToolUtility
{
    public static bool Approximately(Vector3 v1, Vector3 v2) {
        return Vector3.SqrMagnitude(v1 - v2) < float.Epsilon;
    }
}
