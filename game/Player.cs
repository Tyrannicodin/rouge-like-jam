using System.Collections.Generic;
using Godot;
using Godot.Collections;

public class Player : Node2D
{
    private readonly Vector2[] DIRECTIONS = new Vector2[] { new(0, 1), new(0, -1), new(1, 0), new(-1, 0) };

    private MapManager _mapMgr;

    private bool _moving = false;
    private Vector2 _currentMapPos;

    private AStar2D _pathfinder = new();
    private HashSet<Vector2> _usedCells = new();

    public override void _Ready()
    {
        _mapMgr = MapManager.Instance;

        foreach (Vector2 cell in _mapMgr.Map.GetUsedCellsById(0))
        {
            _usedCells.Add(cell);
            _pathfinder.AddPoint(_pathfinder.GetAvailablePointId(), cell);
        }

        foreach (Vector2 cell in _usedCells)
        {
            foreach (Vector2 direction in DIRECTIONS)
            {
                if (_usedCells.Contains(cell + direction))
                {
                    _pathfinder.ConnectPoints(_pathfinder.GetClosestPoint(cell), _pathfinder.GetClosestPoint(cell + direction), false);
                }
            }
        }

        _currentMapPos = _mapMgr.WorldToMap(Position);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex != (int)ButtonList.Left) return;

            Vector2 mapPos = _mapMgr.WorldToMap(mouseEvent.Position);
            if (mapPos.x < MapManager.GRID_WIDTH && mapPos.y < MapManager.GRID_HEIGHT)
            {
                Move(mapPos, 0.5f);
            }
        }
    }

    public void Move(Vector2 mapPos, float speed)
    {
        // First off just tweening to that location
        SceneTreeTween positionTween = CreateTween();

        Vector2[] path = _pathfinder.GetPointPath(
            _pathfinder.GetClosestPoint(_currentMapPos),
            _pathfinder.GetClosestPoint(mapPos)
        );
        bool first = true;
        foreach (Vector2 point in path)
        {
            if (first)
            {
                first = false;
                continue;
            }
            positionTween.TweenProperty(this, "position", _mapMgr.MapToWorld(point), speed);
        }
        positionTween.Play();
        _currentMapPos = mapPos;
    }
}
