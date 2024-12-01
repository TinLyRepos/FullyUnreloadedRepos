using System;
using UnityEngine;

public static class Settings
{
    // UNITS
    public const float pixelsPerUnit = 16.0f;
    public const float tileSizePixels = 16.0f;

    // DUNGEON BUILD SETTINGS
    public const int MAX_DUNGEON_REBUILD_ATTEMPTS_FOR_GRAPH = 1000;
    public const int MAX_DUNGEON_REBUILD_ATTEMPTS = 10;

    // ROOM SETTINGS
    public const int MAX_CHILD_CORRIDORS = 3;
    public const float FADE_IN_TIME = 0.5f;
    public const float DOOR_UNLOCK_DELAY = 1f;

    // ANIMATOR PARAMETERS - PLAYERS
    public static int aimUp         = Animator.StringToHash("aimUp");
    public static int aimDown       = Animator.StringToHash("aimDown");
    public static int aimUpRight    = Animator.StringToHash("aimUpRight");
    public static int aimUpLeft     = Animator.StringToHash("aimUpLeft");
    public static int aimRight      = Animator.StringToHash("aimRight");
    public static int aimLeft       = Animator.StringToHash("aimLeft");
    public static int isIdle        = Animator.StringToHash("isIdle");
    public static int isMoving      = Animator.StringToHash("isMoving");

    public static int rollUp        = Animator.StringToHash("rollUp");
    public static int rollDown      = Animator.StringToHash("rollDown");
    public static int rollLeft      = Animator.StringToHash("rollLeft");
    public static int rollRight     = Animator.StringToHash("rollRight");

    public static int flipUp        = Animator.StringToHash("flipUp");
    public static int flipRight     = Animator.StringToHash("flipRight");
    public static int flipLeft      = Animator.StringToHash("flipLeft");
    public static int flipDown      = Animator.StringToHash("flipDown");

    public static int use           = Animator.StringToHash("use");

    public static float baseSpeedForPlayerAnimations = 8f;

    // Animator parameters - ENEMY
    public static float baseSpeedForEnemyAnimations = 3f;

    // ANIMATOR PARAMETERS - DOORS
    public static int open = Animator.StringToHash("open");

    // ANIMATOR PARAMETERS - DamageableDecoration
    public static int destroy = Animator.StringToHash("destroy");
    public static String stateDestroyed = "Destroyed";

    // GAMEOBJECT TAGS
    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";

    // AUDIO
    public const float musicFadeOutTime = 0.5f;  // Defualt Music Fade Out Transition
    public const float musicFadeInTime = 0.5f;  // Default Music Fade In Transition

    // FIRING CONTROL
    // if the target distance is less than this then the aim angle will be used (calculated from player)
    // else the weapon aim angle will be used (calculated from the weapon).
    public const float useAimAngleDistance = 3.5f; 

    // ASTAR PATHFINDING PARAMETERS
    public const int defaultAStarMovementPenalty = 40;
    public const int preferredPathAStarMovementPenalty = 1;
    public const int targetFrameRateToSpreadPathfindingOver = 60;
    public const float playerMoveDistanceToRebuildPath = 3f;
    public const float enemyPathRebuildCooldown = 2f;

    // UI PARAMETERS
    public const float uiHeartSpacing = 16f;
    public const float CANVAS_AMMO_ICON_SPACING = 4f;

    // ENEMY PARAMETERS
    public const int defaultEnemyHealth = 20;

    // CONTACT DAMAGE PARAMETERS
    public const float contactDamageCollisionResetDelay = 0.5f;

    // HIGHSCORES
    public const int numberOfHighScoresToSave = 100;
}