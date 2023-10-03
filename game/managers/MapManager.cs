using Godot;
using System;

public class MapManager : Singleton<MapManager>
{
    public const int GRID_WIDTH = 10;
    public const int GRID_HEIGHT = 10;
    public const int TILEMAP_SIZE = 32;

    private TileMap _map;

    public override void _Ready()
    {
        _map = GetNode<TileMap>("/root/Root/Map");
    }

    public Vector2 MapToWorld(Vector2 mapPos)
    {
        Vector2 halfTile = new(TILEMAP_SIZE / 2, TILEMAP_SIZE / 2);
        return _map.MapToWorld(mapPos) + halfTile;
    }
    public Vector2 WorldToMap(Vector2 worldPos)
    {
        return _map.WorldToMap(worldPos);
    }
}
