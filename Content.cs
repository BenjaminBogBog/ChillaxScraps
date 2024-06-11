using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx.Configuration;
using LethalLib;
using LethalLib.Extras;
using LethalLib.Modules;
using Unity.Netcode.Components;
using UnityEngine;

namespace Chillax.Bastard.BogBog
{
    public class Content
  {
    public static AssetBundle MainAssets;
    public static Dictionary<string, GameObject> Prefabs = new Dictionary<string, GameObject>();
    public static List<Content.CustomUnlockable> customUnlockables;
    public static List<Content.CustomItem> customItems;
    public static List<Content.CustomEnemy> customEnemies;
    public static List<Content.CustomMapObject> customMapObjects;
    public static GameObject ConfigManagerPrefab;

    public static void TryLoadAssets()
    {
      if (!((UnityEngine.Object) Content.MainAssets == (UnityEngine.Object) null))
        return;
      Content.MainAssets = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "chillaxBundle"));
      Plugin.logger.LogInfo((object) "Loaded asset bundle");
    }

    public static void Load()
    {
      TryLoadAssets();

      Content.customItems = new List<Content.CustomItem>()
      {
        (Content.CustomItem) Content.CustomScrap.Add("Boink", "Assets/Chillax/ChillaxMods/Boink/Boink.asset", Levels.LevelTypes.All, Config.boinkSpawnChance.Value),
        (Content.CustomItem) Content.CustomScrap.Add("MamaMooSup", "Assets/Chillax/ChillaxMods/Cup Noodle/Cup Noodle.asset", Levels.LevelTypes.All, Config.cupNoodleSpawnChance.Value),
        Content.CustomScrap.Add("Freddy", "Assets/Chillax/ChillaxMods/FreddyFastBear/Freddy.asset", Levels.LevelTypes.All, Config.freddySpawnChance.Value),
        Content.CustomScrap.Add("Nokia", "Assets/Chillax/ChillaxMods/Nokia/Nokia.asset", Levels.LevelTypes.All, Config.nokiaSpawnChance.Value),
        Content.CustomScrap.Add("Moai", "Assets/Chillax/ChillaxMods/Moai/Moai.asset", Levels.LevelTypes.All, Config.moaiSpawnChance.Value),
        Content.CustomScrap.Add("Froggy", "Assets/Chillax/ChillaxMods/Froggy Chair/Froggy Chair.asset", Levels.LevelTypes.All, Config.froggySpawnChance.Value),
        Content.CustomScrap.Add("Eevee", "Assets/Chillax/ChillaxMods/Eevee Plush/Eevee.asset", Levels.LevelTypes.All, Config.eeveeSpawnChance.Value),
        Content.CustomScrap.Add("DeathNote", "Assets/Chillax/ChillaxMods/DeathNote/DeathNote.asset", Levels.LevelTypes.All, Config.deathNoteSpawnChance.Value),
        Content.CustomScrap.Add("EmergencyMeeting", "Assets/Chillax/ChillaxMods/EmergencyMeeting/EmergencyMeeting.asset", Levels.LevelTypes.All, Config.emergencyMeetingChance.Value),
        Content.CustomScrap.Add("SuperSneakers", "Assets/Chillax/ChillaxMods/SuperSneakers/SuperSneakers.asset", Levels.LevelTypes.All, Config.superSneakChance.Value),
        Content.CustomScrap.Add("TotemOfUndying", "Assets/Chillax/ChillaxMods/TotemOfUndying/TotemOfUndying.asset", Levels.LevelTypes.All, Config.totemChance.Value),
        Content.CustomScrap.Add("DeathDanceNote", "Assets/Chillax/ChillaxMods/DeathDanceNote/DeathDanceNote.asset", Levels.LevelTypes.All, Config.deathDanceNote.Value)
      };
      Content.customEnemies = new List<Content.CustomEnemy>()
      {
      };
      Content.customUnlockables = new List<Content.CustomUnlockable>()
      {
      };
      Content.customMapObjects = new List<Content.CustomMapObject>()
      {
      };
      foreach (Content.CustomItem customItem in Content.customItems)
      {
        if (customItem.enabled)
        {
          Item obj = Content.MainAssets.LoadAsset<Item>(customItem.itemPath);
          if ((UnityEngine.Object) obj.spawnPrefab.GetComponent<NetworkTransform>() == (UnityEngine.Object) null)
            obj.spawnPrefab.AddComponent<NetworkTransform>();
          Content.Prefabs.Add(customItem.name, obj.spawnPrefab);
          NetworkPrefabs.RegisterNetworkPrefab(obj.spawnPrefab);
          customItem.itemAction(obj);
          if (customItem is Content.CustomShopItem)
          {
            TerminalNode itemInfo = Content.MainAssets.LoadAsset<TerminalNode>(customItem.infoPath);
            Plugin.logger.LogInfo((object) string.Format("Registering shop item {0} with price {1}", (object) customItem.name, (object) ((Content.CustomShopItem) customItem).itemPrice));
            Items.RegisterShopItem(obj, itemInfo: itemInfo, price: ((Content.CustomShopItem) customItem).itemPrice);
          }
          else if (customItem is CustomScrap scrap)
          {
            ChillaxModPlugin.logger.LogInfo($"[CHILLAX] Registering... {obj.itemName} at rarity {scrap.rarity}");
            Items.RegisterScrap(obj, scrap.rarity, scrap.levelType);
          }
           
        }
      }
      foreach (Content.CustomUnlockable customUnlockable in Content.customUnlockables)
      {
        if (customUnlockable.enabled)
        {
          UnlockableItem unlockable = Content.MainAssets.LoadAsset<UnlockableItemDef>(customUnlockable.unlockablePath).unlockable;
          if ((UnityEngine.Object) unlockable.prefabObject != (UnityEngine.Object) null)
            NetworkPrefabs.RegisterNetworkPrefab(unlockable.prefabObject);
          Content.Prefabs.Add(customUnlockable.name, unlockable.prefabObject);
          TerminalNode itemInfo = (TerminalNode) null;
          if (customUnlockable.infoPath != null)
            itemInfo = Content.MainAssets.LoadAsset<TerminalNode>(customUnlockable.infoPath);
          Unlockables.RegisterUnlockable(unlockable, StoreType.Decor, itemInfo: itemInfo, price: customUnlockable.unlockCost);
        }
      }
      foreach (Content.CustomEnemy customEnemy in Content.customEnemies)
      {
        if (customEnemy.enabled)
        {
          EnemyType enemy = Content.MainAssets.LoadAsset<EnemyType>(customEnemy.enemyPath);
          TerminalNode infoNode = Content.MainAssets.LoadAsset<TerminalNode>(customEnemy.infoNode);
          TerminalKeyword infoKeyword = (TerminalKeyword) null;
          if (customEnemy.infoKeyword != null)
            infoKeyword = Content.MainAssets.LoadAsset<TerminalKeyword>(customEnemy.infoKeyword);
          NetworkPrefabs.RegisterNetworkPrefab(enemy.enemyPrefab);
          Content.Prefabs.Add(customEnemy.name, enemy.enemyPrefab);
          Enemies.RegisterEnemy(enemy, customEnemy.rarity, customEnemy.levelFlags, customEnemy.spawnType, infoNode, infoKeyword);
        }
      }
      foreach (Content.CustomMapObject customMapObject in Content.customMapObjects)
      {
        if (customMapObject.enabled)
        {
          SpawnableMapObjectDef mapObject = Content.MainAssets.LoadAsset<SpawnableMapObjectDef>(customMapObject.objectPath);
          NetworkPrefabs.RegisterNetworkPrefab(mapObject.spawnableMapObject.prefabToSpawn);
          Content.Prefabs.Add(customMapObject.name, mapObject.spawnableMapObject.prefabToSpawn);
          MapObjects.RegisterMapObject(mapObject, customMapObject.levelFlags, customMapObject.spawnRateFunction);
        }
      }
      foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
      {
        foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static))
        {
          if (method.GetCustomAttributes(typeof (RuntimeInitializeOnLoadMethodAttribute), false).Length != 0)
            method.Invoke((object) null, (object[]) null);
        }
      }
      
      
      Plugin.logger.LogInfo((object) "[CHILLAX] Custom content loaded!");
    }


    public class CustomItem
    {
      public string name = "";
      public string itemPath = "";
      public string infoPath = "";
      public Action<Item> itemAction = (Action<Item>) (item => { });
      public bool enabled = true;

      public CustomItem(string name, string itemPath, string infoPath, Action<Item> action = null)
      {
        this.name = name;
        this.itemPath = itemPath;
        this.infoPath = infoPath;
        if (action == null)
          return;
        this.itemAction = action;
      }

      public static Content.CustomItem Add(
        string name,
        string itemPath,
        string infoPath = null,
        Action<Item> action = null)
      {
        return new Content.CustomItem(name, itemPath, infoPath, action);
      }
    }

    public class CustomUnlockable
    {
      public string name = "";
      public string unlockablePath = "";
      public string infoPath = "";
      public Action<UnlockableItem> unlockableAction = (Action<UnlockableItem>) (item => { });
      public bool enabled = true;
      public int unlockCost = -1;

      public CustomUnlockable(
        string name,
        string unlockablePath,
        string infoPath,
        Action<UnlockableItem> action = null,
        int unlockCost = -1)
      {
        this.name = name;
        this.unlockablePath = unlockablePath;
        this.infoPath = infoPath;
        if (action != null)
          this.unlockableAction = action;
        this.unlockCost = unlockCost;
      }

      public static Content.CustomUnlockable Add(
        string name,
        string unlockablePath,
        string infoPath = null,
        Action<UnlockableItem> action = null,
        int unlockCost = -1,
        bool enabled = true)
      {
        return new Content.CustomUnlockable(name, unlockablePath, infoPath, action, unlockCost)
        {
          enabled = enabled
        };
      }
    }

    public class CustomShopItem : Content.CustomItem
    {
      public int itemPrice;

      public CustomShopItem(
        string name,
        string itemPath,
        string infoPath = null,
        int itemPrice = 0,
        Action<Item> action = null)
        : base(name, itemPath, infoPath, action)
      {
        this.itemPrice = itemPrice;
      }

      public static Content.CustomShopItem Add(
        string name,
        string itemPath,
        string infoPath = null,
        int itemPrice = 0,
        Action<Item> action = null,
        bool enabled = true)
      {
        Content.CustomShopItem customShopItem = new Content.CustomShopItem(name, itemPath, infoPath, itemPrice, action);
        customShopItem.enabled = enabled;
        return customShopItem;
      }
    }

    public class CustomScrap : Content.CustomItem
    {
      public Levels.LevelTypes levelType = Levels.LevelTypes.All;
      public int rarity;

      public CustomScrap(
        string name,
        string itemPath,
        Levels.LevelTypes levelType,
        int rarity,
        Action<Item> action = null)
        : base(name, itemPath, (string) null, action)
      {
        this.levelType = levelType;
        this.rarity = rarity;
      }

      public static Content.CustomScrap Add(
        string name,
        string itemPath,
        Levels.LevelTypes levelType,
        int rarity,
        Action<Item> action = null)
      {
        return new Content.CustomScrap(name, itemPath, levelType, rarity, action);
      }
    }

    public class CustomEnemy
    {
      public string name;
      public string enemyPath;
      public int rarity;
      public Levels.LevelTypes levelFlags;
      public Enemies.SpawnType spawnType;
      public string infoKeyword;
      public string infoNode;
      public bool enabled = true;

      public CustomEnemy(
        string name,
        string enemyPath,
        int rarity,
        Levels.LevelTypes levelFlags,
        Enemies.SpawnType spawnType,
        string infoKeyword,
        string infoNode)
      {
        this.name = name;
        this.enemyPath = enemyPath;
        this.rarity = rarity;
        this.levelFlags = levelFlags;
        this.spawnType = spawnType;
        this.infoKeyword = infoKeyword;
        this.infoNode = infoNode;
      }

      public static Content.CustomEnemy Add(
        string name,
        string enemyPath,
        int rarity,
        Levels.LevelTypes levelFlags,
        Enemies.SpawnType spawnType,
        string infoKeyword,
        string infoNode,
        bool enabled = true)
      {
        return new Content.CustomEnemy(name, enemyPath, rarity, levelFlags, spawnType, infoKeyword, infoNode)
        {
          enabled = enabled
        };
      }
    }

    public class CustomMapObject
    {
      public string name;
      public string objectPath;
      public Levels.LevelTypes levelFlags;
      public Func<SelectableLevel, AnimationCurve> spawnRateFunction;
      public bool enabled = true;

      public CustomMapObject(
        string name,
        string objectPath,
        Levels.LevelTypes levelFlags,
        Func<SelectableLevel, AnimationCurve> spawnRateFunction = null,
        bool enabled = false)
      {
        this.name = name;
        this.objectPath = objectPath;
        this.levelFlags = levelFlags;
        this.spawnRateFunction = spawnRateFunction;
        this.enabled = enabled;
      }

      public static Content.CustomMapObject Add(
        string name,
        string objectPath,
        Levels.LevelTypes levelFlags,
        Func<SelectableLevel, AnimationCurve> spawnRateFunction = null,
        bool enabled = false)
      {
        return new Content.CustomMapObject(name, objectPath, levelFlags, spawnRateFunction, enabled);
      }
    }
  }
}