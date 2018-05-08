using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGame : MonoBehaviour {

    private static readonly string SaveFile = "save.bin";

    static readonly string SaveGameNames = "saveGameNames";
    static readonly char delimiter = ',';


    static readonly string position = "pos";
    static readonly string rotation = "rot";
    static readonly string experience = "xp";
    static readonly string health = "hp";
    static readonly string mana = "mana";
    static readonly string strength = "str";
    static readonly string constitution = "const";
    static readonly string intelligence = "intel";
    static readonly string dexterity = "dex";
    static readonly string playerType = "plType";
    static readonly string attackType = "atckType";
    static readonly string sword = "swd";

    static void EnsureInit()
    {
        SaveSystem.Initialize(SaveFile);
    }
   

    public static void Save(string profileName)
    {
        EnsureInit();
        var player = PlayerManager.Player;
        SaveSystem.SetVector3(profileName + position, player.transform.position);
        SaveSystem.SetVector3(profileName + rotation, player.transform.rotation.eulerAngles);
        SaveSystem.SetInt(profileName + experience, PlayerManager.Experience);

        PlayerStats ps = PlayerManager.PlayerStats;
        SaveSystem.SetInt(profileName + health, ps.CurrentHealth);
        SaveSystem.SetInt(profileName + mana, ps.CurrentMana);
        SaveSystem.SetInt(profileName + strength, ps.strength.Value);
        SaveSystem.SetInt(profileName + constitution, ps.constitution.Value);
        SaveSystem.SetInt(profileName + intelligence, ps.intelligence.Value);
        SaveSystem.SetInt(profileName + dexterity, ps.dexterity.Value);

        SaveSystem.SetInt(profileName + playerType, (int)PlayerManager.PlayerScript.Type);
        SaveSystem.SetInt(profileName + attackType, PlayerManager.PlayerScript.AttackIndex);
        SaveSystem.SetInt(profileName + sword, PlayerManager.PlayerScript.defaultSwordIndex);

    }

    public static void Load(string profileName)
    {
        EnsureInit();

    }
    public static string[] GetSaveGameNames()
    {
        EnsureInit();
        var val = SaveSystem.GetString(SaveGameNames);
        if (string.IsNullOrEmpty(val)) return null;
        return val.Split(delimiter);
    } 
}
