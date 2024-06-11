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
        public static ConfigEntry<int> nokiaSpawnChance;
        public static ConfigEntry<int> moaiSpawnChance;
        public static ConfigEntry<int> froggySpawnChance;
        public static ConfigEntry<int> eeveeSpawnChance;
        public static ConfigEntry<int> deathNoteSpawnChance;
        public static ConfigEntry<int> emergencyMeetingChance;
        public static ConfigEntry<bool> canUseDeathNote;
        public static ConfigEntry<int> superSneakChance;
        public static ConfigEntry<int> totemChance;
        public static ConfigEntry<int> deathDanceNote;
        
        public static void Load()
        {
            boinkSpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "Boink", 15, "How much does this Item spawn, higher = more common");
            cupNoodleSpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "CupNoodle", 15, "How much does this Item spawn, higher = more common");
            freddySpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "Freddy", 10, "How much does this Item spawn, higher = more common");
            nokiaSpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "Nokia", 10, "How much does this Item spawn, higher = more common");
            moaiSpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "Moai", 10, "How much does this Item spawn, higher = more common");
            froggySpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "Froggy", 15, "How much does this Item spawn, higher = more common");
            eeveeSpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "Eevee", 10, "How much does this Item spawn, higher = more common");
            deathNoteSpawnChance = ChillaxModPlugin.config.Bind<int>("Scrap", "DeathNote", 5, "How much does this Item spawn, higher = more common");
            canUseDeathNote = ChillaxModPlugin.config.Bind<bool>("Death Note", "DeathNoteActive", true, "Can the Death Note be used, turn this off to avoid griefing");
            emergencyMeetingChance = ChillaxModPlugin.config.Bind<int>("Scrap", "EmergencyMeeting", 10, "How much does this Item spawn, higher = more common");
            superSneakChance = ChillaxModPlugin.config.Bind<int>("Scrap", "SuperSneakers", 15, "How much does this Item spawn, higher = more common");
            totemChance = ChillaxModPlugin.config.Bind<int>("Scrap", "TotemOfUndying", 5, "How much does this Item spawn, higher = more common");
            deathDanceNote = ChillaxModPlugin.config.Bind<int>("Scrap", "DeathDanceNote", 5, "How much does this Item spawn, higher = more common");
        }
    }
}