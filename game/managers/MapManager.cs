using Godot;
using System.Collections.Generic;

public class MapManager : Singleton<MapManager>
{
    public const int GRID_WIDTH = 10;
    public const int GRID_HEIGHT = 10;
    public const int TILEMAP_SIZE = 32;

    private TileMap _map;

    public readonly AStar2D Pathfinder = new();

    HashSet<Vector2> moveableCells = new();

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
        Pathfinder.Clear();
        moveableCells.Clear();

        // Add all cells to pathfinder
        foreach (Vector2 cell in Map.GetUsedCellsById(0))
        {
            moveableCells.Add(cell);
            Pathfinder.AddPoint(Pathfinder.GetAvailablePointId(), cell);
        }


        foreach (Vector2 cell in moveableCells)
        {
            Vector2[] directions = new[] { Vector2.Up, Vector2.Right, Vector2.Down, Vector2.Left };
            foreach (Vector2 direction in directions)
            {
                if (moveableCells.Contains(cell + direction))
                {
                    Pathfinder.ConnectPoints(Pathfinder.GetClosestPoint(cell), Pathfinder.GetClosestPoint(cell + direction), false);
                }
            }
        }
    }

    public Vector2[] GetPointPath(Vector2 fromMapPos, Vector2 toMapPos)
    {
        return Pathfinder.GetPointPath(
            Pathfinder.GetClosestPoint(fromMapPos),
            Pathfinder.GetClosestPoint(toMapPos)
        );
    }

    public bool CanMoveToPoint(Vector2 mapPos)
    {
        return moveableCells.Contains(mapPos);
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
}
