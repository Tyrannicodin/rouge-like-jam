using Godot;
using System;

public class ActionButtons : Node2D
{
    [Signal] public delegate void OnSelectedActionChanged();

    private Button _moveButton;
    private Button _attackButton;
    private Button _rotateButton;

    // none or action id.
    private string _selectedAction = "none";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _moveButton = GetNode<Button>("Move");
        _attackButton = GetNode<Button>("Attack");
        _rotateButton = GetNode<Button>("Rotate");
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
                case "rotate":
                    _rotateButton.Pressed = false;
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
