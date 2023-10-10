using System.Collections.Generic;

namespace Godot
{

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

    // Object holding values 
    public struct AssemblyGrid
    {
        public Vector2 size;
        public Dictionary<Vector2, Component> components;
        public List<Vector2> blockedTiles;
    }

    public struct Action
    {
        public enum Type
        {
            Move,
            Shoot,
            Repair
        }

        public Type type;

        /// <summary>
        /// The info this holds is different for each action.
        /// For moving a list of path points to move between.
        /// For shooting the first vector in the list holds the target tile.
        /// For repairs this should be left blank.
        /// </summary>
        public List<Vector2> info;

        public Action(Type type, List<Vector2> info)
        {
            this.type = type;
            this.info = info;
        }
    }
}