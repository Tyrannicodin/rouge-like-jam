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

    public override string Id => "player";

    public override void _Ready()
    {
        base._Ready();

        pathLine = GetTree().Root.FindNode("Root", true, false).GetNode<Line2D>("PathLine");
        actionOverlay = GetTree().Root.FindNode("Root", true, false).GetNode<TileMap>("ActionOverlay");
    }

    public override void _Input(InputEvent @event)
    {
        if (actionInProgress) return;

        if (!(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)) return;
        if (mouseEvent.ButtonIndex != (int)ButtonList.Left) return;

        Vector2 targetPos = mapMgr.WorldToMap(mouseEvent.Position);

        bool hasOverlayCell = actionOverlay.GetCellv(targetPos) != TileMap.InvalidCell;
        if (!hasOverlayCell) return;

        // Clear the overlay now, we are about to take an action.
        actionOverlay.Clear();

        Action chosenAction = Action.None;

        if (currentAction == "move")
        {
            var path = mapMgr.GetPointPath(currentMapPos, targetPos);
            Move(path, 0.2f);

            // Action is move action
            chosenAction = new Action(Action.Type.Move, path);

            // Movement line - disabled temporarily
            //Vector2[] realArray = new Vector2[path.Count];
            //for (int i = 0; i < path.Count; i++)
            //{
            //    realArray[i] = mapMgr.MapToWorld(path[i]);
            //}
            //pathLine.Points = realArray;
        }
        else if (currentAction == "attack")
        {
            Shoot(targetPos, 0.2f);

            // Action is shoot action
            chosenAction = new Action(Action.Type.Shoot, new() { currentMapPos, targetPos });
        }
        if (chosenAction.type != Action.Type.None)
        {
            EnemyManager.Instance.ExecuteEnemyActions(chosenAction);
        }
    }

    // Called by action buttons
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
                HighlightTargets();
                break;
            default:
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
    protected override void OnShootFinished()
    {
        base.OnShootFinished();
        HighlightTargets();
    }

    public override void OnBulletHit(Node node)
    {
        if (currentBullet == null) return;
        actionInProgress = false;

        // The bullet hit something, determine what it is
        if (node is Enemy enemy)
        {
            GD.Print("hit enemy, destroying bullet");
            DestroyBullet();
            HighlightTargets();
        }
        else if (node is Bullet bullet)
        {
            GD.Print("hit bullet, destroying both");
            DestroyBullet();
            bullet.parentEntity.DestroyBullet();
            HighlightTargets();
        }
        else
        {
            //@TODO other collisions?
        }
    }

    private void ClearPoints()
    {
        if (actionInProgress) return;
        pathLine.Points = System.Array.Empty<Vector2>();
    }


    // Action highlights
    private void HighlightMoves()
    {
        var moves = mapMgr.GetValidMoves(currentMapPos, MOVE_RANGE);
        foreach (Vector2 cell in moves)
        {
            actionOverlay.SetCell((int)cell.x, (int)cell.y, 0);
        }
    }

    private void HighlightTargets()
    {
        foreach (Vector2 direction in new Vector2[] { Vector2.Up, Vector2.Down, Vector2.Left, Vector2.Right })
        {
            int multiplier = 1;
            while (true)
            {
                Vector2 targetCell = currentMapPos + (direction * multiplier);
                if (mapMgr.disabledCells.Contains(targetCell))
                {
                    // There's an enemy here, highlight it but then stop.
                    actionOverlay.SetCellv(targetCell, 0);
                    break;
                }
                else if (!mapMgr.moveableCells.Contains(targetCell))
                {
                    // It's an immovable cell, stop immediately without adding it as an option to shoot at.
                    break;
                }

                actionOverlay.SetCellv(targetCell, 0);

                multiplier++;
                if (multiplier > ATTACK_RANGE) break;
            }
        }
    }
}
