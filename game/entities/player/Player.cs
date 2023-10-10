using System.Collections.Generic;
using Godot;

public class Player : Entity
{
    private const int MOVE_RANGE = 2;
    private const int ATTACK_RANGE = 3;
    private Line2D pathLine;
    private TileMap actionOverlay;
    private string currentAction;

    protected override Vector2 ColliderSize => new(16, 15);

    public override void _Ready()
    {
        base._Ready();

        pathLine = GetTree().Root.FindNode("Root", true, false).GetNode<Line2D>("PathLine");
        actionOverlay = GetTree().Root.FindNode("Root", true, false).GetNode<TileMap>("ActionOverlay");
    }

    public override void _Input(InputEvent @event)
    {
        if (!(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)) return;
        if (mouseEvent.ButtonIndex != (int)ButtonList.Left) return;

        Vector2 targetPos = mapMgr.WorldToMap(mouseEvent.Position);
        List<Vector2> path = new();

        bool withinGrid = targetPos.x < MapManager.GRID_WIDTH && targetPos.y < MapManager.GRID_HEIGHT;
        bool canMove = mapMgr.IsCellMoveable(targetPos);
        bool samePlace = targetPos == currentMapPos;

        if (!withinGrid || !canMove || samePlace || moving) return;
        if (currentAction == "move")
        {
            path = mapMgr.GetPointPath(currentMapPos, targetPos);
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
        }
        else if (currentAction == "attack")
        {
            Vector2 direction = targetPos - currentMapPos;
            if (direction.Abs().x >= direction.Abs().y)
            {
                direction.y = 0;
            }
            else
            {
                direction.x = 0;
            }
            if (direction == Vector2.Zero) return;
            BulletManager.Instance.Shoot(currentMapPos, direction.Normalized());
            path = new() { currentMapPos };
        }
        if (path.Count > 0)
        {
            EnemyManager.Instance.MoveEnemies(path);
            BulletManager.Instance.MoveBullets(path);
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
        pathLine.Points = System.Array.Empty<Vector2>();
    }

    private void HighlightMoves()
    {
        var moves = mapMgr.GetValidMoves(currentMapPos, MOVE_RANGE);
        foreach (Vector2 cell in moves)
        {
            actionOverlay.SetCell((int)cell.x, (int)cell.y, 0);
        }
    }

    private void HighlightDirections()
    {
        foreach (Vector2 direction in new Vector2[] { Vector2.Up, Vector2.Down, Vector2.Left, Vector2.Right })
        {
            int multiplier = 1;
            while (multiplier <= ATTACK_RANGE && mapMgr.IsCellMoveable(currentMapPos + direction * multiplier))
            {
                actionOverlay.SetCell((int)(currentMapPos + direction * multiplier).x, (int)(currentMapPos + direction * multiplier).y, 0);
                multiplier++;
            }
        }
    }
}
