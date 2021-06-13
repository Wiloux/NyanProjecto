using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Vector3> pos = new List<Vector3>();
    public float spd;

    public AudioSource _AudioSource;

    public List<ClipVolume> UIClips = new List<ClipVolume>();

    void Start()
    {
        //int children = transform.childCount;
        //for (int i = 0; i < children; ++i)
        //    pos.Add(transform.GetChild(i).GetComponent<RectTransform>().transform.position);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        StartCoroutine(StartLoop());
    }

    private void OnDisable()
    {
        StopCoroutine(StartLoop());
    }

    public IEnumerator StartLoop()
    {
        int children = transform.childCount;
        while (true)
        {
            int uiId = Random.Range(0, UIClips.Count);

            _AudioSource.PlayOneShot(UIClips[uiId].clip, UIClips[uiId].volume);


            for (int i = 0; i < children; ++i)
            {
                int rdm = Random.Range(0, 2);
                if (rdm == 0)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
                else
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            yield return new WaitForSeconds(UIClips[uiId].clip.length);

            yield return new WaitForSeconds(spd);
        }
    }
}
