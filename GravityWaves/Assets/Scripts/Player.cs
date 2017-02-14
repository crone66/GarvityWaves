using Assets.Scripts;
using System;
using UnityEngine;
using XInputDotNetPure;

public class Player : MonoBehaviour
{
    private PlayerIndex index;
    private bool hasPlayerIndex;
    private bool isDead;

    private Vector3 moveVector;
    private Quaternion targetRotation;
    
    private float fireDelay = 1f;
    private float elapsedFireDelay = 0f;
    
    private MoveScript moveScript;
    private AudioSource source;

    [SerializeField]
    [Range(1f, 100f)]
    private float speed = 1f;

    public Texture ColorPlayer1;
    public Texture ColorPlayer2;
    public Texture ColorPlayer3;
    public Texture ColorPlayer4;

    public bool Freeze = false;
    public bool RotateOnMove = false;
    public GameObject Push;
    public DamageAbleObject HealthContainer;
    public AudioClip shot;
    
    [HideInInspector]
    public PlayerIndex Index
    {
        get
        {
            return index;
        }

        set
        {
            index = value;
            hasPlayerIndex = true;
        }
    }

    public bool IsDead
    {
        get { return isDead; }
    }

    private void Start ()
    {
        source = GetComponent<AudioSource>();
        Renderer renderer = GetComponentInChildren<Renderer>();

        if(index == PlayerIndex.One)
            renderer.material.SetTexture("_MainTex", ColorPlayer1);
        if (index == PlayerIndex.Two)
            renderer.material.SetTexture("_MainTex", ColorPlayer2);
        if (index == PlayerIndex.Three)
            renderer.material.SetTexture("_MainTex", ColorPlayer3);
        if (index == PlayerIndex.Four)
            renderer.material.SetTexture("_MainTex", ColorPlayer4);
  
        if (HealthContainer != null)
        {
            HealthContainer.OnDeath += HealthContainer_OnDeath;
        }

        moveScript = GetComponent<MoveScript>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (hasPlayerIndex)
        {
            GamePadState state = GamePad.GetState(Index);
            if (state.IsConnected)
            {
                if(!isDead)
                {
                    if(elapsedFireDelay < fireDelay)
                        elapsedFireDelay += Time.deltaTime;

                    Input(state);
                    UpdateRotation();
                }
            }
            else
            {
                GamePadManager.Disconnect(Index);
            }
        }
    }

    private void Input(GamePadState state)
    {
        Vector2 leftStick = new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
        Vector2 rightStick = new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
        TryMove(leftStick, rightStick);

        if (moveScript != null)
        {
            moveScript.Move(moveVector * 100);
            moveVector = Vector3.zero;
        }
        
        if (state.Triggers.Right > 0)
        {
            if(elapsedFireDelay >= fireDelay)
            {
                elapsedFireDelay = 0f;
                GameObject gobj = Instantiate(Push, transform.position + (new Vector3(0, 2, transform.forward.x) * 2), Quaternion.Euler(0, 90, 0));
                PushScript push = gobj.GetComponent<PushScript>();
                push.PlayerForward = transform.forward;
                source.PlayOneShot(shot);
            }
        }
    }
    
    private void TryMove(Vector2 leftStick, Vector2 rightStick)
    {
        if (!Freeze)
        {
            if (leftStick.x > 0.1f || leftStick.x < 0.1f)
                moveVector += Vector3.right * leftStick.x * Time.deltaTime * speed;

            float rightAngle = rightStick.y > 0 ? 90f : -90f;
            if (rightStick != Vector2.zero)
            {
                targetRotation = Quaternion.Euler(0, rightAngle, 0);
            }
        }
    }

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 14);
    }

    private float CalculateAngle(Vector2 target, Vector2 source)
    {
        return ((float)Math.Atan2(target.y - source.y, target.x - source.x)) * (180f / (float)Math.PI);
    }

    private float FixAngle(float value)
    {
        if (value < 0)
            return 360 + value;

        return value;
    }
    
    private void HealthContainer_OnDeath(object sender, EventArgs e)
    {
        isDead = true;

        Destroy(gameObject);
    }
}
