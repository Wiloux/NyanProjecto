using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPivot : MonoBehaviour
{

    [SerializeField] private Transform player;
    public Transform camPosition;
    public Transform camZoomPosition;

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position;
    }
}
