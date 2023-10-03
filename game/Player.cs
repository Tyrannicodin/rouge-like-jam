using Godot;

public class Player : Node2D
{
    private MapManager _mapMgr;

    private bool _moving = false;
    private Vector2 _currentMapPos;

    public override void _Ready()
    {
        _mapMgr = MapManager.Instance;

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
                Move(mapPos, 1);
            }
        }
    }

    public void Move(Vector2 mapPos, float time)
    {
        // First off just tweening to that location
        SceneTreeTween positionTween = CreateTween();
        positionTween.TweenProperty(this, "position", _mapMgr.MapToWorld(mapPos), time);
        positionTween.Play();
    }
}
