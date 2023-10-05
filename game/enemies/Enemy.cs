using Godot;
using Godot.Collections;

public class Enemy : Entity
{
	private readonly RandomNumberGenerator RNG = new();

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
	public Vector2? GetAction(Vector2[] playerPath)
	{
		Array<Vector2> playerPoints = MapManager.PathToPoints(playerPath);

		bool CanMoveToLocation(Vector2 location)
		{
			bool noPlayer = !playerPoints.Contains(location);
			bool canMove = mapMgr.CanMoveToCell(location);

			return noPlayer && canMove;
		}


		Array<Vector2> directions = new(Vector2.Up, Vector2.Right, Vector2.Down, Vector2.Left);
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


	//public bool Move(Vector2 distance, bool use_tween = true)
	//{
	//	var new_position = distance + GridPosition;
	//	if (new_position.x >= MapManager.GRID_WIDTH || new_position.y >= MapManager.GRID_HEIGHT || new_position.x < 0 || new_position.y < 0)
	//	{
	//		return false;
	//	}
	//	GridPosition = new_position;
	//	if (use_tween)
	//	{
	//		SceneTreeTween position_tween = CreateTween();
	//		position_tween.TweenProperty(this, "position", GridPosition * MapManager.TILEMAP_SIZE, 1);
	//		position_tween.Play();
	//	}
	//	else
	//	{
	//		Position = GridPosition * MapManager.TILEMAP_SIZE;
	//	}
	//	return true;
	//}

	//public void PlayerAction()
	//{ //TODO: Arguments here
	//	Vector2 movement;
	//	int direction;
	//	do
	//	{
	//		direction = RNG.RandiRange(0, 3);
	//		if (direction > 1)
	//		{
	//			movement = new(0, (direction - 2) * 2 - 1);
	//		}
	//		else
	//		{
	//			movement = new(direction * 2 - 1, 0);
	//		}
	//	} while (!Move(movement));
	//}
}