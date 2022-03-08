/**** 
 * Created by: Akram Taghavi-Burrs
 * Date Created: Feb 23, 2022
 * 
 * Last Edited by: Jacob Sharp
 * Date Last Edited: March 7, 2022
 * 
 * Description: GameManager object for the entire game
****/

/** Import Libraries **/
using System; //C# library for system properties
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //libraries for accessing scenes


public class GameManager : MonoBehaviour
{
    /*** VARIABLES ***/

    #region GameManager Singleton
    static private GameManager gm; //refence GameManager
    static public GameManager GM { get { return gm; } } //public access to read only gm 

    //Check to make sure only one gm of the GameManager is in the scene
    void CheckGameManagerIsInScene()
    {
    
        //Check if instnace is null
        if (gm == null)
        {
           gm = this; //set gm to this gm of the game object
            Debug.Log(gm);
        }
        else //else if gm is not null a Game Manager must already exsist
        {
            Destroy(this.gameObject); //In this case you need to delete this gm
        }
        DontDestroyOnLoad(this); //Do not delete the GameManager when scenes load
        Debug.Log(gm);
    }//end CheckGameManagerIsInScene()
    #endregion

    [Header("GENERAL SETTINGS")]
    public string gameTitle = "Untitled Game";  //name of the game
    public string gameCredits = "Made by Me"; //game creator(s)
    public string copyrightDate = "Copyright " + thisDay; //date cretaed

    [Header("GAME SETTINGS")]

    [Tooltip("Will the high score be recoreded")]
    public bool recordBestMoves = false; //is best moves recorded

    //[SerializeField] //Access to private variables in editor
    private int[] defaultBestMoves;
    static public int[] bestMoves; // the default best moves
    public int BestMoves { get { return bestMoves[SceneManager.GetActiveScene().buildIndex]; } set { bestMoves[SceneManager.GetActiveScene().buildIndex] = value; } } //access to private variable best moves [get/set methods]

    [Space(10)]

    static public int moves;  //score value
    public int Moves { get { return moves; } set { moves = value; } }//access to private variable moves [get/set methods]

    [Space(10)]

    public string winMessage = "You Win!"; //Message if player wins
    [HideInInspector] public string endMsg ;//the end screen message, depends on winning outcome

    [Header("SCENE SETTINGS")]
    [Tooltip("Name of the start scene")]
    public string startScene;
    
    [Tooltip("Name of the game over scene")]
    public string gameOverScene;
    
    [Tooltip("Count and name of each Game Level (scene)")]
    public string[] gameLevels; //names of levels
    [HideInInspector]
    public int gameLevelsCount; //what level we are on
    private int loadLevel; //what level from the array to load
     
    public static string currentSceneName; //the current scene name;

    [Header("FOR TESTING")]
    public bool nextLevel = false; //test for next level

    //Game State Varaiables
    [HideInInspector] public enum gameStates { Idle, Playing, Death, GameOver, BeatLevel };//enum of game states
    [HideInInspector] public gameStates gameState = gameStates.Idle;//current game state

    //Timer Varaibles
    private float currentTime; //sets current time for timer
    private bool gameStarted = false; //test if games has started

    //Win/Loose conditon
    [SerializeField] //to test in inspector
    private bool playerWon = true;
 
   //reference to system time
   private static string thisDay = System.DateTime.Now.ToString("yyyy"); //today's date as string


    /*** MEHTODS ***/
   
   //Awake is called when the game loads (before Start).  Awake only once during the lifetime of the script instance.
    void Awake()
    {
        //runs the method to check for the GameManager
        CheckGameManagerIsInScene();

        //store the current scene
        currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        //create the best moves array with an index for each scene
        bestMoves = new int[gameLevels.Length + 2];
        defaultBestMoves = new int[] { 0, 9, 12, 41, 32, 41, 26, 39, 40, 0 };

        //Get the saved best moves
        GetBestMoves();

    }//end Awake()


