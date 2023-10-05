using Godot;
using Godot.Collections;
using System.Collections.Generic;

public class MapManager : Singleton<MapManager>
{
    public const int GRID_WIDTH = 10;
    public const int GRID_HEIGHT = 10;
    public const int TILEMAP_SIZE = 32;

    private TileMap _map;

    public readonly AStar2D pathfinder = new();

    public readonly HashSet<Vector2> moveableCells = new();
    public readonly HashSet<Vector2> disabledCells = new();

    public TileMap Map
    {
        get
        {
            _map ??= GetTree().Root.FindNode("Root", true, false).GetNode<TileMap>("Map");
            return _map;
        }
    }

    public override void _Ready()
    {
        RecalculatePathfinder();
    }

    public void RecalculatePathfinder()
    {
        pathfinder.Clear();
        moveableCells.Clear();

        // Add all cells to pathfinder
        foreach (Vector2 cell in Map.GetUsedCellsById(0))
        {
            moveableCells.Add(cell);
            pathfinder.AddPoint(pathfinder.GetAvailablePointId(), cell);
        }


        foreach (Vector2 cell in moveableCells)
        {
            Vector2[] directions = new[] { Vector2.Up, Vector2.Right, Vector2.Down, Vector2.Left };
            foreach (Vector2 direction in directions)
            {
                if (moveableCells.Contains(cell + direction))
                {
                    pathfinder.ConnectPoints(pathfinder.GetClosestPoint(cell), pathfinder.GetClosestPoint(cell + direction), false);
                }
            }
        }
    }

    public Vector2[] GetPointPath(Vector2 fromMapPos, Vector2 toMapPos)
    {
        return pathfinder.GetPointPath(
            pathfinder.GetClosestPoint(fromMapPos),
            pathfinder.GetClosestPoint(toMapPos)
        );
    }

    public bool CanMoveToCell(Vector2 mapPos)
    {
        return moveableCells.Contains(mapPos) && !disabledCells.Contains(mapPos);
    }

    public void SetCellDisabled(Vector2 mapPos, bool disabled)
    {
        // If we are already in the right state, do nothing
        bool alreadyDisabled = disabledCells.Contains(mapPos);
        if ((disabled && alreadyDisabled) | !disabled && !alreadyDisabled) return;

        pathfinder.SetPointDisabled(pathfinder.GetClosestPoint(mapPos, true), disabled);
        if (disabled)
        {
            disabledCells.Add(mapPos);
        }
        else
        {
            disabledCells.Remove(mapPos);
        }
    }

    public Vector2 MapToWorld(Vector2 mapPos)
    {
        Vector2 halfTile = new(TILEMAP_SIZE / 2, TILEMAP_SIZE / 2);
        return Map.MapToWorld(mapPos) + halfTile;
    }
    public Vector2 WorldToMap(Vector2 worldPos)
    {
        return Map.WorldToMap(worldPos);
    }

    // helpers
    public static int GetPathDistance(Vector2[] path)
    {
        int distance = 0;
        for (int i = 0; i < path.Length; i++)
        {
            if (i + 1 >= path.Length) break;
            distance += (int)path[i].DistanceTo(path[i + 1]);
        }

        return distance;
    }

    public static bool IsPointBetweenPoints(Vector2 targetPoint, Vector2 pointA, Vector2 pointB)
    {
        Vector2 toA = targetPoint.DirectionTo(pointA);
        Vector2 toB = targetPoint.DirectionTo(pointB);

        float dotProduct = toA.Dot(toB);

        // If the dot product is 1, the 2 vector angles above are exact opposites. So says google.
        return dotProduct == -1;
    }

    public static bool IsPointOnPath(Vector2 point, Vector2[] path)
    {
        // Check between every connection on the path.
        for (int i = 0; i < path.Length; i++)
        {
            if (i + 1 >= path.Length) break;
            if (IsPointBetweenPoints(point, path[i], path[i + 1]))
            {
                return true;
            }
        }
        return false;
    }

    public static Array<Vector2> PathToPoints(Vector2[] path)
    {
        // Start by adding the start position
        Array<Vector2> points = new() { path[0] };

        for (int i = 0; i < path.Length; i++)
        {
            if (i + 1 >= path.Length) break;

            Vector2 current = path[i];
            Vector2 to = path[i + 1];

            while (current != to)
            {
                current = current.MoveToward(to, 1);
                points.Add(current);
            }
        }

        return points;
    }
}
