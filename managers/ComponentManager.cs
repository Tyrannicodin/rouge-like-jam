using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class ComponentManager : Singleton<ComponentManager>
{
    public const int GRID_WIDTH = 5;
    public const int GRID_HEIGHT = 5;
    public Vector2 TILEMAP_SIZE = new(17, 17);

    private readonly int[] valid_celltypes = new int[] { 0 };
    private readonly Type[] ignored_component_types = new Type[] { typeof(MyFancyComponent) };

    private TileMap _map;
    private VBoxContainer _component_buttons;

    public readonly HashSet<Vector2> componentConnections = new();
    public readonly List<Vector2> moveableCells = new();
    public Dictionary<Vector2, Component> components = new();
    public Type[] componentTypes;
    private int selectedType = -1;

    public TileMap Map
    {
        get
        {
            _map ??= GetTree().Root.FindNode("Components", true, false).GetNode<TileMap>("Background");
            return _map;
        }
    }
    public VBoxContainer ComponentButtons
    {
        get
        {
            _component_buttons ??= GetTree().Root.FindNode("Components", true, false).GetNode("Components").GetNode<VBoxContainer>("Inner");
            return _component_buttons;
        }
    }

    public override void _Ready()
    {
        componentTypes = typeof(Component).Assembly.GetTypes().Where(subType => subType.IsSubclassOf(typeof(Component)) && !ignored_component_types.Contains(subType)).ToArray();

        RecalculatePathfinder(); ;
        int i = 0;
        foreach (Type componentType in componentTypes)
        {
            // Can't have static virtual types so we create an instance that we discard after the loop
            Component tempComponent = (Component)componentType.GetConstructor(new Type[0]).Invoke(new object[0]);
            Button componentButton = new()
            {
                Name = componentType.Name,
                Text = tempComponent.ComponentName,
                HintTooltip = tempComponent.ComponentDescription,
                ToggleMode = true,
                Theme = ResourceLoader.Load<Theme>("res://art/themes/button/button.tres")
            };
            componentButton.AddConstantOverride("font_size", 10);
            componentButton.Connect("pressed", this, "ComponentTypePicked", new Godot.Collections.Array { i });
            ComponentButtons.AddChild(componentButton);
            i++;
        }
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

        foreach (Vector2 cell in ValidCells())
        {
            moveableCells.Add(cell);
        }
    }

    public void ComponentTypePicked(int idx)
    {
        foreach (Button button in ComponentButtons.GetChildren())
        {
            button.Pressed = false;
        }
        if (idx == selectedType)
        {
            selectedType = -1;
        }
        else
        {

            ComponentButtons.GetChild<Button>(idx).Pressed = true;
            selectedType = idx;
        }
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
        if (!moveableCells.Contains(position) || components.ContainsKey(position)) return;
        Component component = (Component)componentTypes[componentTypeIdx].GetConstructor(new Type[0]).Invoke(new object[0]);
        component.gridPos = position;
        AddChild(component);
        components.Add(position, component);
        componentConnections.Add(new(
            moveableCells.IndexOf(position),
            moveableCells.IndexOf(position + Component.EdgeToDirection(component.OutputLocation))
        ));
    }

    public EntityAttributes GetModifications()
    {
        EntityAttributes attributes = new();
        List<Component> modifiers = new();
        foreach (Component component in components.Values)
        {
            if (!component.Supplier) continue;
            modifiers.Clear();
            modifiers.Add(component);

            Component currentComponent = component;
            Vector2 previousEdge;
            bool connected = true;
            do
            {
                previousEdge = Component.EdgeToDirection(currentComponent.OutputLocation);
                if (
                    !components.ContainsKey(previousEdge + currentComponent.gridPos) ||
                    previousEdge != -Component.EdgeToDirection(components[previousEdge + currentComponent.gridPos].InputLocation)
                )
                {
                    connected = false;
                    break;
                };
                currentComponent = components[previousEdge + currentComponent.gridPos];
            } while (currentComponent != component);
            if (!connected) continue;

            foreach (Component modifiyingComponent in modifiers)
            {
                modifiyingComponent.ModifyAttributes(ref attributes);
            }
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
        return Map.WorldToMap(Map.ToLocal(worldPos));
    }
}
