using Godot;
using System;

public enum EHITBOXTYPE
{
    NONE,
    NORMAL,
    MULTIHIT

}
public struct Hitbox
{
   public Vector2 size;
   public Vector2 offset;
   public float base_damage;
   public float base_knockback;
   public float growth_knockback;
   public Vector2 knockback_angle;
   public int active_frames;
   public Node relative_node;
}