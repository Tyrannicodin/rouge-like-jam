using Godot;
using Godot.Collections;

public struct EntityAttributes
{
    public int health;
    public int range;
    public int damage;
    public int moveSpeed;

    public EntityAttributes(int health = 1, int range = 1, int damage = 1, int moveSpeed = 1)
    {
        this.health = health;
        this.range = range;
        this.damage = damage;
        this.moveSpeed = moveSpeed;
    }
}

public struct AssemblyGrid
{
    public Vector2 size;
    public Dictionary<Vector2, Component> components;
    public Array<Vector2> blockedTiles;
}

public abstract class Entity : Node2D
{
    // references
    protected MapManager mapMgr;

    // Location on the map in map space
    public Vector2 currentMapPos;
    public bool moving = false;
    // I need a number of things on every entity
    // map location
    // attributes
    // components
    // sprite?

    public EntityAttributes attributes = new();

    public AssemblyGrid assemblyGrid;

    public override void _Ready()
    {
        mapMgr = MapManager.Instance;
        currentMapPos = mapMgr.WorldToMap(Position);

        EntityReady();
    }

    // extension of the ready function for use by child classes
    public virtual void EntityReady() { }

    public void Move(Vector2 targetPos, float speed)
    {
        // First off just tweening to that location
        SceneTreeTween positionTween = CreateTween();
        positionTween.SetTrans(Tween.TransitionType.Linear);

        Vector2[] path = mapMgr.GetPointPath(currentMapPos, targetPos);
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
