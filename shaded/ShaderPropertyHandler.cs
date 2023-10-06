using Godot;
using Godot.Collections;
using System;

public class ShaderPropertyHandler : ViewportContainer
{
    private Color initial_color = new(0.7f, 0.281f, 0.47f, 1);
    private Color final_color = new(0.87f, 0.09f, 0.87f, 1);

    public override void _Ready()
    {
        base._Ready();
        SceneTreeTween tweener = GetTree().CreateTween().SetTrans(Tween.TransitionType.Linear);
        tweener.SetLoops();
        tweener.TweenMethod(this, nameof(SetColor), 0f, 1f, 15, new Godot.Collections.Array { 1 });
        tweener.TweenMethod(this, nameof(SetColor), 1f, 0f, 15, new Godot.Collections.Array { 1 });
        tweener.Play();
    }

    public void SetColor(float progress, float maximum)
    {
        Color current_color = progress * final_color + (maximum - progress) * initial_color;

        (Material as ShaderMaterial).SetShaderParam("pink", current_color);

    }
}
