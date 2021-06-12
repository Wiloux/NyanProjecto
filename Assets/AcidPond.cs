using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidPond : MonoBehaviour
{
    // Start is called before the first frame update
    public float timer;
    public float timerMax;
    void Start()
    {
        timer = timerMax;      
    }

    // Update is called once per frame
    void Update()
    {
        if (timerMax >= 0)
        {
            if (timer >= 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
