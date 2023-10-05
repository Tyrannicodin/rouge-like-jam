using System.Collections.Generic;
using Godot;
using Godot.Collections;

public class Player : Entity
{

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
                Vector2[] path = mapMgr.GetPointPath(currentMapPos, targetPos);

                // @TODO temporary limit to move distance
                if (MapManager.GetPathDistance(path) > 2)
                {
                    return;
                }

                Move(path, 0.2f);

                EnemyManager.Instance.MoveEnemies(path);
            }
        }
    }
}
