/****
 * Created by: Jacob Sharp
 * Date Created: February 4, 2022
 * 
 * Last Edited by: Jacob Sharp
 * Date Last Edited: March 4, 2022
 * 
 * Description: Controls key blocks and makes them open locks they touch
 ****/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKey : PlayerBlock
{
    private PlayerCore core; // reference to player core script

    void Start()
    {
        base.Start();

        core = FindObjectOfType<PlayerCore>(); // get a reference to the player core script
    }

    void Update()
    {
        base.Update();

        if (core.playerBlocks.Contains(this) && !moving && !frozen) // open any adjacent locks if in the player object and not moving/frozen
        {
            if (manager.checkBlock(x + 1, y) != null && manager.checkBlock(x + 1, y).type == "lock") openLock(x + 1, y);
            if (manager.checkBlock(x - 1, y) != null && manager.checkBlock(x - 1, y).type == "lock") openLock(x - 1, y);
            if (manager.checkBlock(x, y + 1) != null && manager.checkBlock(x, y + 1).type == "lock") openLock(x, y + 1);
            if (manager.checkBlock(x, y - 1) != null && manager.checkBlock(x, y - 1).type == "lock") openLock(x, y - 1);
        }
        
    }

    public void openLock(int x, int y) // open a lock that the key has touched and remove the key
    {
        manager.checkBlock(x, y).GetComponent<LockBlock>().open(); // open the lock

        manager.clearBlock(this); // remove this block from manager and player core lists
        core.playerBlocks.Remove(this);

        Destroy(this.gameObject); // destroy the key
    }
}
