using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidPond : MonoBehaviour
{
    // Start is called before the first frame update
    public Boss bossScript;
    public float timer;
    public float timerMax;

    [Space(10)]
    [SerializeField] private float damageBySec;

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
                bossScript.acidPonds.Remove(this);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().DealDamage(damageBySec * Time.deltaTime);
        }
    }
}
