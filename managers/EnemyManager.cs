
using System.Collections.Generic;
using Godot;

public class EnemyManager : Singleton<EnemyManager>
{
    // references 
    private MapManager mapMgr;

    [Export]
    public List<Vector2> enemyPositions = new();

    public List<Enemy> enemies = new();

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
            // Disable the starting position
            mapMgr.SetCellDisabled(position, true);
        }
    }

    public void MoveEnemies(List<Vector2> playerPath)
    {
        foreach (Enemy enemy in enemies)
        {

            var destination = enemy.GetAction(playerPath);
            if (destination == null) continue;

            // Re-enable it's last position
            mapMgr.SetCellDisabled(enemy.currentMapPos, false);

            List<Vector2> path = mapMgr.GetPointPath(enemy.currentMapPos, (Vector2)destination);
            enemy.Move(path, 0.2f);

            // Disable it's new position - this is done *instantly* on purpose to prevent any kind of early clicking where an enemy will be.
            mapMgr.SetCellDisabled((Vector2)destination, true);
        }
    }
}