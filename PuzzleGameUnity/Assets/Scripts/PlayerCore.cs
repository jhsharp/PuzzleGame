/****
 * Created by: Jacob Sharp
 * Date Created: January 30, 2022
 * 
 * Last Edited by: Jacob Sharp
 * Date Last Edited: February 28, 2022
 * 
 * Description: Block type that manages all other player blocks
 ****/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCore : PlayerBlock
{
    public List<Block> playerBlocks = new List<Block>();
    public List<Block> tempConnections = new List<Block>();

    public float magnetMoveMultiplier = 1.005f;
    public float magnetMoveSpeed = 0.4f;

    public float pistonMoveSpeed = 0.75f;

    private string moveType = "magnet";
    private Vector3 moveDirection;
    private int moveDirectionSign;
    private float currentSpeed;
    private float targetX;
    private float targetY;
    private float moveBuffer = 0.04f;
    private bool collide = false;
    private int initialAttach = 3;

    void Start()
    {
        base.Start();
        playerBlocks.Add(this);
    }

    void Update()
    {
        base.Update();
        if (initialAttach > 0)
        {
            checkConnections(0, 0);
            attachBlocks();
            initialAttach--;
        }
        if ((targetX - transform.position.x) * moveDirectionSign > moveBuffer  || (targetY - transform.position.y) * moveDirectionSign > moveBuffer)
        {
            if (moveType == "magnet") magnetMove();
            else if (moveType == "piston") pistonMove();
        }
        else if (moving)
        {
            foreach (PlayerBlock blk in playerBlocks)
            {
                blk.transform.position = new Vector3(targetX + (blk.x - x) * manager.blockSize, targetY + (blk.y - y) * manager.blockSize);
                blk.moving = false;
            }
            attachBlocks();
        }
    }

    public bool checkCollision(int xShift, int yShift)
    {
        foreach (PlayerBlock blk in playerBlocks)
        {
            if (manager.checkBlock(blk.x + xShift, blk.y + yShift) != null && manager.checkBlock(blk.x + xShift, blk.y + yShift).type != "player") return true;
            if (blk.type == "piston")
            {
                /* CHECK PISTON COLLISIONS */
            }
        }
        return false;
    }

    public void checkConnections(int xShift, int yShift)
    {
        foreach (PlayerBlock blk in playerBlocks)
        {
            if (manager.checkBlock(blk.x + xShift, blk.y + yShift - 1) != null && manager.checkBlock(blk.x + xShift, blk.y + yShift - 1).type == "player" && !playerBlocks.Contains(manager.checkBlock(blk.x + xShift, blk.y + yShift - 1)) && !tempConnections.Contains(manager.checkBlock(blk.x + xShift, blk.y + yShift - 1))) // down
            {
                tempConnections.Add(manager.checkBlock(blk.x + xShift, blk.y + yShift - 1));
            }
            if (manager.checkBlock(blk.x + xShift, blk.y + yShift + 1) != null && manager.checkBlock(blk.x + xShift, blk.y + yShift + 1).type == "player" && !playerBlocks.Contains(manager.checkBlock(blk.x + xShift, blk.y + yShift + 1)) && !tempConnections.Contains(manager.checkBlock(blk.x + xShift, blk.y + yShift + 1))) // up
            {
                tempConnections.Add(manager.checkBlock(blk.x + xShift, blk.y + yShift + 1));
            }
            if (manager.checkBlock(blk.x + xShift - 1, blk.y + yShift) != null && manager.checkBlock(blk.x + xShift - 1, blk.y + yShift).type == "player" && !playerBlocks.Contains(manager.checkBlock(blk.x + xShift - 1, blk.y + yShift)) && !tempConnections.Contains(manager.checkBlock(blk.x + xShift - 1, blk.y + yShift))) // left
            {
                tempConnections.Add(manager.checkBlock(blk.x + xShift - 1, blk.y + yShift));
            }
            if (manager.checkBlock(blk.x + xShift + 1, blk.y + yShift) != null && manager.checkBlock(blk.x + xShift + 1, blk.y + yShift).type == "player" && !playerBlocks.Contains(manager.checkBlock(blk.x + xShift + 1, blk.y + yShift)) && !tempConnections.Contains(manager.checkBlock(blk.x + xShift + 1, blk.y + yShift))) // right
            {
                tempConnections.Add(manager.checkBlock(blk.x + xShift + 1, blk.y + yShift));
            }
        }
    }

    public void attachBlocks()
    {
        List<PlayerBlock> newBlocks = new List<PlayerBlock>();
        foreach (PlayerBlock blk in playerBlocks)
        {
            foreach (PlayerBlock attachment in tempConnections)
            {
                if (!newBlocks.Contains(attachment) && Vector3.Distance(blk.transform.position, attachment.transform.position) <= (manager.blockSize + currentSpeed * Time.deltaTime)) // moving up or down
                {
                    newBlocks.Add(attachment);
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

    public void resetPositions()
    {
        foreach (PlayerBlock blk in playerBlocks)
        {
            blk.transform.position = new Vector3(transform.position.x + (blk.x - x) * manager.blockSize, transform.position.y + (blk.y - y) * manager.blockSize);
        }
    }

    public void freezeBlocks(bool freeze)
    {
        foreach (PlayerBlock blk in playerBlocks)
        {
            blk.frozen = freeze;
        }
    }


    public void targetMoveHorizontal(int distance)
    {
        collide = false;
        int xShift = 0;
        int direction = 1;
        if (distance < 0) direction = -1;
        distance = Mathf.Abs(distance);
        while (distance > 0 && !collide)
        {
            xShift++;
            distance--;
            if (checkCollision(xShift * direction, 0))
            {
                xShift -= 1;
                collide = true;
            }
            checkConnections(xShift * direction, 0);
        }
        targetX = (x + xShift * direction) * manager.blockSize + manager.transform.position.x + manager.blockSize / 2;
        targetY = transform.position.y;
    }

    public void targetMoveVertical(int distance)
    {
        collide = false;
        int yShift = 0;
        int direction = 1;
        if (distance < 0) direction = -1;
        distance = Mathf.Abs(distance);
        while (distance > 0 && !collide)
        {
            yShift++;
            distance--;
            if (checkCollision(0, yShift * direction))
            {
                yShift -= 1;
                collide = true;
            }
            checkConnections(0, yShift * direction);
        }
        targetY = (y + yShift * direction) * manager.blockSize + manager.transform.position.x + manager.blockSize / 2;
        targetX = transform.position.x;
    }

    public void magnetMoveStart(Vector3 direction)
    {
        moveDirection = direction;
        if (direction.x != 0)
        {
            moveDirectionSign = (int)direction.x;
            targetMoveHorizontal(manager.roomWidth * moveDirectionSign);
        }
        else
        {
            moveDirectionSign = (int)direction.y;
            targetMoveVertical(manager.roomHeight * moveDirectionSign);
        }
        currentSpeed = magnetMoveSpeed;
        moveType = "magnet";
        foreach (PlayerBlock blk in playerBlocks)
        {
            blk.moving = true;
        }
    }

    public void magnetMove()
    {
        foreach (PlayerBlock blk in playerBlocks)
        {
            blk.moving = true;
            blk.transform.position += moveDirection * currentSpeed * Time.deltaTime;
        }
        attachBlocks();
        currentSpeed *= magnetMoveMultiplier;
    }

    public bool pistonMoveStart(Vector3 direction) // attempts to push off a block with the activated piston and returns whether the player has space to be pushed into
    {
        bool pistonMove = true;
        moveDirection = direction;
        if (direction.x != 0)
        {
            moveDirectionSign = (int)direction.x;
            targetMoveHorizontal(moveDirectionSign);
            if (Mathf.Abs(targetX - transform.position.x) < 0.01) pistonMove = false;
        }
        else
        {
            moveDirectionSign = (int)direction.y;
            targetMoveVertical(moveDirectionSign);
            if (Mathf.Abs(targetY - transform.position.y) < 0.01) pistonMove = false;
        }
        if (pistonMove)
        {
            currentSpeed = pistonMoveSpeed;
            moveType = "piston";
            foreach (PlayerBlock blk in playerBlocks)
            {
                blk.moving = true;
            }
        }
        return pistonMove;
    }

    public void pistonMove()
    {
        foreach (PlayerBlock blk in playerBlocks)
        {
            blk.moving = true;
            blk.transform.position += moveDirection * currentSpeed * Time.deltaTime;
        }
        attachBlocks();
    }
}
