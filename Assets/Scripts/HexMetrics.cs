using UnityEngine;

public static class HexMetrics
{
    public const float outerToInner = 0.86602540378f;
    public const float innerToOuter = 1f / outerToInner;

    public const float outerRadius = 0.5f;
    public const float innerRadius = outerRadius * outerToInner;

    public static Vector3 ToWorldPosition(int x, int z) => new Vector3
    {
        x = (x + z * 0.5f - z / 2) * (innerRadius * 2f),
        y = 0f,
        z = -z * outerRadius * 1.5f,
    };
}