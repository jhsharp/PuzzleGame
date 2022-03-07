/****
 * Created by: Jacob Sharp
 * Date Created: February 28, 2022
 * 
 * Last Edited by: Jacob Sharp
 * Date Last Edited: March 4, 2022
 * 
 * Description: Manages the extension and retraction of piston blocks
 ****/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiston : PlayerBlock
{
    public int pistonDirection; // direction of the piston arm

    [HideInInspector] public int extension = 0; // how far the arm is currently extended
    public int extensionMax = 3; // how far the arm can extend

    public GameObject armParent; // used to control the arm location
    public GameObject[] armSegments; // individual segments of the arm

    private PlayerCore core; // reference to player core
    [HideInInspector] public Vector3 pistonDirectionVector; // int used in the editor to determine the direction of the piston

    private float moveSpeed; // how quickly the piston arms move
    private Vector3 targetPos; // used to direct the piston arm's movement

    // Booleans to manage the state of the piston arm
    private bool extending = false;
    private bool retracting = false;
    private bool armMoving = false;

    void Start()
    {
        base.Start();

        // Set the direction vector of the piston
        if (pistonDirection == 0) pistonDirectionVector = up;
        else if (pistonDirection == 1) pistonDirectionVector = right;
        else if (pistonDirection == 2) pistonDirectionVector = down;
        else if (pistonDirection == 3) pistonDirectionVector = left;

        core = FindObjectOfType<PlayerCore>(); // get a reference to the player core

        moveSpeed = core.pistonMoveSpeed; // get the piston move speed from the player core
    }

    void Update()
    {
        base.Update();

        // Extend the arm if it is mid-extension
        if (extending)
        {
            pistonExtend();
            // Check if the arm has reached its destination
            if (Mathf.Abs(armParent.transform.localPosition.x - targetPos.x) < (moveSpeed * Time.deltaTime) && Mathf.Abs(armParent.transform.localPosition.y - targetPos.y) < (moveSpeed * Time.deltaTime))
            {
                armParent.transform.localPosition = targetPos; // lock the arm into place
                core.freezeBlocks(false); // allow the player to be interacted with
                extending = false; // stop extending the arm
            }
        }

        // Retract the arm if it is currently retracting
        if (retracting)
        {
            pistonRetract();
            // Check if the arm has reached its destination
            if (Mathf.Abs(armParent.transform.localPosition.x - targetPos.x) < (moveSpeed * Time.deltaTime) && Mathf.Abs(armParent.transform.localPosition.y - targetPos.y) < (moveSpeed * Time.deltaTime))
            {
                armSegments[extension].SetActive(false); // make the sprite for the retracted segment of the arm invisible
                armParent.transform.localPosition = targetPos; // lock the arm into place
                core.freezeBlocks(false); // allow the player to be interacted with
                retracting = false; // stop retracting
            }
        }

        // Move the arm if the piston is pushing off a wall
        if (armMoving)
        {
            pistonMoveArm();
            if (!moving) // if the piston has finished moving, stop moving the arm
            {
                armMoving = false;
                armParent.transform.localPosition = extension * pistonDirectionVector * manager.blockSize; // reset local position to be exact
            }
        }
    }

    public override void activate() // extend the piston on left click
    {
        base.activate();
        if (extension >= extensionMax) return; // don't extend if the arm is aleady at max extension
        if (core.playerBlocks.Contains(this)) // only activate if the piston is controlled by the player
        {
            // Update the arm segment that is to be extended and up the extension count
            armSegments[extension].SetActive(true);
            extension++;

            // Check to see if the arm is going to collide with another block
            Block collisionBlock = manager.checkBlock(x + (int)pistonDirectionVector.x * (extension), y + (int)pistonDirectionVector.y * (extension));
            if (collisionBlock != null) // if there is a collision block
            {
                if (core.playerBlocks.Contains(collisionBlock) && collisionBlock != this) // if the direction of the piston contains a player controlled block, don't extend the piston
                {
                    extension--;
                    armSegments[extension].SetActive(false);
                }
                else // otherwise attempt to push off of the wall
                {
                    if (core.pistonMoveStart(-pistonDirectionVector)) // only push off the wall if there is room for the player to move
                    {
                        pistonMoveArmStart();
                        gm.Moves++; // add a move
                    }
                    else // if the player is blocked, don't extend the piston
                    {
                        extension--;
                        armSegments[extension].SetActive(false);
                    }
                }
            }
            else // if there is no block in the way of the pison, extend the arm
            {
                pistonExtendStart();
            }
        }
    }

    public override void deactivate() // retract the arm on right click
    {
        base.deactivate();
        if (core.playerBlocks.Contains(this) && extension > 0) // only retract if the arm is at least partially extended
        {
            extension--;
            pistonRetractStart();
        }
    }

    public void pistonExtendStart() // start the extension process for the arm
    {
        targetPos = armParent.transform.localPosition + pistonDirectionVector * manager.blockSize; // set the destination for the arm
        core.freezeBlocks(true); // stop player interaction
        extending = true; // start extending
        gm.Moves++; // add a move
    }
    public void pistonExtend() // continue moving the piston arm out
    {
        armParent.transform.localPosition += pistonDirectionVector * moveSpeed * Time.deltaTime;
        
    }

    public void pistonRetractStart() // start the retraction process for the arm
    {
        targetPos = armParent.transform.localPosition - pistonDirectionVector * manager.blockSize; // set the destination for the arm
        core.freezeBlocks(true); // stop player interaction
        retracting = true; // start retracting
        gm.Moves++; // add a move
    }

    public void pistonRetract() // continue retracting the piston arm
    {
        armParent.transform.localPosition -= pistonDirectionVector * moveSpeed * Time.deltaTime;
    }

    public void pistonMoveArmStart() // prepare the arm to push off the wall
    {
        targetPos = armParent.transform.position; // lock the arms position
        armMoving = true; // start the pushing process
    }

    public void pistonMoveArm() // keep the arm in place as the piston pushes off the wall
    {
        armParent.transform.position = targetPos;
    }
}
