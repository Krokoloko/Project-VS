using Godot;
using System;
using System.Linq;
using System.Linq.Expressions;

public class PlayerInfo : Node
{
    [Export]
    public NodePath character_name_node;
    private Label character_name_label;

    [Export]
    public NodePath damage_node;
    private Node damage_output;

    [Export]
    public NodePath player_portrait_node;
    private TextureRect player_portrait;

    [Export]
    public NodePath player_id_label_node;
    private Label player_id_label;

    [Export]
    private NodePath stock_display_node;
    private Node stock_display;

    private Texture stock_icon;
    private Vector2 icon_scale;
    
    [Export]
    public PLAYER_CURSOR player_id;
    private Player player;

    [Export]
    public NodePath player_container;

    [Export]
    public CharacterNumberRelation sprite_number_relation;

    public override void _Ready()
    {
        character_name_label = GetNode<Label>(character_name_node);
        player_portrait = GetNode<TextureRect>(player_portrait_node);
        damage_output = GetNode<Node>(damage_node);
        stock_display = GetNode<Node>(stock_display_node);
        player_id_label = GetNode<Label>(player_id_label_node);

        var container = GetNode<Node>(player_container);
        for(int i = 0; i < container.GetChildCount(); i++)
        {
            Player p = container.GetChildOrNull<Player>(i);
            if(p != null)
            {
                if(p.GetPlayerID() == player_id)
                {
                    player = p;
                    player_id_label.Text = "P" + ((int)player_id+1).ToString() + " \\";
                }
            } 
        }

        if(player == null)
        {
            this.QueueFree();
        }else
        {
            stock_icon = player.GetStockIcon();
            icon_scale = player.GetStockScale();
            player.Connect("OnPlayerDamaged", this, "UpdateUI");
            player.Connect("OnPlayerDie", this, "UpdateUI");

            character_name_label.Text = player.GetPlayerName();
            for(int i = 0; i < stock_display.GetChildCount(); i++)
            {
                var stock = stock_display.GetChildOrNull<TextureRect>(i);
                if(stock != null)
                {
                    stock.Texture = stock_icon;
                    stock.RectScale = icon_scale;
                }
            }
            UpdateUI();
            GetNode<TextureRect>("damage display/digit_1").RectScale = sprite_number_relation.scaling;
            GetNode<TextureRect>("damage display/digit_2").RectScale = sprite_number_relation.scaling;
            GetNode<TextureRect>("damage display/digit_3").RectScale = sprite_number_relation.scaling;
        }
    }

    private void UpdateUI()
    {   
        var damage = player.GetPercentage();
        var stock_count = player.GetStockCount();
        
        for(int i = 0; i < stock_display.GetChildCount(); i++)
        {
            var child = stock_display.GetChild<TextureRect>(i);
            if(i < stock_count)
            {
                child.Texture = stock_icon;
            }else
            {
                child.Texture = null;
            }
        }
        
        string damage_string = damage.ToString();
        string final_damage = damage_string;
        if(damage_string.Find('.') >= 0)
        {
            final_damage = damage_string.Substring(0,damage_string.Find('.'));
        }
        
        TextureRect digit_1 = GetNode<TextureRect>("damage display/digit_1");
        TextureRect digit_2 = GetNode<TextureRect>("damage display/digit_2");
        TextureRect digit_3 = GetNode<TextureRect>("damage display/digit_3");
        
        if(final_damage.Length() >= 3)
        {
            string digit_1_string = "" + final_damage[0];
            string digit_2_string = "" + final_damage[1];
            string digit_3_string = "" + final_damage[2];
        
            int index_1 = Array.IndexOf(sprite_number_relation.symbols, digit_1_string);
            int index_2 = Array.IndexOf(sprite_number_relation.symbols, digit_2_string);
            int index_3 = Array.IndexOf(sprite_number_relation.symbols, digit_3_string);
        
        
            digit_1.Texture = sprite_number_relation.textures[index_1];
            digit_2.Texture = sprite_number_relation.textures[index_2];
            digit_3.Texture = sprite_number_relation.textures[index_3]; 
        }        
        else if(final_damage.Length() >= 2)
        {
            string digit_1_string = "0";
            string digit_2_string = "" + final_damage[0];
            string digit_3_string = "" + final_damage[1];
        
        
            int index_1 = Array.IndexOf(sprite_number_relation.symbols, digit_1_string);
            int index_2 = Array.IndexOf(sprite_number_relation.symbols, digit_2_string);
            int index_3 = Array.IndexOf(sprite_number_relation.symbols, digit_3_string);        
        
            digit_1.Texture = sprite_number_relation.textures[index_1];
            digit_2.Texture = sprite_number_relation.textures[index_2];
            digit_3.Texture = sprite_number_relation.textures[index_3]; 
        }
        else if(final_damage.Length() >= 1)
        {
            string digit_1_string = "0";
            string digit_2_string = "0";
            string digit_3_string = "" + final_damage[0];        
        
            int index_1 = Array.IndexOf(sprite_number_relation.symbols, digit_1_string);
            int index_2 = Array.IndexOf(sprite_number_relation.symbols, digit_2_string);
            int index_3 = Array.IndexOf(sprite_number_relation.symbols, digit_3_string);
        
            digit_1.Texture = sprite_number_relation.textures[index_1];
            digit_2.Texture = sprite_number_relation.textures[index_2];
            digit_3.Texture = sprite_number_relation.textures[index_3]; 
        }
    }
}
