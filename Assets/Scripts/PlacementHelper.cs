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
}
