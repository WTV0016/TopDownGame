using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public int lives;
    int livesbefore;
    public int drops;


    // Start is called before the first frame update
    void Start()
    {
        lives = Random.Range(6, 15);
        drops = Random.Range(1, 7);
    }

    public void DestroyStone(GameObject item, Transform container)
    {
        GameObject obj;
        for(int i = 0; i < drops; i++)
        {
            obj = Instantiate(item, position: new Vector3(transform.position.x + Random.Range(0,3), transform.position.y + Random.Range(0, 3), 0), Quaternion.identity);
            obj.transform.SetParent(container);
        }

        Destroy(transform.gameObject);
    }
}
