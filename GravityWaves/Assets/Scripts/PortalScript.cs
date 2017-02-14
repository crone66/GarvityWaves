using UnityEngine;

public class PortalScript : MonoBehaviour
{
    public GameObject OutPortal;
    public AudioSource source;
    public int Index;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void Teleport(GameObject drone)
    {
        drone.transform.position = new Vector3(OutPortal.transform.position.x, drone.transform.position.y, OutPortal.transform.position.z);
        Enemy enmey = drone.GetComponent<Enemy>();
        enmey.startPos = drone.transform.position;

        DamageAbleObject damageScript = drone.GetComponent<DamageAbleObject>();
        damageScript.DoDamage(1);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(OutPortal != null)
        {
            if(collision.gameObject.CompareTag("Drone"))
            {
                source.Play();
                Enemy script = collision.gameObject.GetComponent<Enemy>();
                if (script.CurrentLane.gameObject == gameObject.transform.parent.gameObject)
                {
                    script.ElapsedExplodeTime = 0f;
                    Teleport(collision.gameObject);
                }
            }
        }
    }
}
