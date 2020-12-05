using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public List<Player> players = new List<Player>();

    public GameObject MeeplePrefab;


    public void CreatePlayer(int id, string name, Color32 color)
    {
        players.Add(new Player(id, name, color, MeeplePrefab));
       // players.AddLast(new Player(name, color));

    }

    public List<Player> GetPlayers()
    {
        return players;
    }

    public Player GetPlayer(int index)
    {
        return players[index];
    }


    public class Player
    {    
        private Material mat;
        private int score;
        private int id;
        private string playerName;
        private Color32 playerColor;
        public GameObject[] meeples;

        public Material GetMaterial()
        {
            return mat;
        }
        public Player(int id, string name, Color32 color, GameObject MeeplePrefab)
        {
            this.id = id;
            this.playerName = name;
            mat = new Material(Shader.Find("Diffuse"));
            mat.name = playerName;
            mat.SetColor("_Color", color);
            this.playerColor = color;
            this.score = 0;
            this.meeples = generateMeeples(this, MeeplePrefab);
        }

        public int getID()
        {
            return id;
        }
       
        public string GetPlayerName()
        {
            return playerName;
        }
        public void SetPlayerName(String playerName)
        {
            this.playerName = playerName;
        }

        public int GetPlayerScore()
        {
            return score;
        }
        public void SetPlayerScore(int playerScore)
        {
            this.score = playerScore;
        }
        public void addScore(int scoreToAdd)
        {
            this.score = this.score + scoreToAdd;
        }

        public Color32 GetPlayerColor()
        {
            return playerColor;
        }
        public void SetPlayerColor(Color32 playerColor)
        {
            this.playerColor = playerColor;
        }

        
        /// <summary>
        /// Generates 8 meeples for the player.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="MeeplePrefab"></param>
        /// <returns></returns>
        private GameObject[] generateMeeples(Player player, GameObject MeeplePrefab)
        {
            GameObject[] res = new GameObject[8];
            for (int i = 0; i < 8; i++)
            {
                GameObject meeple = GameObject.Instantiate(MeeplePrefab, new Vector3(20.0f, 20.0f, 20.0f), Quaternion.identity);
                meeple.GetComponent<MeepleScript>().createByPlayer(player);
                meeple.transform.parent = UnityEngine.GameObject.Find("BaseTile").transform;
                res[i] = meeple;
            }
            return res;
        }
    }


}
