using Godot;
using System;

public class ActionButtons : HBoxContainer
{
    [Signal] public delegate void OnSelectedActionChanged(string action);

    private Button _moveButton;
    private Button _attackButton;

    // none or action id.
    private string _selectedAction = "none";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _moveButton = GetNode<Button>("Move");
        _attackButton = GetNode<Button>("Attack");
    }

    public void ChangeSelectedAction(string actionId, bool noEmit)
    {
        if (_selectedAction == actionId)
        {
            // We already have this action button selected, deselect.
            switch (actionId)
            {
                case "move":
                    _moveButton.Pressed = false;
                    break;
                case "attack":
                    _attackButton.Pressed = false;
                    break;
                default:
                    // Somehow invalid action was sent, do nothing.
                    return;
            }
            _selectedAction = "none";
        }
        else
        {
            _selectedAction = actionId;
        }

        if (!noEmit)
        {
            EmitSignal("OnSelectedActionChanged", _selectedAction);
        }
    }
}
