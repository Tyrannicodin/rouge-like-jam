using Godot;
using System;

public class MapManager : Singleton<MapManager>
{
    public const int GRID_WIDTH = 10;
    public const int GRID_HEIGHT = 10;
    public const int TILEMAP_SIZE = 32;

    private TileMap _map;

    public TileMap Map
    {
        get
        {
            _map ??= GetTree().Root.FindNode("Root", true, false).GetNode<TileMap>("Map");
            return _map;
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
}
