using UnityEngine;
using System.Collections.Generic;


public static class PlacementHelper
{
    public static List<Direction> FindNeighbour(Vector3Int position, ICollection<Vector3Int> collection) { 
        List<Direction> neighbourDirections = new List<Direction>();

        if (collection.Contains(position + Vector3Int.up)) neighbourDirections.Add(Direction.Up);
        if (collection.Contains(position + Vector3Int.down)) neighbourDirections.Add(Direction.Down);
        if (collection.Contains(position + Vector3Int.left)) neighbourDirections.Add(Direction.Left);
        if (collection.Contains(position + Vector3Int.right)) neighbourDirections.Add(Direction.Right);

        return neighbourDirections;
    }

    public static Vector3Int GetOffset(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector3Int.up;
            case Direction.Down:
                return Vector3Int.down;
            case Direction.Left:
                return Vector3Int.left;
            case Direction.Right:
                return Vector3Int.right;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
        throw new System.ArgumentOutOfRangeException(nameof(direction), direction, "Invalid direction");
    }

    public static Direction OppositeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }
}
