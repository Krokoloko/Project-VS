using Godot;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

public class HitboxHandler : Node
{
    public const float INTERVAL_PER_TICK = 1.0f/5.0f;

    private Dictionary<int, HitboxSequence> sequences;
    private Dictionary<int, NodePath> nodepaths;
    private Dictionary<int, float> timers;
    private Dictionary<int, int> frame_counters;
    private Dictionary<int, int> frame_tresholds; 
    private HitboxProps[] hitboxes;

    private int global_index_counter;

    private PackedScene hitbox_node;

    public override void _Ready()
    {
        hitbox_node = GD.Load<PackedScene>("res://Assets/GamePhysicsLogic/HitBoxData/Hitbox.tscn");
        global_index_counter = 0;
        sequences = new Dictionary<int, HitboxSequence>();
        nodepaths = new Dictionary<int, NodePath>();
        timers = new Dictionary<int, float>();
        frame_counters = new Dictionary<int, int>();
        frame_tresholds = new Dictionary<int, int>();
    }

    public override void _Process(float delta)
    {
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
                frame_counters[frame_counter.Key] = (int)(timers[frame_counter.Key]/INTERVAL_PER_TICK);
            }
            foreach(var sequence in sequences.ToArray())
            {
                int key = sequence.Key;
                int frame = frame_counters[key];
                if(frame >= sequences[key].sequence.Length) continue;
                HitboxData hitboxData = sequences[key].sequence[frame];
                if(hitboxData == null) continue;
                for (int j = 0; j < hitboxData.composition.Length; j++)
                {
                    CollisionData collider = hitboxData.composition[j];
                    if(collider == null) continue;
                    Vector3 player_position = GetNode<Spatial>(nodepaths[key]).Transform.origin;
                    Color color = new Color(1.0f,0,0);
                    float flip = GetNode<Player>(nodepaths[key]).anim_controller.GetFlip()?-1:1;
                    
                    Hitbox hitbox = hitbox_node.Instance<Hitbox>();
                    hitbox.frames = 1;
                    hitbox.Translation = new Vector3(collider.offset.x*flip,collider.offset.y,0);
                    StaticBody physicsbody = hitbox.GetChild<StaticBody>(0);
                    CollisionShape hitbox_collider = physicsbody.GetChild<CollisionShape>(0);
                    hitbox_collider.Scale = new Vector3(collider.size.x, collider.size.y, 1.0f);

                    GetNode<Node>(nodepaths[key]).AddChild(hitbox);
                }
            }
        }
    }

    public bool SequenceIsFinished(int handle)
    {
        if(!sequences.ContainsKey(handle)) return true;
        return frame_counters[handle] >= sequences[handle].total_frames;        
    }

    public void DeleteSequence(int handle)
    {
        if(!sequences.ContainsKey(handle)) return;
        sequences.Remove(handle);
        nodepaths.Remove(handle);
        timers.Remove(handle);
        frame_counters.Remove(handle);
        frame_tresholds.Remove(handle);
    }

    public int AddSequence(HitboxSequence sequence, NodePath node)
    {
        int index = global_index_counter;
        sequences.Add(index, sequence);
        nodepaths.Add(index, node);
        timers.Add(index, 0.0f);
        frame_counters.Add(index, 0);
        frame_tresholds.Add(index, sequence.sequence[0].frames);
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
