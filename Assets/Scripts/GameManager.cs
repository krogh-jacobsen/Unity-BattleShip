using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    //[System.Serializable]
    //public class Player
    //{
    //    public enum PlayerType
    //    {
    //        Human,
    //        NPC
    //    }
    //    public PlayerType playerType;
    //}

    int activePlayer;
    public Player[] players = new Player[2];
}
