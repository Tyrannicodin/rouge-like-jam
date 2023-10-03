using Godot;
using System;

public class Enemy : Sprite
{
	private Vector2 GridPosition = new();
	private readonly RandomNumberGenerator RNG = new();

	public override void _Ready()
	{
		Offset = new(16, 16);
		Texture = new AtlasTexture()
		{
			Atlas = ResourceLoader.Load("res://temp_sprites/enemies.png") as Texture,
			Region = new Rect2(32, 0, new Vector2(32, 32))
		};
	}


	public bool Move(Vector2 distance, bool use_tween = true)
	{
		var new_position = distance + GridPosition;
		if (new_position.x >= MapManager.GRID_WIDTH || new_position.y >= MapManager.GRID_HEIGHT || new_position.x < 0 || new_position.y < 0)
		{
			return false;
		}
		GridPosition = new_position;
		if (use_tween)
		{
			SceneTreeTween position_tween = CreateTween();
			position_tween.TweenProperty(this, "position", GridPosition * MapManager.TILEMAP_SIZE, 1);
			position_tween.Play();
		}
		else
		{
			Position = GridPosition * MapManager.TILEMAP_SIZE;
		}
		return true;
	}

	public void PlayerAction()
	{ //TODO: Arguments here
		Vector2 movement;
		int direction;
		do
		{
			direction = RNG.RandiRange(0, 3);
			if (direction > 1)
			{
				movement = new(0, (direction - 2) * 2 - 1);
			}
			else
			{
				movement = new(direction * 2 - 1, 0);
			}
		} while (!Move(movement));
	}
}