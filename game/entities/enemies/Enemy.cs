using System.Collections.Generic;
using Godot;
using Godot.Collections;

public class Enemy : Entity
{
	public override void EntityReady()
	{
		// Add a sprite as a child
		Sprite sprite = new()
		{
			Texture = new AtlasTexture()
			{
				Atlas = ResourceLoader.Load("res://temp_sprites/enemies.png") as Texture,
				Region = new Rect2(32, 0, new Vector2(32, 32))
			}
		};
		AddChild(sprite);
	}

	// @TODO for now just returns the new position to move to
	// Player action is also just vector2 passed in
	public Vector2? GetAction(List<Vector2> playerPath)
	{
		List<Vector2> playerPoints = MapManager.PathToPoints(playerPath);

		bool CanMoveToLocation(Vector2 location)
		{
			bool noPlayer = !playerPoints.Contains(location);
			bool canMove = mapMgr.CanMoveToCell(location);

			return noPlayer && canMove;
		}


		List<Vector2> directions = new() { Vector2.Up, Vector2.Right, Vector2.Down, Vector2.Left };
		Array<Vector2> availableDirections = new();
		foreach (Vector2 direction in directions)
		{
			if (CanMoveToLocation(currentMapPos + direction))
			{
				availableDirections.Add(direction);
			}
		}

		if (availableDirections.Count == 0) return null;
		availableDirections.Shuffle();

		return currentMapPos + availableDirections[0];
	}
}