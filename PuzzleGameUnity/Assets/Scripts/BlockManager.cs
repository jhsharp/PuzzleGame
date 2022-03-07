/****
 * Created by: Jacob Sharp
 * Date Created: January 30, 2022
 * 
 * Last Edited by: Jacob Sharp
 * Date Last Edited: January 30, 2022
 * 
 * Description: Keeps track of all blocks in the level
 ****/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public float blockSize; // size of block objects
    public int roomWidth; // room width in terms of block count
    public int roomHeight; // room height in terms of block count
    public Block[,] blockGrid; // grid that keeps track of all block loactions

    void Start()
    {
        // Create an empty grid for blocks to be stored in
        blockGrid = new Block[roomWidth, roomHeight];
        for (int i = 0; i < roomWidth; i++)
        {
            for (int j = 0; j < roomHeight; j++)
            {
                blockGrid[i, j] = null;
            }
        }
    }

    public void storeBlock(int x, int y, Block block) // store the specified block at the specified position in the grid
    {
        blockGrid[x, y] = block;
    }

    public void clearBlock(Block block) // remove the specified block from the grid
    {
        for (int i = 0; i < roomWidth; i++)
        {
            for (int j = 0; j < roomHeight; j++)
            {
                if (blockGrid[i, j] == block) blockGrid[i, j] = null;
            }
        }
    }

    public void moveBlock(int x, int y, Block block) // move a block from its current location to the specified position in the grid
    {
        clearBlock(block);
        storeBlock(x, y, block);
    }

    public Block checkBlock(int x, int y) // return the block in the specified position in the grid
    {
        if (blockGrid[x, y] == null) return null;
        else return blockGrid[x, y];
    }
}
