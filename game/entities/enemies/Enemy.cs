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

	public Action GetAction(Action playerAction)
	{
		List<Vector2> noEntryCells = playerAction.type switch
		{
			Action.Type.Move => MapManager.PathToPoints(playerAction.info),
			Action.Type.Shoot => new() { playerAction.info[0] },
			_ => new(),
		};

		// Here is where enemy will decide what to do depending on the player action
		// For now just moe randomly

		bool CanMoveToLocation(Vector2 location)
		{
			bool noPlayer = !noEntryCells.Contains(location);
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

		if (availableDirections.Count == 0) return Action.None;
		availableDirections.Shuffle();


		// Re-enable our last position
		mapMgr.SetCellDisabled(currentMapPos, false);

		List<Vector2> ourPath = mapMgr.GetPointPath(currentMapPos, currentMapPos + availableDirections[0]);

		// Disable the final position - this is done *instantly* on purpose to prevent any kind of early clicking where an enemy will be.
		mapMgr.SetCellDisabled(currentMapPos + availableDirections[0], true);

		// Return a move action
		return new Action(Action.Type.Move, ourPath);
	}
}