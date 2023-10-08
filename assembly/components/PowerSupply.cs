using Godot;

public class PowerSupply : Component
{
	public PowerSupply()
	{
		GD.Print("PSU created");
	}

	public override Edge InputLocation => Edge.Left;
	public override Edge OutputLocation => Edge.Right;
	public override bool Supplier => true;
	public override Vector2 SpritePosition => new(0, 0);

	public override string ComponentName => "Power supply";
	public override string ComponentDescription => "Provides power to a circuit";

	public override void ModifyAttributes(ref EntityAttributes attributes)
	{

	}
}