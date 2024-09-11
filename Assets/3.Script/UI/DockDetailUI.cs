using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DockDetailUI : MonoBehaviour
{
    public Image Character_Image;
    public Text Character_Name;
    public Text Character_ShipName;
    public Text Character_ShipType;
    
    public void CharacterDetail(Character character)
    {
        Character_Image.sprite = Resources.Load<Sprite>($"CharacterImages/{character.imageName}");
        Character_Name.text = character.name;
        //Character_ShipName.text = character.shipname;
        Character_ShipType.text = character.shipType;
    }

}
