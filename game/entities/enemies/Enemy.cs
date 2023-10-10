using System.Collections.Generic;
using Godot;
using Godot.Collections;

public class Enemy : Entity
{
	public override string Id => "enemy";

	protected override Vector2 ColliderSize => new(16, 15);

	public override void EntityReady()
	{
		// Add a sprite as a child
		Sprite sprite = new()
		{
			Texture = new AtlasTexture()
			{
				Atlas = ResourceLoader.Load<Texture>("res://temp_sprites/enemies.png"),
				Region = new Rect2(32, 0, new Vector2(32, 32))
			},
			Scale = new Vector2(0.5f, 0.5f)
		};
		AddChild(sprite);
	}

	// @TODO needs to return an action object.
	// Player action is also just vector2 passed in
	public Vector2? GetAction(List<Vector2> playerPath)
	{
		List<Vector2> playerPoints = MapManager.PathToPoints(playerPath);

		bool CanMoveToLocation(Vector2 location)
		{
			bool noPlayer = !playerPoints.Contains(location);
			bool canMove = mapMgr.IsCellMoveable(location);

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