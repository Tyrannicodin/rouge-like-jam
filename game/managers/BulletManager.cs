
using System.Collections.Generic;
using Godot;

public class BulletManager : Singleton<BulletManager>
{
	// references 
	private MapManager mapMgr;

	public List<Bullet> bullets = new();

	public override void _Ready()
	{
		mapMgr = MapManager.Instance;
	}

	public void MoveBullets(List<Vector2> playerPath)
	{
		List<Bullet> toRemove = new();
		foreach (Bullet bullet in bullets)
		{
			var destination = bullet.GetAction(playerPath);
			if (destination == null)
			{
				toRemove.Add(bullet);
				continue;
			};

			bullet.Move(new() { bullet.currentMapPos, (Vector2)destination }, 0.2f);
		}
		foreach (Bullet bullet in toRemove)
		{
			bullets.Remove(bullet);
			bullet.QueueFree();
		}
	}

	public void Shoot(Vector2 from, Vector2 direction)
	{
		Bullet newBullet = new()
		{
			Position = mapMgr.MapToWorld(from),
			direction = direction
		};
		bullets.Add(newBullet);
		AddChild(newBullet);
	}
}