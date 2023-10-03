using Godot;

public class Singleton<T> : Node where T : Node
{
    private static T instance = null;

    /// <summary>
    /// Only accessible in the ready function or later
    /// </summary>
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GD.PrintErr(
                    "Failed to get " + typeof(T).Name + " instance.\n" +
                    "Please make sure you are not trying to access the instance before ready!"
                    );
            }
            return instance;
        }
    }

    public override void _EnterTree()
    {
        if (instance == null)
        {
            GD.PrintErr("There is more than one " + typeof(T).Name + " singleton in the scene!");
            return;
        }
        instance = this as T;
    }

    public override void _ExitTree()
    {
        instance = null;
    }
}

