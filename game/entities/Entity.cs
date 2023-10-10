using System.Collections.Generic;
using Godot;

public abstract class Entity : Node2D
{
    // references
    protected MapManager mapMgr;

    // Location on the map in map space
    public Vector2 currentMapPos;

    // Current bullet we are firing
    public Bullet currentBullet = null;

    // Current tween
    public SceneTreeTween currentTween = null;

    public bool actionInProgress = false;

    public EntityAttributes attributes = new();

    public AssemblyGrid assemblyGrid = new();

    protected Area2D collider;
    protected virtual Vector2 ColliderSize
    {
        get { return new(); }
    }

    // Required overrides
    public abstract string Id { get; }

    public override void _Ready()
    {
        collider = new();
        CollisionShape2D colliderShape = new()
        {
            Shape = new RectangleShape2D()
            {
                Extents = ColliderSize / 2
            }
        };
        collider.AddChild(colliderShape);
        AddChild(collider);

        mapMgr = MapManager.Instance;
        currentMapPos = mapMgr.WorldToMap(Position);

        EntityReady();
    }

    // extension of the ready function for use by child classes
    public virtual void EntityReady() { }

    public void Move(List<Vector2> path, float speed)
    {
        actionInProgress = true;

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

        positionTween.Connect("finished", this, nameof(OnMoveFinished));
        positionTween.Play();


        currentTween = positionTween;
    }

    public void Shoot(Vector2 targetCell, float speed)
    {
        actionInProgress = true;

        // Create and move each bullet ourselves
        currentBullet = new(this)
        {
            Rotation = currentMapPos.AngleTo(targetCell)
        };
        AddChild(currentBullet);

        // Move the bullet
        SceneTreeTween bulletTween = CreateTween();
        bulletTween.SetTrans(Tween.TransitionType.Linear);
        bulletTween.TweenProperty(currentBullet, "position", ToLocal(mapMgr.MapToWorld(targetCell)), 0.2f * currentMapPos.DistanceTo(targetCell));

        bulletTween.Connect("finished", this, nameof(OnShootFinished));
        bulletTween.Play();

        currentTween = bulletTween;
    }

    protected virtual void OnMoveFinished()
    {
        // Set new map position and allow moving again
        currentMapPos = mapMgr.WorldToMap(Position);
        actionInProgress = false;

        // Set the current tween to null again
        currentTween = null;
    }

    protected virtual void OnShootFinished()
    {
        // We finished shooting without hitting anything, destroy the bullet
        actionInProgress = false;
        DestroyBullet();
    }

    public virtual void OnBulletHit(Node node)
    {
        if (currentBullet == null) return;
        actionInProgress = false;

        DestroyBullet();
    }

    public void DestroyBullet()
    {
        if (currentBullet == null) return;
        // First stop the current tween
        currentTween.Stop();
        currentTween = null;

        currentBullet.QueueFree();
        currentBullet = null;
    }
}
