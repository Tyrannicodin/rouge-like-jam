using System.Collections.Generic;
using Godot;
using Godot.Collections;

public class Player : Entity
{
    private Line2D pathLine;

    public override void _Ready()
    {
        base._Ready();

        pathLine = GetTree().Root.FindNode("Root", true, false).GetNode<Line2D>("PathLine");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex != (int)ButtonList.Left) return;

            Vector2 targetPos = mapMgr.WorldToMap(mouseEvent.Position);

            bool withinGrid = targetPos.x < MapManager.GRID_WIDTH && targetPos.y < MapManager.GRID_HEIGHT;
            bool canMove = mapMgr.CanMoveToCell(targetPos);
            bool samePlace = targetPos == currentMapPos;

            if (withinGrid && canMove && !samePlace && !moving)
            {
                List<Vector2> path = mapMgr.GetPointPath(currentMapPos, targetPos);

                // @TODO temporary limit to move distance
                if (MapManager.GetPathDistance(path) > 5 || path.Count == 0)
                {
                    return;
                }

                Vector2[] realArray = new Vector2[path.Count];
                for (int i = 0; i < path.Count; i++)
                {
                    realArray[i] = mapMgr.MapToWorld(path[i]);
                }
                pathLine.Points = realArray;
                Move(path, 0.2f);

                EnemyManager.Instance.MoveEnemies(path);
            }
        }
    }

    protected override void OnMoveFinished()
    {
        base.OnMoveFinished();
        SceneTreeTimer timer = GetTree().CreateTimer(0.2f);
        timer.Connect("timeout", this, nameof(ClearPoints));
    }

    private void ClearPoints()
    {
        if (moving) return;
        pathLine.Points = new Vector2[0];
    }
}
