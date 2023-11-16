using Godot;
using System;

public partial class Moveset : Resource
{
    [Export]
    public HitboxSequence Jab1{get; set;}

    [Export]
    public HitboxSequence Jab2{get; set;}

    [Export]
    public HitboxSequence Jab3{get; set;}

    [Export]
    public HitboxSequence RapidJab{get; set;}

    [Export]
    public HitboxSequence DashAttack{get; set;}

    [Export]
    public HitboxSequence ForwardTilt{get; set;}

    [Export]
    public HitboxSequence UpTilt{get; set;}

    [Export]
    public HitboxSequence DownTilt{get; set;}

    [Export]
    public HitboxSequence ForwardSmash{get; set;}

    [Export]
    public HitboxSequence UpSmash{get; set;}

    [Export]
    public HitboxSequence DownSmash{get; set;}

    [Export]
    public HitboxSequence NeutralAerial{get; set;}

    [Export]
    public HitboxSequence ForwardAerial{get; set;}

    [Export]
    public HitboxSequence BackAerial{get; set;}

    [Export]
    public HitboxSequence UpAerial{get; set;}

    [Export]
    public HitboxSequence DownAerial{get; set;}

    [Export]
    public HitboxSequence NeutralSpecial{get; set;}

    [Export]
    public HitboxSequence ForwardSpecial{get; set;}

    [Export]
    public HitboxSequence UpSpecial{get; set;}

    [Export]
    public HitboxSequence DownSpecial{get; set;}

    public Moveset() : this(null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null){}
    public Moveset(HitboxSequence p_jab1, HitboxSequence p_jab2, HitboxSequence p_jab3, HitboxSequence p_rapidjab, HitboxSequence p_dashattack,
    HitboxSequence p_forwardtilt, HitboxSequence p_uptilt, HitboxSequence p_downtilt, HitboxSequence p_forwardsmash, HitboxSequence p_upsmash,
    HitboxSequence p_downsmash, HitboxSequence p_neutralair, HitboxSequence p_forwardair, HitboxSequence p_backair, HitboxSequence p_upair, 
    HitboxSequence p_downair, HitboxSequence p_neutralspecial, HitboxSequence p_forwardspecial, HitboxSequence p_upspecial, HitboxSequence p_downspecial)
    {
        Jab1 = p_jab1;
        Jab2 = p_jab2;
        Jab3 = p_jab3;
        RapidJab = p_rapidjab;
        DashAttack = p_dashattack;
        ForwardTilt = p_forwardtilt;
        UpTilt = p_uptilt;
        DownTilt = p_downtilt;
        ForwardSmash = p_forwardsmash;
        UpSmash = p_upsmash;
        DownSmash = p_downsmash;
        NeutralAerial = p_neutralair;
        ForwardAerial = p_forwardair;
        BackAerial = p_backair;
        UpAerial = p_upair;
        DownAerial = p_downair;
        NeutralSpecial = p_neutralspecial;
        ForwardSpecial = p_forwardspecial;
        UpSpecial = p_upspecial;
        DownSpecial = p_downspecial;
    }
}
