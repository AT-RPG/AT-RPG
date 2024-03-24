using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    public Transform player;
    public Transform building;
    public Transform npc;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        (transform as RectTransform).anchoredPosition  = Camera.allCameras[1].WorldToViewportPoint(player.position);

    }
}