    // Update is called once per frame
    private void Update()
    {
        //if ESC is pressed , exit game
        //if (Input.GetKey("escape")) { ExitGame(); }

        // if R is pressed, restart the room
        if (Input.GetKey("r"))
        {
            moves = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        //Check for next level
        if (nextLevel) { NextLevel(); }

        //Check Score
        //CheckMoves();


    }//end Update


    //LOAD THE GAME FOR THE FIRST TIME OR RESTART
   public void StartGame()
    {
        //SET ALL GAME LEVEL VARIABLES FOR START OF GAME

        gameLevelsCount = 1; //set the count for the game levels
        loadLevel = gameLevelsCount - 1; //the level from the array
        if (!nextLevel)
        {
            SceneManager.LoadScene(gameLevels[loadLevel]); //load first game level
            PlayerPrefs.SetInt("Current Level", gameLevelsCount);
        }

        gameState = gameStates.Playing; //set the game state to playing

        moves = 0; //set starting score

        /*
        //set High Score
        if (recordBestMoves) //if we are recording highscore
        {
            //if the high score, is less than the default high score
            if (highScore <= defaultHighScore)
            {
                highScore = defaultHighScore; //set the high score to defulat
                PlayerPrefs.SetInt("HighScore", highScore); //update high score PlayerPref
            }//end if (highScore <= defaultHighScore)
        }//end  if (recordHighScore)
        */

        endMsg = winMessage; //set the end message default

        playerWon = true; //set player winning condition to false
    }//end StartGame()



    //EXIT THE GAME
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exited Game");
    }//end ExitGame()


    //GO TO THE GAME OVER SCENE
    public void GameOver()
    {
        gameState = gameStates.GameOver; //set the game state to gameOver

       //if(playerWon) { endMsg = winMessage; } else { endMsg = looseMessage; } //set the end message

        SceneManager.LoadScene(gameOverScene); //load the game over scene
        Debug.Log("Gameover");
    }
    
    
    //GO TO THE NEXT LEVEL
    public void NextLevel()
    {
        if (!nextLevel)
        {
            CheckMoves(); // update best moves if necessary
        }
        nextLevel = false; //reset the next level
        moves = 0;

        //as long as our level count is not more than the amount of levels
        if (gameLevelsCount < gameLevels.Length)
        {
            gameLevelsCount++; //add to level count for next level
            loadLevel = gameLevelsCount - 1; //find the next level in the array
            PlayerPrefs.SetInt("Current Level", gameLevelsCount);
            Debug.Log(loadLevel);
            Debug.Log(gameLevels[loadLevel]);
            SceneManager.LoadScene(gameLevels[loadLevel]); //load next level

        }else{ //if we have run out of levels go to game over
            GameOver();
        } //end if (gameLevelsCount <=  gameLevels.Length)

    }//end NextLevel()

    // LOAD A PARTICULAR LEVEL
    public void LoadGame()
    {
        nextLevel = true;
        StartGame(); // initialize game stats
        gameLevelsCount = PlayerPrefs.GetInt("Current Level") - 1; // set level
        NextLevel();
    }

    void CheckMoves()
    { //This method manages the score on update. Right now it just checks if we are greater than the high score.
  
        //if the score is more than the high score
        if (moves < BestMoves)
        { 
            BestMoves = moves; //set the best moves to current moves
            PlayerPrefs.SetInt("BestMoves Scene " + SceneManager.GetActiveScene().buildIndex, BestMoves); //set the playerPref for best moves in that scene
        }//end if(score > highScore)

    }//end CheckMoves()

    void GetBestMoves()
    {//Get the saved highscore

        for (int i = 0; i < bestMoves.Length; i++)
        {
            //if the PlayerPref alredy exists for the high score
            if (PlayerPrefs.HasKey("BestMoves Scene " + i))
            {
                //Debug.Log("Has Key");
                bestMoves[i] = PlayerPrefs.GetInt("BestMoves Scene " + i); //set the high score to the saved high score
                if (bestMoves[i] == 0)
                {
                    PlayerPrefs.SetInt("BestMoves Scene " + i, defaultBestMoves[i]); //set the playerPref to default best moves
                    bestMoves[i] = defaultBestMoves[i];
                }
            }//end if (PlayerPrefs.HasKey("HighScore"))
            else
            {
                PlayerPrefs.SetInt("BestMoves Scene " + i, defaultBestMoves[i]); //set the playerPref to default best moves
                bestMoves[i] = defaultBestMoves[i];
            }
        }
    }//end GetBestMoves()

}
