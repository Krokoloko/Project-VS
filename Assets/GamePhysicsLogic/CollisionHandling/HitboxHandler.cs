using Godot;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

public class HitboxHandler : Node
{
    public bool HITBOX_VISUALISER = true;
    private Dictionary<int, HitboxSequence> sequences;
    private Dictionary<int, NodePath> nodepaths;
    private Dictionary<int, PLAYER_CURSOR> collision_ids;
    private Dictionary<int, float> timers;
    private Dictionary<int, int> frame_counters;
    private Dictionary<int, int> previous_frame_counters;
    private Dictionary<int, int> frame_tresholds; 
    private Dictionary<int, float> intervals;
    private Dictionary<int, bool> reverse_direction;
    private HitboxProps[] hitboxes;

    private int global_index_counter;

    private PackedScene hitbox_node;
    private Texture hitbox_debug_sprite;
    private List<int> flagged_for_deletion;

    public override void _Ready()
    {
        hitbox_debug_sprite = GD.Load<Texture>("res://Assets/DebugAssets/Hitbox.png");
        hitbox_node = GD.Load<PackedScene>("res://Assets/GamePhysicsLogic/HitBoxData/Hitbox.tscn");
        global_index_counter = 0;
        hitboxes = new HitboxProps[0];
        sequences = new Dictionary<int, HitboxSequence>();
        nodepaths = new Dictionary<int, NodePath>();
        collision_ids = new Dictionary<int, PLAYER_CURSOR>();
        timers = new Dictionary<int, float>();
        previous_frame_counters = new Dictionary<int, int>();
        frame_counters = new Dictionary<int, int>();
        frame_tresholds = new Dictionary<int, int>();
        flagged_for_deletion = new List<int>();
        intervals = new Dictionary<int, float>();
        reverse_direction = new Dictionary<int, bool>();
    }

    public override void _Process(float delta)
    {
        if(Input.IsActionJustReleased("Debug_Hitboxes"))
        {
            HITBOX_VISUALISER = !HITBOX_VISUALISER;
        }
        ProcessSequences(delta);
    }

    private void ProcessSequences(float delta)
    {
        for (int i = 0; i < sequences.Count; i++)
        {
            foreach(var timer in timers.ToArray())
            {
                timers[timer.Key] += delta;
            }
            foreach(var frame_counter in frame_counters.ToArray())
            {
                frame_counters[frame_counter.Key] = (int)(timers[frame_counter.Key]/intervals[frame_counter.Key]);
            }
            foreach(var sequence in sequences.ToArray())
            {
                int key = sequence.Key;
                int frame = frame_counters[key];
                if(frame >= sequences[key].sequence.Length) 
                {
                    DeleteSequence(sequence.Key);
                    continue;
                }
                HitboxData hitboxData = null;
                if(frame >= 0)
                {
                    hitboxData = sequences[key].sequence[frame];
                }
                if(hitboxData == null) continue;
                if(frame > previous_frame_counters[key])
                {
                    for (int j = 0; j < hitboxData.composition.Length; j++)
                    {
                        CollisionData collider = hitboxData.composition[j];
                        if(collider == null) continue;
                        Vector3 player_position = GetNode<Spatial>(nodepaths[key]).Transform.origin;
                        Color color = new Color(1.0f,0,0);
                        float flip = GetNode<Player>(nodepaths[key]).anim_controller.GetFlip()?-1:1;
                        
                        Hitbox hitbox = hitbox_node.Instance<Hitbox>();
                        hitbox.hitboxData = collider;
                        hitbox.frames = hitboxData.frames;
                        hitbox.time_per_frame = intervals[key];
                        hitbox.Translation = new Vector3(collider.offset.x*flip,collider.offset.y,0);
                        hitbox.reverse = reverse_direction[key];
                        Area physicsbody = hitbox.GetChild<Area>(0);
                        physicsbody.SetCollisionMaskBit(1 + (int)PLAYER_CURSOR.P1, PLAYER_CURSOR.P1 != collision_ids[key]);
                        physicsbody.SetCollisionMaskBit(1 + (int)PLAYER_CURSOR.P2, PLAYER_CURSOR.P2 != collision_ids[key]);
                        physicsbody.SetCollisionMaskBit(1 + (int)PLAYER_CURSOR.P3, PLAYER_CURSOR.P3 != collision_ids[key]);
                        physicsbody.SetCollisionMaskBit(1 + (int)PLAYER_CURSOR.P4, PLAYER_CURSOR.P4 != collision_ids[key]);

                        //Non player specific bitmask
                        physicsbody.SetCollisionMaskBit(6, true);

                        CollisionShape hitbox_collider = physicsbody.GetChild<CollisionShape>(0);
                        hitbox_collider.Scale = new Vector3(collider.size.x, collider.size.y, 1.0f);
                        if(HITBOX_VISUALISER)
                        {
                            Sprite3D sprite = new Sprite3D();
                            sprite.Translation = new Vector3(sprite.Translation.x, sprite.Translation.y, 1f);
                            sprite.Scale = hitbox_collider.Scale * 3;
                            sprite.Texture = hitbox_debug_sprite;
                            hitbox.AddChild(sprite);
                        }
                        GetNode<Node>(nodepaths[key]).AddChild(hitbox);
                        previous_frame_counters[key] = frame;
                        frame_counters[key] = frame - hitboxData.frames+1;
                        timers[key] -= (hitboxData.frames-1)*intervals[key];
                    }
                }
            }
        }

    }

    public bool SequenceIsFinished(int handle)
    {
        if(!sequences.ContainsKey(handle)) return true;
        return frame_counters[handle] >= sequences[handle].total_frames;        
    }
    public void ShouldDeleteSequences()
    {
        if(flagged_for_deletion.Count > 0)
        {
            //flagged_for_deletion.Sort(,);
            for(int i = 0; i < flagged_for_deletion.Count; i++)
            {
                DeleteSequence(flagged_for_deletion[i]);
            }
        }
    }

    public void DeleteSequence(int handle)
    {
        if(!sequences.ContainsKey(handle)) return;
        sequences.Remove(handle);
        nodepaths.Remove(handle);
        timers.Remove(handle);
        collision_ids.Remove(handle);
        frame_counters.Remove(handle);
        previous_frame_counters.Remove(handle);
        frame_tresholds.Remove(handle);
        intervals.Remove(handle);
        reverse_direction.Remove(handle);
    }

    public int AddSequence(HitboxSequence sequence, float speed, NodePath node, PLAYER_CURSOR id, bool reverse)
    {
        int index = global_index_counter;
        sequences.Add(index, sequence);
        nodepaths.Add(index, node);
        collision_ids.Add(index, id);
        timers.Add(index, 0.0f);
        frame_counters.Add(index, 0);
        previous_frame_counters.Add(index, -1);
        frame_tresholds.Add(index, sequence.sequence[0].frames);
        intervals.Add(index,speed);
        reverse_direction.Add(index, reverse);
        global_index_counter++;
        return global_index_counter;
    }

    private void AddHitboxes(HitboxData data, NodePath relative_to)
    {
        foreach(CollisionData collisions in data.composition)
        {
            HitboxProps hitbox = new HitboxProps();
            hitbox.active_frames = data.frames;
            hitbox.base_damage = collisions.base_damage;
            hitbox.growth_knockback = collisions.growth_knockback;
            hitbox.knockback_angle = collisions.GetKnockbackVector();
            hitbox.offset = collisions.offset;
            hitbox.size = collisions.size;
            hitbox.relative_node = GetNode(relative_to);
            hitboxes.Append(hitbox);
        }
    }
}
