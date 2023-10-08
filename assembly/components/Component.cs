using Godot;
using System;


public abstract class Component : Node2D
{

    public enum Edge
    {
        Top,
        Right,
        Bottom,
        Left
    }

    public override void _Ready()
    {
        base._Ready();
        Position = ComponentManager.Instance.MapToWorld(gridPos);
        AddChild(new Sprite()
        {
            Texture = new AtlasTexture()
            {
                //@TODO switch to component spritesheet
                Atlas = ResourceLoader.Load("res://temp_sprites/enemies.png") as Texture,
                Region = new(SpritePosition.x, SpritePosition.y, new(32, 32))
            },
            Scale = new Vector2(0.5f, 0.5f)
        });
    }

    public static float EdgeToRotation(Edge edge)
    {
        return edge switch
        {
            Edge.Right => 90,
            Edge.Bottom => 180,
            Edge.Left => 270,
            _ => (float)0,
        };
    }

    public static Vector2 EdgeToDirection(Edge edge)
    {
        return edge switch
        {
            Edge.Right => Vector2.Right,
            Edge.Bottom => Vector2.Down,
            Edge.Left => Vector2.Left,
            _ => Vector2.Up,
        };
    }

    public abstract Edge InputLocation { get; }
    public abstract Edge OutputLocation { get; }
    public abstract bool Supplier { get; }
    public abstract Vector2 SpritePosition { get; }
    public abstract string ComponentName { get; }
    public abstract string ComponentDescription { get; }

    public abstract void ModifyAttributes(ref EntityAttributes attributes);

    // Info about current position
    public Vector2 gridPos;
    public Edge facingDirection = Edge.Top;
}


public class MyFancyComponent : Component
{
    // input is at the bottom
    public override Edge InputLocation => Edge.Bottom;

    // output is at the top
    public override Edge OutputLocation => Edge.Top;

    // does not supply power
    public override bool Supplier => false;

    // sprite is at 0,0 on the spritesheet
    public override Vector2 SpritePosition => new(0, 0);

    // display name is fancy part
    public override string ComponentName => "Fancy part";

    // describe what it does
    public override string ComponentDescription => "This component is very shiny!\n+1 movement range";

    public override void ModifyAttributes(ref EntityAttributes attributes)
    {
        // This component increases range by 1
        attributes.range++;
    }
}