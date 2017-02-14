using UnityEngine;

public class PushScript : MonoBehaviour
{
    [SerializeField]
    [Range(0.01f, 1f)]
    private float duration = 0.05f;

    private float elapsedTime = 0f;
    public GameObject WaveEffect;
    public ParticleSystem ps;
    public Vector3 PlayerForward;

    // Use this for initialization
    void Start()
    {
        float angle = -75f;
        if (PlayerForward.x < 0)
            angle = 105f;

        Instantiate(WaveEffect, transform.position, Quaternion.Euler(0, angle, 0));
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= duration)
            Destroy(gameObject);
    }
}

