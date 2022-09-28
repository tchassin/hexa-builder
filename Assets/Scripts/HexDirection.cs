using System.Collections.Generic;

public enum HexDirection : int
{
    E,
    SE,
    SW,
    W,
    NW,
    NE,
}

public static class DirectionExtensions
{
    public static HexDirection GetOpposite(this HexDirection direction)
        => (int)direction < 3 ? (direction + 3) : (direction - 3);

    public static HexDirection FlipVertically(this HexDirection direction) => direction switch
    {
        HexDirection.SE => HexDirection.NE,
        HexDirection.SW => HexDirection.NW,
        HexDirection.NW => HexDirection.SW,
        HexDirection.NE => HexDirection.SE,
        _ => direction
    };

    public static List<HexDirection> FlipVertically(this List<HexDirection> directions)
    {
        var flippedDirections = new List<HexDirection>(directions.Count);
        for (int i = 0; i < directions.Count; i++)
            flippedDirections.Add(directions[i].FlipVertically());
        flippedDirections.Sort();

        return flippedDirections;
    }

    public static int DistanceTo(this HexDirection direction, HexDirection otherDirection)
        => otherDirection - direction;

    public static bool IsPermutationOf(this List<HexDirection> directions, List<HexDirection> otherDirections, out int distance, out bool isFlipped)
    {
        isFlipped = false;
        distance = int.MinValue;

        if (directions == null || otherDirections == null)
            return false;

        if (directions.Count != otherDirections.Count)
            return false;

        if (directions.Count == 0)
        {
            distance = 0;

            return true;
        }

        // Check without flipping vertically
        distance = GetPermutationDistance(directions, otherDirections);
        if (distance != int.MinValue)
            return true;

        // Check the vertical flip is a permutation
        distance = GetPermutationDistance(directions, otherDirections.FlipVertically());
        if (distance == int.MinValue)
            return false;

        isFlipped = true;

        return true;
    }

    static int GetPermutationDistance(List<HexDirection> directions, List<HexDirection> otherDirections)
    {
        static int GetPermutationDistanceWithOffset(List<HexDirection> directions, List<HexDirection> refDir, int start)
        {
            int distance = directions[start].DistanceTo(refDir[0]);
            for (int i = 1; i < directions.Count; i++)
            {
                int index = (start + i) % directions.Count;
                HexDirection rotation = directions[index].Rotate(distance);
                if (rotation != refDir[i])
                    return int.MinValue;
            }

            return distance < 0 ? distance + 6 : distance;
        }

        for (int i = 0; i < directions.Count; i++)
        {
            int distance = GetPermutationDistanceWithOffset(directions, otherDirections, i);
            if (distance != int.MinValue)
                return distance;
        }

        return int.MinValue;
    }

    public static HexDirection Rotate(this HexDirection direction, int rotation)
    {
        int rotated = ((int)direction + rotation) % 6;
        if (rotated < 0)
            rotated += 6;

        return (HexDirection)rotated;
    }

    public static float ToAngle(this HexDirection direction)
    {
        int distance = (int)direction;

        return DistanceToAngle(distance);
    }

    public static float AngleTo(this HexDirection direction, HexDirection otherDirection)
    {
        int distance = direction.DistanceTo(otherDirection);

        return DistanceToAngle(distance);
    }

    public static float DistanceToAngle(int distance)
    {
        // Clamp angle to ]-180; 180]
        int angle = distance % 6;
        int clampedAngle = angle > 3 ? angle - 6 : angle <= -3 ? angle + 6 : angle;

        return clampedAngle * 60.0f;
    }

    public static HexCoordinates ToOffest(this HexDirection direction) => direction switch
    {
        HexDirection.E => HexCoordinates.east,
        HexDirection.SE => HexCoordinates.southEast,
        HexDirection.SW => HexCoordinates.southWest,
        HexDirection.W => HexCoordinates.west,
        HexDirection.NW => HexCoordinates.northWest,
        HexDirection.NE => HexCoordinates.northEast,
        _ => HexCoordinates.zero
    };
}
