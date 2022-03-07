/****
 * Created by: Jacob Sharp
 * Date Created: January 30, 2022
 * 
 * Last Edited by: Jacob Sharp
 * Date Last Edited: March 4, 2022
 * 
 * Description: Block type that manages all other player blocks
 ****/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCore : PlayerBlock
{
    public List<Block> playerBlocks = new List<Block>(); // list of blocks controlled by the player
    public List<Block> tempConnections = new List<Block>(); // blocks that will eventually become connected along the player's path of motion

    // Paramaters for the magnet movement
    public float magnetMoveMultiplier = 1.005f;
    public float magnetMoveSpeed = 0.4f;

    public float pistonMoveSpeed = 0.75f; // the speed of piston movement

    private string moveType = ""; // the type of movement currently being used
    private Vector3 moveDirection; // the direction of movement
    private int moveDirectionSign; // the sign of the movement direction
    private float currentSpeed; // how fast the player is currently moving

    // The target grid position of the core when movement is occuring
    private float targetX;
    private float targetY;

    private float moveBuffer = 0.04f; // how far the player can be from its destination before it is locked into place and movement stops
    private bool collide = false; // whether a collision has been detected
    private int initialAttach = 10; // a number of frames at level load in which the player core attempts to attach all nearby blocks

    void Start()
    {
        base.Start();
        playerBlocks.Add(this); // add the core to the player blocks list
    }

    void Update()
    {
        base.Update();

        // Attach any blocks near the core's starting position in the level
        if (initialAttach > 0) 
        {
            checkConnections(0, 0);
            attachBlocks();
            initialAttach--;
        }

        // Continue moving if there is still distance for the player to travel while moving
        if ((targetX - transform.position.x) * moveDirectionSign > moveBuffer  || (targetY - transform.position.y) * moveDirectionSign > moveBuffer) 
        {
            if (moveType == "magnet") magnetMove();
            else if (moveType == "piston") pistonMove();
        }

        // Stop moving if the player has reached the destination
        else if (moving)
        {
            // Lock each player controlled block into place and stop if from moving
            foreach (PlayerBlock blk in playerBlocks)
            {
                blk.transform.position = new Vector3(targetX + (blk.x - x) * manager.blockSize, targetY + (blk.y - y) * manager.blockSize);
                blk.moving = false;
            }

            // Attach nearby blocks and update their positions in the manager
            checkConnections(0, 0);
            attachBlocks();
            updateManagerPositions();
        }
    }

    public bool checkCollision(int xShift, int yShift) // returns whether the player can move the specified amount without a collision
    {
        foreach (PlayerBlock blk in playerBlocks) // check for currently connected blocks
        {
            if (manager.checkBlock(blk.x + xShift, blk.y + yShift) != null && manager.checkBlock(blk.x + xShift, blk.y + yShift).type != "player") return true; // check the target location of each block for a collision
            if (blk.gameObject.GetComponent<PlayerPiston>() != null) // check for piston arm collisions
            {
                PlayerPiston piston = (PlayerPiston)blk;
                for (int i = 1; i <= piston.extension; i++)
                {
                    if (manager.checkBlock(blk.x + xShift + i * (int)piston.pistonDirectionVector.x, blk.y + yShift + i*(int)piston.pistonDirectionVector.y) != null && manager.checkBlock(blk.x + xShift + i * (int)piston.pistonDirectionVector.x, blk.y + yShift + i * (int)piston.pistonDirectionVector.y).type != "player") return true;
                }
            }
        }
        foreach (PlayerBlock blk in tempConnections) // check for blocks that will eventually be connected
        {
            if (xShift != 0) // if moving horizonatlly
            {
                if (manager.checkBlock(blk.x + xShift - blk.tempOffset, blk.y) != null && manager.checkBlock(blk.x + xShift - blk.tempOffset, blk.y).type != "player") return true;
            }
            else if (yShift != 0) // if moving vertically
            {
                if (manager.checkBlock(blk.x, blk.y + yShift - blk.tempOffset) != null && manager.checkBlock(blk.x, blk.y + yShift - blk.tempOffset).type != "player") return true;
            }
        }
        return false; // indicate if there were no collisions
    }

    public void checkConnections(int xShift, int yShift) // check for connections when the player is stationary (checks in all directions)
    {
        PlayerBlock currentConnection;
        foreach (PlayerBlock blk in playerBlocks)
        {
            // Check down
            if (manager.checkBlock(blk.x + xShift, blk.y + yShift - 1) != null && manager.checkBlock(blk.x + xShift, blk.y + yShift - 1).type == "player" && !playerBlocks.Contains(manager.checkBlock(blk.x + xShift, blk.y + yShift - 1)) && !tempConnections.Contains(manager.checkBlock(blk.x + xShift, blk.y + yShift - 1))) // down
            {
                currentConnection = (PlayerBlock)manager.checkBlock(blk.x + xShift, blk.y + yShift - 1);
                tempConnections.Add(currentConnection); // add the block to the connection list
                currentConnection.tempOffset = yShift; // set its offset based on how far it currently is from the player
            }

            // Check up
            if (manager.checkBlock(blk.x + xShift, blk.y + yShift + 1) != null && manager.checkBlock(blk.x + xShift, blk.y + yShift + 1).type == "player" && !playerBlocks.Contains(manager.checkBlock(blk.x + xShift, blk.y + yShift + 1)) && !tempConnections.Contains(manager.checkBlock(blk.x + xShift, blk.y + yShift + 1))) // up
            {
                currentConnection = (PlayerBlock)manager.checkBlock(blk.x + xShift, blk.y + yShift + 1);
                tempConnections.Add(currentConnection);
                currentConnection.tempOffset = yShift;
            }

            // Check left
            if (manager.checkBlock(blk.x + xShift - 1, blk.y + yShift) != null && manager.checkBlock(blk.x + xShift - 1, blk.y + yShift).type == "player" && !playerBlocks.Contains(manager.checkBlock(blk.x + xShift - 1, blk.y + yShift)) && !tempConnections.Contains(manager.checkBlock(blk.x + xShift - 1, blk.y + yShift))) // left
            {
                currentConnection = (PlayerBlock)manager.checkBlock(blk.x + xShift - 1, blk.y + yShift);
                tempConnections.Add(currentConnection);
                currentConnection.tempOffset = xShift;
            }

            // Check right
            if (manager.checkBlock(blk.x + xShift + 1, blk.y + yShift) != null && manager.checkBlock(blk.x + xShift + 1, blk.y + yShift).type == "player" && !playerBlocks.Contains(manager.checkBlock(blk.x + xShift + 1, blk.y + yShift)) && !tempConnections.Contains(manager.checkBlock(blk.x + xShift + 1, blk.y + yShift))) // right
            {
                currentConnection = (PlayerBlock)manager.checkBlock(blk.x + xShift + 1, blk.y + yShift);
                tempConnections.Add(currentConnection);
                currentConnection.tempOffset = xShift;
            }
        }
    }

    public void checkConnectionsMotion(int xShift, int yShift) // check for connections when the player is in motion (only checks in direction of motion)
    {
        PlayerBlock currentConnection;
        Block checkBlock;
        foreach (PlayerBlock blk in playerBlocks)
        {
            checkBlock = manager.checkBlock(blk.x + xShift + (int)moveDirection.x, blk.y + yShift + (int)moveDirection.y); // only check in the direction of motion
            if (checkBlock != null && checkBlock.type == "player" && !playerBlocks.Contains(checkBlock) && !tempConnections.Contains(checkBlock))
            {
                currentConnection = (PlayerBlock)checkBlock;
                tempConnections.Add(currentConnection); // add the block to the connection list
                currentConnection.tempOffset = yShift; // set its offset based on how far it currently is from the player
            }
        }
    }

    public void attachBlocks() // attaches blocks when the player is stationary (attaches in all directions)
    {
        List<PlayerBlock> newBlocks = new List<PlayerBlock>();
        foreach (PlayerBlock blk in playerBlocks)
        {
            foreach (PlayerBlock attachment in tempConnections)
            {
                // Attach nearby blocks if they are adjacent to a player block
                if (!newBlocks.Contains(attachment) && Vector3.Distance(blk.transform.position, attachment.transform.position) <= (manager.blockSize + currentSpeed * Time.deltaTime + 0.001))
                {
                    newBlocks.Add(attachment);
                    attachment.tempOffset = 0;
                }

            }
        }
        foreach (PlayerBlock blk in newBlocks)
        {
                playerBlocks.Add(blk);
                tempConnections.Remove(blk);
                resetPositions();
        }   
    }

    public void attachBlocksMotion() // attaches blocks when the player is moving (only attaches in the direction of motion)
    {
        List<PlayerBlock> newBlocks = new List<PlayerBlock>();
        foreach (PlayerBlock blk in playerBlocks)
        {
            // Attach blocks that are immediately in a player block's path of motion
            foreach (PlayerBlock attachment in tempConnections)
            {
                if (!newBlocks.Contains(attachment) && (Mathf.Abs(blk.transform.position.x - attachment.transform.position.x) <= (moveDirection.x*(manager.blockSize + currentSpeed * Time.deltaTime) + 0.001)) && (Mathf.Abs(blk.transform.position.y - attachment.transform.position.y) <= (moveDirection.y * (manager.blockSize + currentSpeed * Time.deltaTime) + 0.001)))
                {
                    newBlocks.Add(attachment);
                    attachment.tempOffset = 0;
                }

            }
        }
        foreach (PlayerBlock blk in newBlocks)
        {
            playerBlocks.Add(blk);
            tempConnections.Remove(blk);
            resetPositions();
        }
    }

    public void resetPositions() // locks all of the player controlled blocks into their appropriate grid spaces
    {
        foreach (PlayerBlock blk in playerBlocks)
        {
            blk.transform.position = new Vector3(transform.position.x + (blk.x - x) * manager.blockSize, transform.position.y + (blk.y - y) * manager.blockSize);
        }
    }

    public void updateManagerPositions() // updates the manager with each player controlled block's position
    {
        foreach (PlayerBlock blk in playerBlocks)
        {
            manager.moveBlock(blk.x, blk.y, blk);
        }
    }

    public void freezeBlocks(bool freeze) // enable/disable blocks from being interactable
    {
        foreach (PlayerBlock blk in playerBlocks)
        {
            blk.frozen = freeze;
        }
    }


    public void targetMoveHorizontal(int distance) // determine how far a block can move horizontally
    {
        // Set initial parameters
        collide = false;
        int xShift = 0;
        int direction = 1;
        if (distance < 0) direction = -1;
        distance = Mathf.Abs(distance);

        // Keep checking one block out at a time to search for collisions and stop when a collision has been found or the intended distance has been reached
        while (distance > 0 && !collide)
        {
            xShift++;
            distance--;
            if (checkCollision(xShift * direction, 0))
            {
                xShift -= 1;
                collide = true;
            }
            checkConnectionsMotion(xShift * direction, 0); // record any connections that would be made along the player's path of motion
        }

        // Set the destination of the player core
        targetX = (x + xShift * direction) * manager.blockSize + manager.transform.position.x + manager.blockSize / 2;
        targetY = transform.position.y;
    }

    public void targetMoveVertical(int distance) // determine how far a block can move vertically
    {
        // Set initial parameters
        collide = false;
        int yShift = 0;
        int direction = 1;
        if (distance < 0) direction = -1;
        distance = Mathf.Abs(distance);

        // Keep checking one block out at a time to search for collisions and stop when a collision has been found or the intended distance has been reached
        while (distance > 0 && !collide)
        {
            yShift++;
            distance--;
            if (checkCollision(0, yShift * direction))
            {
                yShift -= 1;
                collide = true;
            }
            checkConnectionsMotion(0, yShift * direction); // record any connections that would be made along the player's path of motion
        }

        // Set the destination of the player core
        targetY = (y + yShift * direction) * manager.blockSize + manager.transform.position.x + manager.blockSize / 2;
        targetX = transform.position.x;
    }

    public void magnetMoveStart(Vector3 direction) // Prepare the player blocks for magnet movement
    {
        moveDirection = direction; // update the move direction

        // Find the destination for horizontal movement
        if (direction.x != 0)
        {
            moveDirectionSign = (int)direction.x;
            targetMoveHorizontal(manager.roomWidth * moveDirectionSign);
        }

        // Find the destination for vertical movement
        else
        {
            moveDirectionSign = (int)direction.y;
            targetMoveVertical(manager.roomHeight * moveDirectionSign);
        }

        // Update movement paramaters
        currentSpeed = magnetMoveSpeed;
        moveType = "magnet";

        // Set all blocks to the moving state
        foreach (PlayerBlock blk in playerBlocks)
        {
            blk.moving = true;
        }
        if (Mathf.Abs(targetX - transform.position.x) > 0.05f || Mathf.Abs(targetY - transform.position.y) > 0.05f) gm.Moves++; // add a move if the player is actually moving a distance
    }

    public void magnetMove() // continue moving all blocks with magnet movement
    {
        // Move all player controlled blocks
        foreach (PlayerBlock blk in playerBlocks)
        {
            blk.moving = true;
            blk.transform.position += moveDirection * currentSpeed * Time.deltaTime;
        }
        attachBlocksMotion(); // attach any blocks the player has reached
        currentSpeed *= magnetMoveMultiplier; // accelerate the motion
    }

    public bool pistonMoveStart(Vector3 direction) // attempts to push off a block with the activated piston and returns whether the player has space to be pushed into
    {
        // Set parameters
        bool pistonMove = true;
        moveDirection = direction;

        // Find the destination for horizontal movement
        if (direction.x != 0)
        {
            moveDirectionSign = (int)direction.x;
            targetMoveHorizontal(moveDirectionSign);
            if (Mathf.Abs(targetX - transform.position.x) < 0.01) pistonMove = false; // if the player has no room to move, indicate this by setting pistonMove to false
        }

        // Find the destination for vertical movement
        else
        {
            moveDirectionSign = (int)direction.y;
            targetMoveVertical(moveDirectionSign);
            if (Mathf.Abs(targetY - transform.position.y) < 0.01) pistonMove = false; // if the player has no room to move, indicate this by setting pistonMove to false
        }

        // If the piston has room to move, start movement
        if (pistonMove)
        {
            currentSpeed = pistonMoveSpeed;
            moveType = "piston";
            foreach (PlayerBlock blk in playerBlocks)
            {
                blk.moving = true;
            }
        }
        return pistonMove; // return whether the player has room to move
    }

    public void pistonMove() // continue piston movement
    {
        // Move all player controlled blocks
        foreach (PlayerBlock blk in playerBlocks)
        {
            blk.moving = true;
            blk.transform.position += moveDirection * currentSpeed * Time.deltaTime;
        }
        attachBlocksMotion(); // attach any blocks immediately in the player's path
    }
}
