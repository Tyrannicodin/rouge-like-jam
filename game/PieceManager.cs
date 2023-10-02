using Godot;
using System;
using System.Collections.Generic;

public class PieceManager : Node
{
    [Export]
    public Godot.Collections.Array<Vector2> enemy_positions = new();

    private readonly List<Enemy> enemies = new();

    public override void _Ready()
    {
        Enemy new_enemy;
        foreach (var position in enemy_positions)
        {
            new_enemy = new();
            new_enemy.Move(position, false);
            enemies.Add(new_enemy);
            AddChild(new_enemy, true);
        }
    }

    public void Turn()
    {
        foreach (var enemy in enemies)
        {
            enemy.PlayerAction();
        }
    }
}