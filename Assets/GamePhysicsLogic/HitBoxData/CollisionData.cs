using Godot;
using System;
using System.Dynamic;

public partial class CollisionData : Resource
{
    [Export]
    public Vector2 offset {get; set;}

    [Export]
    public Vector2 size {get; set;}

    [Export]
    public float base_damage {get; set;}
    
    [Export]
    public float base_knockback {get; set;}
    
    [Export]
    public float growth_knockback {get; set;}
    
    [Export]
    public float knockback_angle {get; set;}
    private bool is_special_angle;

    public CollisionData() : this(Vector2.Zero, Vector2.Zero, 0.0f, 0.0f, 0.0f, 0.0f) {}
    public CollisionData(Vector2 p_offset, Vector2 p_size, float p_base_damage, float p_base_knockback, float p_growth_knockback, float p_knockback_angle)
    {
        offset = p_offset;
        size = p_size;
        base_damage = p_base_damage;
        base_knockback = p_base_knockback;
        growth_knockback = p_growth_knockback;
        knockback_angle = p_knockback_angle;
        is_special_angle = knockback_angle >= 360;
    }

    public Vector2 GetKnockbackVector()
    {
        float rad = Mathf.Deg2Rad(knockback_angle);
        return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
    }

    public bool IsSpecialAngle()
    {
        return is_special_angle;
    }
}
