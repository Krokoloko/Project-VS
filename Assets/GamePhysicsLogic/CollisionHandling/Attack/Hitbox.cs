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
    Vector2 size;
    Vector2 offset;
    float damage;
    float knockback;
    Vector2 knockback_angle;
    int active_frames;

}