using System.Collections.Generic;
using Godot;

public class Bullet : Node2D
{
	public Entity parentEntity;


	private Area2D collider;

	public Bullet(Entity entity)
	{
		parentEntity = entity;
	}

	public override void _Ready()
	{
		// Add a collider as a child
		collider = new();
		CollisionShape2D colliderShape = new()
		{
			Shape = new RectangleShape2D()
			{
				Extents = new Vector2(4, 4)
			}
		};
		collider.AddChild(colliderShape);
		AddChild(collider);

		// Add a sprite as a child
		Sprite sprite = new()
		{
			Texture = new AtlasTexture()
			{
				Atlas = ResourceLoader.Load<Texture>("res://temp_sprites/background.png"),
				Region = new Rect2(0, 0, new Vector2(32, 32))
			},
			Scale = new Vector2(0.2f, 0.2f)
		};
		AddChild(sprite);
	}

	public override void _PhysicsProcess(float delta)
	{
		foreach (Area2D collision in collider.GetOverlappingAreas())
		{
			Node colliding = collision.GetParent();
			parentEntity.OnBulletHit(colliding);
		}
	}
}