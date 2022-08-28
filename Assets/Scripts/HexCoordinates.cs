using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HexCoordinates
{
    public static HexCoordinates zero => new HexCoordinates(0, 0);
    public static HexCoordinates west => new HexCoordinates(1, 0);
    public static HexCoordinates southWest => new HexCoordinates(0, 1);
    public static HexCoordinates southEast => new HexCoordinates(0, 1);
    public static HexCoordinates east => new HexCoordinates(-1, 0);
    public static HexCoordinates northEast => new HexCoordinates(-1, -1);
    public static HexCoordinates northWest => new HexCoordinates(1, -1);
    public static List<HexCoordinates> neighborOffsets
        => new List<HexCoordinates>
            {
                east,
                southEast,
                southWest,
                west,
                northWest,
                northEast,
            };

    public int x => m_x;
    public int z => m_z;
    public int y => -x - z;

    [SerializeField] private int m_x, m_z;

    public HexCoordinates(int x, int z)
    {
        m_x = x;
        m_z = z;
    }

    public int DistanceTo(HexCoordinates other)
        => (Mathf.Abs(x - other.x) + Mathf.Abs(y - other.y) + Mathf.Abs(z - other.z)) >> 1;

    public static int Distance(HexCoordinates coordinates0, HexCoordinates coordinates1)
        => coordinates0.DistanceTo(coordinates1);

    public static HexCoordinates FromPosition(Vector3 position)
    {
        float x = position.x / (HexMetrics.innerRadius * 2f);
        float y = -x;

        float offset = position.z / (HexMetrics.outerRadius * 3f);
        x -= offset;
        y -= offset;

        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);

        if (iX + iY + iZ != 0)
        {
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);

            if (dX > dY && dX > dZ)
                iX = -iY - iZ;
            else if (dZ > dY)
                iZ = -iX - iY;
        }

        return new HexCoordinates(iX, iZ);
    }

    public Vector3 ToPosition() => HexMetrics.ToWorldPosition(x, z);

    public static HexCoordinates operator +(HexCoordinates left, HexCoordinates right)
        => new HexCoordinates(left.x + right.x, left.z + right.z);

    public static HexCoordinates operator -(HexCoordinates left, HexCoordinates right)
        => new HexCoordinates(left.x - right.x, left.z - right.z);

    public static HexCoordinates operator -(HexCoordinates coordinates)
        => new HexCoordinates(-coordinates.x, -coordinates.z);

    public static bool operator ==(HexCoordinates left, HexCoordinates right)
        => left.IsSameCoordinates(right);

    public static bool operator !=(HexCoordinates left, HexCoordinates right)
        => !left.IsSameCoordinates(right);

    public override bool Equals(object obj)
    {
        return base.Equals(obj) && obj is HexCoordinates other && IsSameCoordinates(other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"({x}; {y}; {z})";
    }

    private bool IsSameCoordinates(HexCoordinates other)
        => (x == other.x) && (z == other.z);
}