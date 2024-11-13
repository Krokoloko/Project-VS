using Godot;
using System;

public abstract class AttackController : Node
{
    protected Player player;
    
    public abstract void PreformAction(EACTION_TYPE action_type, float speed, bool reverse_direction);
    public abstract void StopAction();
}
