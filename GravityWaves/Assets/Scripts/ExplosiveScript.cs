using UnityEngine;

public class ExplosiveScript : MonoBehaviour
{
    private float duration = 3f;
    private float elapsedTime = 0f;
	
	// Update is called once per frame
	void Update ()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > duration)
            Destroy(gameObject);
	}
}
