using System.Collections.Generic;
using Godot;
using Godot.Collections;

public class Player : Entity
{
    private const int MOVE_RANGE = 2;
    private const int ATTACK_RANGE = 3;
    private Line2D pathLine;
    private TileMap actionOverlay;
    private string currentAction;

    public override void _Ready()
    {
        base._Ready();

        pathLine = GetTree().Root.FindNode("Root", true, false).GetNode<Line2D>("PathLine");
        actionOverlay = GetTree().Root.FindNode("Root", true, false).GetNode<TileMap>("ActionOverlay");
    }

    public override void _Input(InputEvent @event)
    {
        if (!(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)) return;
        if (currentAction == "move")
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
                if (MapManager.GetPathDistance(path) > MOVE_RANGE || path.Count == 0)
                {
                    return;
                }

                Vector2[] realArray = new Vector2[path.Count];
                for (int i = 0; i < path.Count; i++)
                {
                    realArray[i] = mapMgr.MapToWorld(path[i]);
                }
                pathLine.Points = realArray;
                actionOverlay.Clear();
                Move(path, 0.2f);

                EnemyManager.Instance.MoveEnemies(path);
            }
        }
        else if (currentAction == "attack")
        {
            
        }
    }

    public void ActionSelected(string action)
    {
        actionOverlay.Clear();
        currentAction = action;
        switch (currentAction)
        {
            case "move":
                HighlightMoves();
                break;
            case "attack":
                HighlightDirections();
                break;
            case "rotate":
                break;
        }
    }

    protected override void OnMoveFinished()
    {
        base.OnMoveFinished();
        HighlightMoves();

        SceneTreeTimer timer = GetTree().CreateTimer(0.2f);
        timer.Connect("timeout", this, nameof(ClearPoints));
    }

    private void ClearPoints()
    {
        if (moving) return;
        pathLine.Points = new Vector2[0];
    }

    private void HighlightMoves()
    {
        foreach (Vector2 cell in mapMgr.GetValidMoves(currentMapPos, MOVE_RANGE))
        {
            actionOverlay.SetCell((int)cell.x, (int)cell.y, 0);
        }
    }

    private void HighlightDirections()
    {
        foreach (Vector2 direction in new Vector2[] { Vector2.Up, Vector2.Down, Vector2.Left, Vector2.Right })
        {
            int multiplier = 1;
            while (multiplier <= ATTACK_RANGE && mapMgr.moveableCells.Contains(currentMapPos + direction * multiplier))
            {
                actionOverlay.SetCell((int)(currentMapPos + direction * multiplier).x, (int)(currentMapPos + direction * multiplier).y, 0);
                multiplier++;
            }
        }
    }
}
