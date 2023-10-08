using System.Collections.Generic;
using Godot;
using Godot.Collections;

public class Bullet : Entity
{
	public Vector2 direction;

	protected override Vector2 ColliderSize => new(16, 15);

	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);
		foreach (Area2D collision in collider.GetOverlappingAreas())
		{
			Node collidingEntity = collision.GetParent();
			if (collidingEntity is Enemy)
			{
				EnemyManager.Instance.enemies.Remove(collidingEntity as Enemy);
				mapMgr.SetCellDisabled((collidingEntity as Enemy).currentMapPos, false);
				collidingEntity.QueueFree();
				BulletManager.Instance.bullets.Remove(collidingEntity as Bullet);
				collidingEntity.QueueFree();
			}
			else if (collidingEntity is Bullet)
			{
				BulletManager.Instance.bullets.Remove(collidingEntity as Bullet);
				collidingEntity.QueueFree();
			}
			else
			{
				//@TODO player collisions
			}
		}
	}

	public override void EntityReady()
	{
		// Add a sprite as a child
		Sprite sprite = new()
		{
			Texture = new AtlasTexture()
			{
				Atlas = ResourceLoader.Load("res://temp_sprites/enemies.png") as Texture,
				Region = new Rect2(0, 0, new Vector2(32, 32))
			},
			Scale = new Vector2(0.5f, 0.5f)
		};

		AddChild(sprite);
	}

	public Vector2? GetAction(List<Vector2> playerPath)
	{
		if (mapMgr.moveableCells.Contains(currentMapPos + direction))
		{
			return currentMapPos + direction;
		}
		return null;
	}
}