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

    public static float EdgeToRotation(Edge edge)
    {
        switch (edge)
        {
            case Edge.Right:
                return 90;
            case Edge.Bottom:
                return 180;
            case Edge.Left:
                return 270;
            default:
                return 0;
        }
    }

    public abstract Edge InputLocation { get; }
    public abstract Edge OutputLocation { get; }

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

    public override void ModifyAttributes(ref EntityAttributes attributes)
    {
        // This component increases range by 1
        attributes.range++;
    }
}