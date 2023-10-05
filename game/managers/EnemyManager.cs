
using Godot;
using Godot.Collections;

public class EnemyManager : Singleton<EnemyManager>
{
    // references 
    private MapManager mapMgr;

    [Export]
    public Array<Vector2> enemyPositions = new();

    public Array<Enemy> enemies = new();

    public override void _Ready()
    {
        mapMgr = MapManager.Instance;

        // create the enemies
        Enemy newEnemy;
        foreach (var position in enemyPositions)
        {
            newEnemy = new()
            {
                Position = mapMgr.MapToWorld(position)
            };
            enemies.Add(newEnemy);
            AddChild(newEnemy, true);
        }
    }

    public void MoveEnemies(Vector2[] playerPath)
    {
        foreach (Enemy enemy in enemies)
        {
            // Re-enable it's last position
            mapMgr.SetCellDisabled(enemy.currentMapPos, false);

            var destination = enemy.GetAction(playerPath);
            Vector2[] path = mapMgr.GetPointPath(enemy.currentMapPos, destination);
            enemy.Move(path, 0.2f);

            // Disable the new position
            mapMgr.SetCellDisabled(destination, true);
        }
    }



    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex != (int)ButtonList.Right) return;

            Vector2 targetPos = MapManager.Instance.WorldToMap(mouseEvent.Position);

            bool withinGrid = targetPos.x < MapManager.GRID_WIDTH && targetPos.y < MapManager.GRID_HEIGHT;

            if (withinGrid)
            {
                MapManager.Instance.SetCellDisabled(targetPos, true);
            }
        }
    }
}