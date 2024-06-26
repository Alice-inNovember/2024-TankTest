using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private List<GameObject> doors;
    // Start is called before the first frame update
    void Start()
    {
        Random.InitState((int)(transform.position.x + transform.position.y));
        doors[Random.Range(0, 3)].SetActive(false);
    }
}
