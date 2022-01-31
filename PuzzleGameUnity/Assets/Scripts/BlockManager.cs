using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public float blockSize;
    public int roomWidth;
    public int roomHeight;
    public Block[,] blockGrid;

    // Start is called before the first frame update
    void Start()
    {
        blockGrid = new Block[roomWidth, roomHeight];
        for (int i = 0; i < roomWidth; i++)
        {
            for (int j = 0; j < roomHeight; j++)
            {
                blockGrid[i, j] = null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void storeBlock(int x, int y, Block block)
    {
        blockGrid[x, y] = block;
    }

    public void clearBlock(Block block)
    {
        for (int i = 0; i < roomWidth; i++)
        {
            for (int j = 0; j < roomHeight; j++)
            {
                if (blockGrid[i, j] == block) blockGrid[i, j] = null;
            }
        }
    }

    public void moveBlock(int x, int y, Block block)
    {
        clearBlock(block);
        storeBlock(x, y, block);
    }

    public Block checkBlock(int x, int y)
    {
        if (blockGrid[x, y] == null) return null;
        else return blockGrid[x, y];
    }
}
