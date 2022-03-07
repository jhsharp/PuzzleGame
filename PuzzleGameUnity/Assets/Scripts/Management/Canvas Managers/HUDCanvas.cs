/**** 
 * Created by: Akram Taghavi-Burrs
 * Date Created: Feb 23, 2022
 * 
 * Last Edited by: Jacob Sharp
 * Last Edited: March 6, 2022
 * 
 * Description: Updates HUD canvas referecing game manager
****/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDCanvas : MonoBehaviour
{
    /*** VARIABLES ***/

    GameManager gm; //reference to game manager

    [Header("Canvas SETTINGS")]
    public Text levelTextbox; //textbox for level count
    public Text movesTextbox; //textbox for the moves
    public Text bestMovesTextbox; //textbox for best moves
    
    //GM Data
    private int level;
    //private int totalLevels;
    private int moves;
    private int bestMoves;

    private void Start()
    {
        gm = GameManager.GM; //find the game manager

        //reference to level info
        level = gm.gameLevelsCount;
        //totalLevels = gm.gameLevels.Length;



        SetHUD();
    }//end Start

    // Update is called once per frame
    void Update()
    {
        GetGameStats();
        SetHUD();
    }//end Update()

    void GetGameStats()
    {
        moves = gm.Moves;
        bestMoves = gm.BestMoves;
    }

    void SetHUD()
    {
        //if texbox exsists update value
        if (levelTextbox) { levelTextbox.text = "Level " + level/* + "/" + totalLevels*/; }
        if (movesTextbox) { movesTextbox.text = "Moves:  " + moves; }
        if (bestMovesTextbox) { bestMovesTextbox.text = "Best: " + bestMoves; }

    }//end SetHUD()

}
