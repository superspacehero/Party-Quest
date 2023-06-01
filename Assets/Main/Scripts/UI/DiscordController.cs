using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;

public class DiscordController : MonoBehaviour
{
    public long applicationID;
    [Space]
    public string details = "Playing";
    public string state = "In Game";
    [Space]
    public string largeImageKey = "game_icon";
    public string largeImageText = "Game Icon";

    private long startTime;

    private static bool instanceExists = false;
    public Discord.Discord discord;
    
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (!instanceExists)
        {
            instanceExists = true;
            DontDestroyOnLoad(gameObject);
        }
        else if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        discord = new Discord.Discord(applicationID, (System.UInt64)CreateFlags.NoRequireDiscord);
        startTime = System.DateTimeOffset.Now.ToUnixTimeSeconds();

        UpdatePresence();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // Destroy the GameObject if the Discord instance is null
        try
        {
            discord.RunCallbacks();
        }
        catch
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// LateUpdate is called every frame, if the Behaviour is enabled.
    /// It is called after all Update functions have been called.
    /// </summary>
    void LateUpdate()
    {
        UpdatePresence();
    }

    public void UpdatePresence()
    {
        try
        {
            var activityManager = discord.GetActivityManager();
            var activity = new Discord.Activity
            {
                Details = details,
                State = state,
                Timestamps =
                {
                    Start = startTime
                },
                Assets =
                {
                    LargeImage = largeImageKey,
                    LargeText = largeImageText
                }
            };

            activityManager.UpdateActivity(activity, (res) =>
            {
                if (res == Discord.Result.Ok)
                {
                    // Debug.Log("Discord Rich Presence updated successfully!");
                }
                else
                {
                    Debug.LogError("Discord Rich Presence failed to update!");
                }
            });
        }
        catch
        {
            Debug.LogError("Discord Rich Presence failed to update!");
            Destroy(gameObject);
        }
    }
}
