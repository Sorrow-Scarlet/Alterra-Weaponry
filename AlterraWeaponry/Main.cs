﻿namespace VELD.AlterraWeaponry;

[BepInPlugin(modGUID, modName, modVers)]
public class Main : BaseUnityPlugin
{
    // MOD INFO
    private const string modName = "Alterra Weaponry";
    private const string modGUID = "com.VELD.AlterraWeaponry";
    private const string modVers = "1.0.5";

    // BepInEx/Harmony/Unity
    private static readonly Harmony harmony = new(modGUID);
    public static ManualLogSource logger;

    // STORY GOALS
    internal static StoryGoal AWPresentationGoal = new("Log_PDA_Goal_AWPresentation", Story.GoalType.PDA, 8f) { playInCreative = true, playInCinematics = false, delay = 8f };

    public static readonly AssetBundle assets = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "alterraweaponry.assets"));
    internal static Options Options { get; } = OptionsPanelHandler.RegisterModOptions<Options>();

    private void Awake()
    {
        logger = Logger;
        logger.LogInfo($"{modName} {modVers} started patching.");
        harmony.PatchAll();
        logger.LogInfo($"{modName} {modVers} harmony patched.");
        LanguagesHandler.LanguagePatch();
        logger.LogInfo($"{modName} {modVers} languages lines patched.");
        PatchPDAEncyEntries();
        logger.LogInfo($"{modName} {modVers} PDA encyclopedia entries registered.");
        PatchPDALogs();
        logger.LogInfo($"{modName} {modVers} PDA logs registered.");

        Coal coal = new();
        coal.Patch();

        BlackPowder blackPowder = new();
        blackPowder.Patch();

        ExplosiveTorpedo explosiveTorpedo = new();
        explosiveTorpedo.Patch();

        PrawnSelfDefenseModule prawnSelfDefenseModule = new();
        prawnSelfDefenseModule.Patch();

        logger.LogInfo($"{modName} {modVers} items registered.");
    }

    private void Update()
    {
        if(UnityInput.Current.GetKeyDown(KeyCode.P))
        {
            logger.LogInfo("Should play audio.");
            GameObject cameraObject = Camera.main.gameObject;
            AudioSource audioSource = cameraObject.GetComponent<AudioSource>();
            audioSource.clip = Main.assets.LoadAsset<AudioClip>("AudioClip.PWAPresentation");
            audioSource.Play();
            logger.LogInfo("Should have played an audio.");
        }
    }

    private static void PatchPDALogs()
    {
        // Load audio clips
        logger.LogInfo($"{modName} {modVers} Loading audio clips...");
        AudioClip AWPresentationAudioClip = GameObject.Instantiate<AudioClip>(assets.LoadAsset<AudioClip>("AudioClip.PWAPresentation"));
        AudioClip AWFirstLethalAudioClip = GameObject.Instantiate<AudioClip>(assets.LoadAsset<AudioClip>("AudioClip.FirstLethalMessage"));
        logger.LogInfo($"{modName} {modVers} Audio clips loaded!");

        logger.LogInfo($"{modName} {modVers} Registering PDA Logs...");

        // Presentation PDA log "Hello xenoworker 91802..."
        CustomSoundHandler.RegisterCustomSound(AWPresentationGoal.key, AWPresentationAudioClip, AudioUtils.BusPaths.PDAVoice);
        FMODAsset presentation = ScriptableObject.CreateInstance<FMODAsset>();
        presentation.path = AWPresentationGoal.key;
        presentation.id = AWPresentationGoal.key;
        PDAHandler.AddLogEntry(
            AWPresentationGoal.key,
            "Subtitles_AWPresentation",
            sound: presentation
        );

// First lethal weapon PDA log "A lethal weapon have been detected into your inventory..."
        CustomSoundHandler.RegisterCustomSound("Log_PDA_Goal_FirstLethal", AWFirstLethalAudioClip, AudioUtils.BusPaths.PDAVoice);
        FMODAsset firstLethal = ScriptableObject.CreateInstance<FMODAsset>();
        firstLethal.path = "Log_PDA_Goal_FirstLethal";
        firstLethal.id = "Log_PDA_Goal_FirstLethal";
        PDAHandler.AddLogEntry(
            "Log_PDA_Goal_FirstLethal",
            "Subtitles_AWFirstLethal",
            sound: firstLethal
        );
    }

    private static void PatchPDAEncyEntries()
    {
        // Register AWModInfo entry
        PDAHandler.AddEncyclopediaEntry(new()
        {
            key = "AWModInfo",
            kind = PDAEncyclopedia.EntryData.Kind.Encyclopedia,
            nodes = new[] { "Meta" },
            path = "Meta",
            unlocked = true,
        });

        // Explosive torpedoes entry
        PDAHandler.AddEncyclopediaEntry(new()
        {
            key = "ExplosiveTorpedo",
            kind = PDAEncyclopedia.EntryData.Kind.Encyclopedia,
            nodes = new[] { "Tech", "Weaponry" },
            path = "Tech/Weaponry",
            unlocked = false,
        });

        // Prawn laser arm entry
        PDAHandler.AddEncyclopediaEntry(new()
        {
            key = "PrawnLaserArm",
            kind = PDAEncyclopedia.EntryData.Kind.Encyclopedia,
            nodes = new[] { "Tech", "Weaponry" },
            path = "Tech/Weaponry",
            unlocked = false,
        });

        // Prawn Self Defense Module
        PDAHandler.AddEncyclopediaEntry(new()
        {
            key = "PrawnDefensePerimeter",
            kind = PDAEncyclopedia.EntryData.Kind.Encyclopedia,
            nodes = new[] { "Tech", "Weaponry", "Modules" },
            path = "Tech/Weaponry/Modules",
            unlocked = false,
        });
    }
}
