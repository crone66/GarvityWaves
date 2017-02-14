using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public DamageAbleObject Health;
    public GameObject Explosives;
    private MoveScript moveScript;

    public Texture HealthTexture1;
    public Texture HealthTexture2;
    public Texture HealthTexture3;
    public Texture HealthTexture4;
    public Texture HealthTexture5;

    private Vector3 movement;

    public LaneScript CurrentLane;
    private bool isPending;

    [SerializeField]
    [Range(1, 1000)]
    private float speed = 5f;

    private float amplitude = 2f;
    private float period = 0.5f;
    public Vector3 startPos;
    public bool pendingUp;
    private bool reverse = false;

    private float explodeDelay = 30;
    public float ElapsedExplodeTime = 0f;

    private Renderer myRenderer;
    private Quaternion targetRotation;
    public GameObject ParticleSystem;
    public GameObject particleObject;
    private ParticleSystem ps;
    private LaneScript lastLane;

    // Use this for initialization
    void Start()
    {
        Health = GetComponent<DamageAbleObject>();
        myRenderer = GetComponent<Renderer>();
        particleObject = Instantiate(ParticleSystem);
        ps = particleObject.GetComponentInChildren<ParticleSystem>();
        myRenderer.material.SetTexture("_MainTex", HealthTexture1);
        Health.OnDeath += Health_OnDeath;
        Health.OnReceiveDamage += Health_OnReceiveDamage;
        moveScript = GetComponent<MoveScript>();
        moveScript.AddGravity = false;
        startPos = transform.position;
    }

    private void Health_OnReceiveDamage(object sender, Assets.Scripts.OnHealthChangedArgs e)
    {
        float perc = (Health.Health - e.ChangeValue) / Health.MaxHealth;
        if(perc <= 0.21f)
        {
            myRenderer.material.SetTexture("_MainTex", HealthTexture5);
        }
        else if (perc <= 0.41f)
        {
            myRenderer.material.SetTexture("_MainTex", HealthTexture4);
        }
        else if (perc <= 0.61f)
        {
            myRenderer.material.SetTexture("_MainTex", HealthTexture3);
        }
        else if (perc <= 0.81f)
        {
            myRenderer.material.SetTexture("_MainTex", HealthTexture2);
        }
    }

    private void Health_OnDeath(object sender, EventArgs e)
    {
        explodeDelay = 3f;
        ElapsedExplodeTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Health.Health <= 0)
        {
            ElapsedExplodeTime += Time.deltaTime;
            if (ElapsedExplodeTime > explodeDelay)
            {
                DamageAbleObject damage = CurrentLane.gameObject.GetComponent<DamageAbleObject>();
                damage.DoDamage(1);
                Instantiate(Explosives, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 14);

        WaveUpdate();
        if (isPending)
        {
            if(pendingUp)
            {
                startPos += new Vector3(0, 0, speed) * Time.deltaTime;
                if (!reverse && startPos.z > CurrentLane.transform.position.z)
                {
                    startPos = new Vector3(startPos.x, startPos.y, CurrentLane.transform.position.z);
                    isPending = false;
                }
                else if(reverse && startPos.z > lastLane.transform.position.z + 6f)
                {
                    startPos = new Vector3(startPos.x, startPos.y, CurrentLane.transform.position.z - 3f);
                    reverse = false;
                }
            }
            else
            {
                startPos -= new Vector3(0, 0, speed) * Time.deltaTime;
                if (!reverse && startPos.z < CurrentLane.transform.position.z)
                {
                    startPos = new Vector3(startPos.x, startPos.y, CurrentLane.transform.position.z);
                    isPending = false;
                }
                else if (reverse && startPos.z < lastLane.transform.position.z - 6f)
                {
                    startPos = new Vector3(startPos.x, startPos.y, CurrentLane.transform.position.z + 3);
                    reverse = false;
                }
            }
        }


        transform.position += new Vector3(speed * CurrentLane.Direction, 0, 0) * Time.deltaTime;
        startPos = new Vector3(transform.position.x, startPos.y, startPos.z);

        if (moveScript != null)
        {
            moveScript.Move(movement);
            if(!isPending)
                movement = Vector3.zero;
        }
    }

    float elapsedTime = 0f;
    private void WaveUpdate()
    {
        elapsedTime += Time.deltaTime;
        
        float theta = elapsedTime / period;
        float distance = amplitude * Mathf.Sin(theta);
        transform.position = startPos + new Vector3(0, 0, 1) * distance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Health.Health < 0)
            ElapsedExplodeTime = 0;

        if (!isPending)
        {
            if (other.gameObject.CompareTag("Push"))
            {
                Health.DoDamage(1);
                if (CurrentLane.transform.position.z > other.transform.position.z)
                    MoveLaneDown();
                else
                    MoveLaneUp();

                Destroy(other.gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Drone"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy.CurrentLane == CurrentLane)
            {
                particleObject.transform.position = transform.position;
                if (!ps.isPlaying)
                    ps.Play();
                enemy.Health.DoDamage(1);
                if (pendingUp)
                {
                    enemy.MoveLaneUp();
                }
                else
                {
                    enemy.MoveLaneDown();
                }
            }
        }
    }

    public void MoveLaneDown()
    {
        int index = CurrentLane.Index - 1;
        int loops = 0;
        LaneScript[] lanes = GameObject.FindObjectsOfType<LaneScript>();
        do
        {
            if (index < 0)
                index = 3;

            for (int i = 0; i < lanes.Length; i++)
            {
                if (lanes[i].Index == index)
                {
                    ChangeLane(lanes[i], false);
                    return;
                }
            }
            index--;
            loops++;
        } while (loops <= lanes.Length);

        Debug.Log("Lane not found!");
    }

    public void MoveLaneUp()
    {
        int loops = 0;
        int index = CurrentLane.Index + 1;
        LaneScript[] lanes = GameObject.FindObjectsOfType<LaneScript>();
        do
        {
            if (index > 3)
                index = 0;

            for (int i = 0; i < lanes.Length; i++)
            {
                if (lanes[i].Index == index)
                {
                    ChangeLane(lanes[i], true);
                    return;
                }
            }
            index++;
            loops++;
        } while (loops <= lanes.Length);
        Debug.Log("Lane not found!");
    }

    private void ChangeLane(LaneScript newLane, bool up)
    {
        LaneScript[] lanes = GameObject.FindObjectsOfType<LaneScript>();
        if (up && IsMin(lanes, newLane) && IsMax(lanes, CurrentLane))
        {
            pendingUp = true;
            reverse = true;
        }
        else if(!up && IsMin(lanes, CurrentLane) && IsMax(lanes, newLane))
        {
            pendingUp = false;
            reverse = true;
        }
        else
        {
            pendingUp = CurrentLane.transform.position.z < newLane.transform.position.z;
            reverse = false;
        }

        if (CurrentLane.Direction != newLane.Direction)
            targetRotation = Quaternion.Euler(0, 180, 0) * transform.rotation;

        lastLane = CurrentLane;
        CurrentLane = newLane;      
        isPending = true;
    }

    public bool IsMin(LaneScript[] lanes, LaneScript curLane)
    {
        for (int i = 0; i < lanes.Length; i++)
        {
            if (curLane != lanes[i] && lanes[i].Index < curLane.Index)
                return false;
        }
        return true;
    }

    public bool IsMax(LaneScript[] lanes, LaneScript curLane)
    {
        for (int i = 0; i < lanes.Length; i++)
        {
            if (curLane != lanes[i] && lanes[i].Index > curLane.Index)
                return false;
        }
        return true;
    }
}
