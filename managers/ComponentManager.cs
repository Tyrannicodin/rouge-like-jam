using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class ComponentManager : Singleton<ComponentManager>
{
    public const int GRID_WIDTH = 10;
    public const int GRID_HEIGHT = 10;
    public Vector2 TILEMAP_SIZE = new(17, 17);

    private readonly int[] valid_celltypes = new int[] { 0 };

    private TileMap _map;

    public readonly HashSet<Vector2> componentConnections = new();
    public readonly List<Vector2> moveableCells = new();
    public List<Component> components = new();
    public Type[] componentTypes = new[] { typeof(PowerSupply) };
    private int selectedType = -1;

    public TileMap Map
    {
        get
        {
            _map ??= GetTree().Root.FindNode("Components", true, false).GetNode<TileMap>("Background");
            return _map;
        }
    }

    public override void _Ready()
    {
        RecalculatePathfinder();
    }

    public List<Vector2> ValidCells()
    {
        List<Vector2> cells = new();
        foreach (int cell_type in valid_celltypes)
        {
            foreach (Vector2 cell in Map.GetUsedCellsById(cell_type))
            {
                cells.Add(cell);
            }
        }
        return cells;
    }

    public void RecalculatePathfinder()
    {
        componentConnections.Clear();
        moveableCells.Clear();
        components = new();

        // Add all cells to pathfinder
        foreach (Vector2 cell in ValidCells())
        {
            moveableCells.Add(cell);
        }
    }

    public void ComponentTypePicked(int idx)
    {
        selectedType = idx;
    }

    public override void _Input(InputEvent @event)
    {
        if (!(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)) return;
        if (mouseEvent.ButtonIndex != (int)ButtonList.Left) return;

        Vector2 targetPos = WorldToMap(mouseEvent.Position);

        bool withinGrid = targetPos.x < MapManager.GRID_WIDTH && targetPos.y < MapManager.GRID_HEIGHT;

        if (!withinGrid || selectedType < 0 || selectedType >= componentTypes.Length) return;
        PlaceComponent(selectedType, targetPos);
    }

    public void PlaceComponent(int componentTypeIdx, Vector2 position)
    {
        Component component = (Component)componentTypes[componentTypeIdx].GetConstructor(new Type[0]).Invoke(new object[0]);
        component.gridPos = position;
        AddChild(component);
        components.Add(component);
        componentConnections.Add(new(
            moveableCells.IndexOf(position),
            moveableCells.IndexOf(position + Component.EdgeToDirection(component.OutputLocation))
        ));
    }

    public bool ComponentsConnected(Component component1, Component component2)
    {
        if (!(moveableCells.Contains(component1.gridPos) && moveableCells.Contains(component2.gridPos))) return false;
        IEnumerable<Vector2> component1Connection = componentConnections.Where(connection => moveableCells.IndexOf(component1.gridPos) == connection.x);
        if (component1Connection.Count() == 0 || component1Connection.First().y != moveableCells.IndexOf(component2.gridPos)) return false;
        return true;
    }

    public EntityAttributes GetModifications()
    {
        EntityAttributes attributes = new();
        foreach (Component component in components)
        {
            if (!component.Supplier) continue;

        }
        return attributes;
    }

    public Vector2 MapToWorld(Vector2 mapPos)
    {
        Vector2 halfTile = new(TILEMAP_SIZE.x / 2, TILEMAP_SIZE.y / 2);
        return Map.MapToWorld(mapPos) + halfTile;
    }
    public Vector2 WorldToMap(Vector2 worldPos)
    {
        return Map.WorldToMap(worldPos);
    }
}
