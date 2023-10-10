
using System.Collections.Generic;
using System.Linq;
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

    public void ExecuteEnemyActions(Action playerAction)
    {
        foreach (Enemy enemy in enemies)
        {
            Action enemyAction = enemy.GetAction(playerAction);

            switch (enemyAction.type)
            {
                case Action.Type.Move:
                    // Move the enemy using the path returned
                    enemy.Move(enemyAction.info, 0.2f);
                    break;
                default:
                    break;
            }
        }
    }
}