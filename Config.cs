using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Logger = UnityEngine.Logger;

namespace Chillax.Bastard.BogBog
{
    public class Config
    {
        public static ConfigEntry<int> boinkSpawnChance;
        public static ConfigEntry<int> cupNoodleSpawnChance;
        public static ConfigEntry<int> freddySpawnChance;
        public static ConfigEntry<int> moaiSpawnChance;
        public static ConfigEntry<int> froggySpawnChance;
        public static ConfigEntry<int> eeveeSpawnChance;
        public static ConfigEntry<int> deathNoteSpawnChance;
        public static ConfigEntry<int> emergencyMeetingChance;
        public static ConfigEntry<bool> canUseDeathNote;
        public static ConfigEntry<int> superSneakChance;
        public static ConfigEntry<int> totemChance;
        public static ConfigEntry<int> deathDanceNote;
        public static ConfigEntry<int> masterSword;
        public static ConfigEntry<int> ocarinaSpawn;

        public static ConfigEntry<int> unoReverseCardSpawnChance;
        public static ConfigEntry<bool> enableUnoReverseCard;

        public static ConfigEntry<int> boinkCooldown;

        // Duckies
        public static ConfigEntry<int> blueDuckSpawnChance;
        public static ConfigEntry<int> redDuckSpawnChance;
        public static ConfigEntry<int> goldDuckSpawnChance;

        public static void Load()
        {
            boinkSpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "Boink", 15, "How much does this Item spawn, higher = more common");
            boinkCooldown = ChillaxModPlugin.config.Bind<int>("Scrap", "BoinkCooldown", 5, "Boink's cooldown in seconds");

            cupNoodleSpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "CupNoodle", 15, "How much does this Item spawn, higher = more common");
            //freddySpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "Freddy", 10, "How much does this Item spawn, higher = more common");
            moaiSpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "Moai", 10, "How much does this Item spawn, higher = more common");
            froggySpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "Froggy", 15, "How much does this Item spawn, higher = more common");
            eeveeSpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "Eevee", 10, "How much does this Item spawn, higher = more common");

            deathNoteSpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "DeathNote", 5, "How much does this Item spawn, higher = more common");
            canUseDeathNote = ChillaxModPlugin.config.Bind<bool>("Death Note", "DeathNoteActive", true, "Can the Death Note be used, turn this off to avoid griefing");

            emergencyMeetingChance = ChillaxModPlugin.config.Bind<int>("Scrap", "EmergencyMeeting", 10, "How much does this Item spawn, higher = more common");

            superSneakChance = ChillaxModPlugin.config.Bind<int>("Scrap", "SuperSneakers", 15, "How much does this Item spawn, higher = more common");
            //totemChance = ChillaxModPlugin.config.Bind<int>("Scrap", "TotemOfUndying", 5, "How much does this Item spawn, higher = more common");
            deathDanceNote = ChillaxModPlugin.config.Bind<int>("Scrap", "DeathDanceNote", 5, "How much does this Item spawn, higher = more common");

            masterSword = ChillaxModPlugin.config.Bind<int>("Scrap", "MasterSword", 5, "How much does this Item spawn, higher = more common");
            ocarinaSpawn = ChillaxModPlugin.config.Bind<int>("Scrap", "Ocarina", 5, "How much does this Item spawn, higher = more common");

            unoReverseCardSpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "UnoReverseCard", 5, "How much does this Item spawn, higher = more common");
            enableUnoReverseCard = ChillaxModPlugin.config.Bind<bool>("UnoReverseCard", "UnoReverseCardActive", true, "Enable Uno Reverse Card (Can be jank, and have bugs)");

            blueDuckSpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "BlueDuck", 5, "How much does this Item spawn, higher = more common");
            redDuckSpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "RedDuck", 5, "How much does this Item spawn, higher = more common");
            goldDuckSpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "GoldDuck", 5, "How much does this Item spawn, higher = more common");
        }
    }
}