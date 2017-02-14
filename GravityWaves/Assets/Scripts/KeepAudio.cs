
using UnityEngine;

public class KeepAudio : MonoBehaviour
{
	void Start ()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Menu");
        if (objs.Length == 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
