/****
 * Created by: Jacob Sharp
 * Date Created: March 5, 2022
 * 
 * Last Edited by: Jacob Sharp
 * Date Last Edited: March 6, 2022
 * 
 * Description: Controls the goal zone to move to the next level
 ****/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalZone : MonoBehaviour
{
    //public string nextScene; // the scene to move to upon level completion

    private int xMin, xMax, yMin, yMax; // bounds of the goal zone

    private bool transitioning; // variables to manage the transition time between levels
    private float transitionTimer;
    public float transitionDelay;

    private Collider2D col; // reference to collider
    private BlockManager manager; // reference to block manager
    private PlayerCore core; // reference to player core

    void Start()
    {
        col = GetComponent<Collider2D>(); // get a reference to the collider
        manager = FindObjectOfType<BlockManager>(); // get a reference to the block manager
        core = FindObjectOfType<PlayerCore>(); // get a reference to the player core

        // Set the bounds of the goal area based on the collider dimensions
        xMin = (int)Mathf.Floor((transform.position.x - (col.bounds.extents.x - 0.05f) - manager.transform.position.x) / manager.blockSize);
        xMax = (int)Mathf.Floor((transform.position.x + (col.bounds.extents.x - 0.05f) - manager.transform.position.x) / manager.blockSize);
        yMin = (int)Mathf.Floor((transform.position.y - (col.bounds.extents.y - 0.05f) - manager.transform.position.y) / manager.blockSize);
        yMax = (int)Mathf.Floor((transform.position.y + (col.bounds.extents.y - 0.05f) - manager.transform.position.y) / manager.blockSize);
    }

    void Update()
    {
        if (!transitioning) // if the level is not already complete, check for completion
        {
            if (checkComplete()) // if the level was just finished, start transitioning to the next level
            {
                transitioning = true;
                transitionTimer = transitionDelay;
                core.freezeBlocks(true);
            }
        }
        else transition(); // continue transitioning to the next stage
    }

    public bool checkComplete() // indicates whether all player controlled blocks are within the goal area
    {
        foreach (PlayerBlock blk in core.playerBlocks)
        {
            if (blk.x < xMin || blk.x > xMax || blk.y < yMin || blk.y > yMax) return false; // return false if there is a block outside the goal area
        }
        return true; // otherwise return true
    }

    private void transition() // manages the transition process to the next stage
    {
        if (transitionTimer <= 0) // if the transition time has elapsed, go to next level
        {
            GameManager.GM.NextLevel();
        }
        else transitionTimer -= Time.deltaTime; // otherwise continue counting down to the scene switch
    }
}
