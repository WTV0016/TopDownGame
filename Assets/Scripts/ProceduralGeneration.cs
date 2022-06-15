using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProceduralGeneration : MonoBehaviour
{
    public GameObject[] grassprefabs;
    public GameObject[] structureprefabs;
    public int size;
    public GameObject structurecontainer;

    int[,] map;

    public GameObject player;
    public int viewdistance;
    GameObject[] world1d;
    GameObject[,] world2d;
    GameObject[] activeChunks;

    int playerpos;


    void Start()
    {
        activeChunks = new GameObject[viewdistance * viewdistance];
        for (int i = 0; i < viewdistance * viewdistance; i++)
        {
            activeChunks[i] = null;
        }
        world1d = new GameObject[size];
        for (int x = 0; x < size; x++)
        {
            world1d[x] = null;
        }
        //seed = Mathf.RoundToInt(Random.Range(1,9999999));

        map = CreateArray(size);
        RenderMap(map, grassprefabs, world1d);
        StructureGeneration(structurecontainer, structureprefabs, size);
        //RenderWorld(map, prefabs, world2d, container);
        CheckView(player, world1d, activeChunks, size, viewdistance);

        playerpos = Mathf.RoundToInt(player.transform.position.x / 5);

        //CheckPlayerPos(player, world2d, viewdistance, activeChunks);

    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.RoundToInt(player.transform.position.x / 5) != playerpos)
        {
            playerpos = Mathf.RoundToInt(player.transform.position.x / 5);
            CheckView(player, world1d, activeChunks, size, viewdistance);
            //CheckPlayerPos(player, world2d, viewdistance, activeChunks);
        }

    }

    public int[,] CreateArray(int size)
    {
        int[,] map = new int[size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                map[x, y] = Mathf.RoundToInt(Random.Range(0,100));
            }
        }
        return map;
    }

    public void RenderMap(int[,] map, GameObject[] prefabs, GameObject[] world)
    {
        GameObject obj;
        GameObject Chunk;

        Vector3 pos = new Vector3(0, 0, 0);

        for (int x = 0; x < size; x++)
        {
            pos.x += 5;
            pos.y = 0;
            Chunk = new GameObject(name: "Chunk_" + x);
            Chunk.transform.parent = transform;
            Chunk.transform.position = new Vector2(pos.x, 0);
            world[x] = Chunk;

            for (int y = 0; y < size; y++)
            {
                pos.y += 5;
                if (map[x, y] == 0)
                {
                    obj = Instantiate(prefabs[1], position: new Vector3(pos.x,pos.y,0), Quaternion.identity);
                    obj.transform.parent = Chunk.transform;

                }
                else if (map[x, y] == 1)
                {
                    obj = Instantiate(prefabs[2], position: new Vector3(pos.x, pos.y, 0), Quaternion.identity);
                    obj.transform.parent = Chunk.transform;

                }
                else if (map[x, y] == 2)
                {
                    obj = Instantiate(prefabs[1], position: new Vector3(pos.x, pos.y, 0), Quaternion.identity);
                    obj.transform.parent = Chunk.transform;

                }
                else
                {
                    obj = Instantiate(prefabs[0], position: new Vector3(pos.x, pos.y, 0), Quaternion.identity);
                    obj.transform.parent = Chunk.transform;

                }
            }
        }


    }

    public void RenderWorld(int[,] map, GameObject[] prefabs, GameObject[,] world, GameObject container)
    {
        GameObject obj;

        Vector3 pos = new Vector3(0, 0, 0);

        for (int x = 0; x < size; x++)
        {
            pos.x += 5;
            pos.y = 0;
            

            for (int y = 0; y < size; y++)
            {
                pos.y += 5;
                if (map[x, y] == 0)
                {
                    obj = Instantiate(prefabs[2], position: new Vector3(pos.x, pos.y, 0), Quaternion.identity);
                    obj.transform.parent = container.transform;
                    obj.SetActive(false);

                    world[x, y] = obj;

                }
                else if (map[x, y] == 1)
                {
                    obj = Instantiate(prefabs[2], position: new Vector3(pos.x, pos.y, 0), Quaternion.identity);
                    obj.transform.parent = container.transform;
                    obj.SetActive(false);

                    world[x, y] = obj;

                }
                else if (map[x, y] == 2)
                {
                    obj = Instantiate(prefabs[2], position: new Vector3(pos.x, pos.y, 0), Quaternion.identity);
                    obj.transform.parent = container.transform;
                    obj.SetActive(false);

                    world[x, y] = obj;

                }
                else
                {
                    obj = Instantiate(prefabs[0], position: new Vector3(pos.x, pos.y, 0), Quaternion.identity);
                    obj.transform.parent = container.transform;
                    obj.SetActive(false);

                    world[x, y] = obj;

                }


            }
        }


    }

    void CheckView(GameObject Player, GameObject[] world, GameObject[] activeChunks, int width, int viewdistance)
    {
        int count = 0;


        foreach (GameObject tile in activeChunks)
        {
            try
            {
                tile.SetActive(false);

            }
            catch
            {

            }
        }
        for (int x = 0; x < width; x++)
        {
            if (world[x].transform.position.x > Player.transform.position.x - viewdistance && world[x].transform.position.x < Player.transform.position.x + viewdistance)
            {
                world[x].SetActive(true);
                activeChunks[count] = world[x];
                count++;
            }
            else { world[x].SetActive(false); }
        }
    }

    void CheckPlayerPos(GameObject Player, GameObject[,] world, int viewdistance, GameObject[] activetiles)
    {
        int count = 0;
        Vector2Int worldpostoindex = new Vector2Int(Mathf.RoundToInt(Player.transform.position.x /*Mathf.RoundToInt(Mathf.Sqrt(world.Length) / 2*/), Mathf.RoundToInt(Player.transform.position.y) /*Mathf.RoundToInt(Mathf.Sqrt(world.Length) / 2)*/);
        foreach(GameObject gobj in activetiles)
        {
            try { gobj.SetActive(false); }
            catch { }
            
        }
        for (int x = 0; x < viewdistance; x++)
        {
            for(int y = 0; y < viewdistance; y++)
            {
                try
                {
                    world[worldpostoindex.x + x - (viewdistance / 2), worldpostoindex.y + y - (viewdistance / 2)].SetActive(true);
                    activetiles[count] = world[worldpostoindex.x + x - (viewdistance / 2), worldpostoindex.y + y - (viewdistance / 2)];
                    count++;
                }
                catch
                {
                     
                }
                Debug.Log(worldpostoindex.x + ", " + worldpostoindex.y);
            }
        }
    }

    void StructureGeneration(GameObject container, GameObject[] prefabs, int size)
    {
        int randint = 0;
        GameObject obj = null;
        Debug.Log(size);
        for(int x = 0; x < size; x++)
        {
            for(int y = 0; y < size; y++)
            {
                randint = Mathf.RoundToInt(Random.Range(0, 75000));
                if (randint == 1)
                {
                    obj = Instantiate(prefabs[Mathf.RoundToInt(Random.Range(0, 3))], position: new Vector3(x, y, 0), Quaternion.identity);
                    obj.transform.SetParent(container.transform);
                }
            }
        }
    }
}
