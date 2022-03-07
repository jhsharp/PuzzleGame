/****
 * Created by: Jacob Sharp
 * Date Created: January 30, 2022
 * 
 * Last Edited by: Jacob Sharp
 * Date Last Edited: January 30, 2022
 * 
 * Description: Basic class for all block types
 ****/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    internal BlockManager manager;
    internal GameManager gm; //reference to game manager

    public string type;
    public bool fixedPosition = true;

    public int x;
    public int y;

    internal bool stored = false;

    internal Vector3 up = new Vector3(0, 1, 0);
    internal Vector3 down = new Vector3(0, -1, 0);
    internal Vector3 left = new Vector3(-1, 0, 0);
    internal Vector3 right = new Vector3(1, 0, 0);


    // Start is called before the first frame update
    internal void Start()
    {
        // Get references to block manager and game manager
        manager = FindObjectOfType<BlockManager>();
        gm = GameManager.GM;

        x = (int)Mathf.Floor((transform.position.x - manager.transform.position.x) / manager.blockSize);
        y = (int)Mathf.Floor((transform.position.y - manager.transform.position.y) / manager.blockSize);
    }

    // Update is called once per frame
    internal void Update()
    {
        // Store block if not stored
        if (manager.checkBlock(x, y) != this) 
        {
            manager.storeBlock(x, y, this);
        }

        // Set Position
        x = (int)Mathf.Floor((transform.position.x - manager.transform.position.x) / manager.blockSize);
        y = (int)Mathf.Floor((transform.position.y - manager.transform.position.y) / manager.blockSize);

        if (fixedPosition)
        {
            Vector3 newPos = new Vector3(x * manager.blockSize + manager.transform.position.x + manager.blockSize / 2, y * manager.blockSize + manager.transform.position.y + manager.blockSize / 2, 0);
            transform.position = newPos;
        }
    }
}
