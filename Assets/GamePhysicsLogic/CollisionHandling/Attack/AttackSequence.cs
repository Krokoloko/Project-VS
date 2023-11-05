
public struct AttackSequence 
{
    //Sequence of hitboxes per frame based of the frame in the animation
    int startup_frame;
    int enter_cooldown_frame;
    Hitbox[] hitbox_sequence;
}