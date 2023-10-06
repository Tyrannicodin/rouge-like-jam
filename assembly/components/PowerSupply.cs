using Godot;

public class PowerSupply : Component
{
	// input is at the bottom
	public override Edge InputLocation => Edge.Bottom;

	// output is at the top
	public override Edge OutputLocation => Edge.Top;

	// display name is fancy part
	public override string ComponentName => "Power supply";

	// describe what it does
	public override string ComponentDescription => "Provides power to a circuit";

	public override void ModifyAttributes(ref EntityAttributes attributes)
	{

	}
}