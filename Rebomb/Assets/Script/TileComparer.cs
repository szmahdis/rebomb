using System.Collections.Generic;
using UnityEngine;

public class Vector3EqualityComparer : IEqualityComparer<Vector3>
{
    private const float Tolerance = 0.001f;

    public bool Equals(Vector3 v1, Vector3 v2)
    {
        return Mathf.Abs(v1.x - v2.x) < Tolerance && Mathf.Abs(v1.z - v2.z) < Tolerance;
    }

    public int GetHashCode(Vector3 obj)
    {
        // Hash code generation using rounded values to ensure consistent hashing
        // Note: tile index starts from (-1, y, -1) now.
        int x = Mathf.RoundToInt((obj.x + 2) / Tolerance);
        int z = Mathf.RoundToInt((obj.z + 2) / Tolerance);
        return x ^ z;
    }
}