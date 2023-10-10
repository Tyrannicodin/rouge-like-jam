using System.Collections.Generic;
using Godot;

public abstract class Entity : Node2D
{
    // references
    protected MapManager mapMgr;

    // Location on the map in map space
    public Vector2 currentMapPos;
    public bool moving = false;

    public EntityAttributes attributes = new();

    public AssemblyGrid assemblyGrid;

    protected Area2D collider;
    protected virtual Vector2 ColliderSize
    {
        get { return new(); }
    }

    public override void _Ready()
    {
        collider = new();
        CollisionShape2D collider_shape = new()
        {
            Shape = new RectangleShape2D()
            {
                Extents = ColliderSize / 2
            }
        };
        collider.AddChild(collider_shape);
        AddChild(collider);

        mapMgr = MapManager.Instance;
        currentMapPos = mapMgr.WorldToMap(Position);

        EntityReady();
    }

    // extension of the ready function for use by child classes
    public virtual void EntityReady() { }

    public void Move(List<Vector2> path, float speed)
    {
        // First off just tweening to that location
        SceneTreeTween positionTween = CreateTween();
        positionTween.SetTrans(Tween.TransitionType.Linear);
        foreach (Vector2 point in path)
        {
            if (point == currentMapPos)
            {
                // Don't move to where we already are
                continue;
            }
            positionTween.TweenProperty(this, "position", mapMgr.MapToWorld(point), speed);
        }

        moving = true;

        positionTween.Connect("finished", this, "OnMoveFinished");
        positionTween.Play();
    }

    protected virtual void OnMoveFinished()
    {
        // Set new map position and allow moving again
        currentMapPos = mapMgr.WorldToMap(Position);
        moving = false;
    }
}
