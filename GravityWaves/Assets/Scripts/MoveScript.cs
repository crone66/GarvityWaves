using System;
using UnityEngine;
using Assets.Scripts;

public class MoveScript : MonoBehaviour
{
    private Rigidbody physics;
    private Vector3 movement;
    private float defaultMovementMultiplicator;
    
    [SerializeField]
    [Range(0, 2)]
    private float MovementMultiplicator = 1f;

    [SerializeField]
    private bool UseRigidBody = false;

    public bool AddGravity = true;
    public event EventHandler<OnMovingArgs> OnMoving;
 
    private void Start()
    {
        defaultMovementMultiplicator = MovementMultiplicator;
        if(UseRigidBody)
            physics = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(!UseRigidBody)
        {
            OnMovingArgs args = new OnMovingArgs(movement);

            if (OnMoving != null)
                OnMoving.Invoke(this, args);

            if (!args.Cancel)
            {
                transform.position += (args.Velocity * MovementMultiplicator);
                movement = Vector3.zero;
            }
        }
    }

    private void FixedUpdate()
    {      
        if (UseRigidBody)
        {
            OnMovingArgs args = new OnMovingArgs(movement + (AddGravity ? Physics.gravity : Vector3.zero));

            if (OnMoving != null)
                OnMoving.Invoke(this, args);

            if (!args.Cancel)
            {
                physics.velocity = (args.Velocity * MovementMultiplicator);
                movement = Vector3.zero;
            }
        }
    }

    public void Move(Vector3 movement)
    {
        this.movement = movement;
    }

    public void ResetMultiplicator()
    {
        MovementMultiplicator = defaultMovementMultiplicator;
    }
}


