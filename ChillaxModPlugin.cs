using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace Chillax.Bastard.BogBog
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("evaisa.lethallib")]
    [BepInDependency("evaisa.lethalthings")]
    public class ChillaxModPlugin : BaseUnityPlugin
    {
        public static ManualLogSource logger;
        //"pluginGuid" - is a Globally Unique ID, that your mod will have.
        //"pluginName" - is your mod's name, that will be displayed in console.
        //"pluginVersion" - is your mod's version. It must be in format 1.2[.3[.4]], that can be parsed by System.Version.
        public const string PluginGuid = "chillax.bastard.mod";
        public const string PluginName = "Chillax Mods";
        public const string PluginVersion = "0.8.1";

        public static readonly Harmony harmony = new Harmony("chillax.bastard.mod");

        public static ConfigFile config;
        public void Awake()
        {
            logger = Logger;
            config = Config;
            
            BogBog.Config.Load();
            Content.Load();

            harmony.PatchAll(typeof(ChillaxModPlugin));
            
            Logger.LogInfo("CHILLAX Plugin Loaded");
        }
    }
}