using UnityEngine;

public static class HexagonUtils
{
    public static float widthRatio => 0.86602540378f; // sin(PI/3)

    public static Vector3 HexGridToWorldPosition(Vector2Int hexPosition)
        => HexGridToWorldPosition(hexPosition.x, hexPosition.y);
    public static Vector3 HexGridToWorldPosition(int x, int y)
    {
        float worldX = (x + (y & 1) * 0.5f) * widthRatio;
        float worldZ = -y * 0.75f;

        return new Vector3(worldX, 0.0f, worldZ);
    }

    public static int Distance(Vector2Int start, Vector2Int end)
        => Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y) >> 1;
}
