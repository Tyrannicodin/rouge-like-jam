using Godot;

public class Motor : Component
{
	public override Edge InputLocation => Edge.Left;
	public override Edge OutputLocation => Edge.Top;
	public override bool Supplier => false;
	public override Vector2 SpritePosition => new(1, 0);
	public override string ComponentName => "Motor";
	public override string ComponentDescription => "Keep it moving!\n+1 movement range";

	public override void ModifyAttributes(ref EntityAttributes attributes)
	{
		attributes.range++;
	}
}