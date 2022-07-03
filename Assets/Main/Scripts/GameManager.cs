using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static List<Player> players = new List<Player>();

    public static void AddPlayer(Player player)
    {
        players.Add(player);
    }

    public static void RemovePlayer(Player player)
    {
        players.Remove(player);
    }
}
