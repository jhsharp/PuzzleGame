/****
 * Created by: Jacob Sharp
 * Date Created: January 30, 2022
 * 
 * Last Edited by: Jacob Sharp
 * Date Last Edited: March 4, 2022
 * 
 * Description: Manages activation of magnet blocks
 ****/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagnet : PlayerBlock
{
    public int magnetDirection; // variable that allows the direction of the magnet to be set in the editor

    private PlayerCore core; // reference to player core
    private Vector3 magnetDirectionVector; // direction vector for which way the magnet is facing

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        // Set the direction of the magnet
        if (magnetDirection == 0) magnetDirectionVector = up;
        else if (magnetDirection == 1) magnetDirectionVector = right;
        else if (magnetDirection == 2) magnetDirectionVector = down;
        else if (magnetDirection == 3) magnetDirectionVector = left;

        core = FindObjectOfType<PlayerCore>(); // get a reference to the player core
    }

    void Update()
    {
        base.Update();
    }

    public override void activate() // attempt to move with the magnet
    {
        base.activate();
        if (core.playerBlocks.Contains(this)) // only move if the magnet block is activated
        {
            // Check whether there is line of sight to a magnet block
            int i = 1;
            Block checkBlock = manager.checkBlock(x + (int)magnetDirectionVector.x * i, y + (int)magnetDirectionVector.y * i);
            while (checkBlock == null || (checkBlock.type == "player" && core.playerBlocks.Contains(checkBlock))) // find ther nearest non empty space that isn't a player controlled block
            {
                i++;
                checkBlock = manager.checkBlock(x + (int)magnetDirectionVector.x * i, y + (int)magnetDirectionVector.y * i);
            }
            if (manager.checkBlock(x + (int)magnetDirectionVector.x * i, y + (int)magnetDirectionVector.y * i).type == "magnet") // move if there the nearest block in the line is a magnet
            {
                core.magnetMoveStart(magnetDirectionVector);
            }
        }
    }
}
