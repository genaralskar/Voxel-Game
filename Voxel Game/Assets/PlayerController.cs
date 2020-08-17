using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float gravityMod = 1f;

    [SerializeField]
    private float jumpMod = 1f;

    [SerializeField]
    private float edgeDetectDistance = .5f;

    public GameObject art;

    private Vector3 moveVector = Vector3.zero;
    private CharacterController cc;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.z = Input.GetAxis("Vertical");


        if(Input.GetButtonDown("Jump") && cc.isGrounded)
        {
            moveVector.y = jumpMod;
        }

        if(moveVector.y > -gravityMod)
        {
            moveVector.y -= gravityMod * Time.deltaTime;
            if (moveVector.y < -gravityMod)
                moveVector.y = -gravityMod;
        }


        Move();
    }

    private void Move()
    {
        //get x and z for movement stuff, normalize if to large
        Vector3 xz = new Vector3(moveVector.x, 0, moveVector.z);
        xz = xz.normalized;

        //rotate art to face move direction
        if (xz.sqrMagnitude > .1f)
        {
            Quaternion rot = Quaternion.LookRotation(xz);
            transform.rotation = rot;
        }

        //raycast to see if player on edge, seperate checks so the player can still slide along the edge
        Vector3 xOffset = new Vector3(xz.normalized.x * edgeDetectDistance, 1.1f, 0);
        Vector3 zOffset = new Vector3(0, 1.1f, xz.normalized.z * edgeDetectDistance);

        Ray rayx = new Ray(transform.position + xOffset, Vector3.down);
        Ray rayz = new Ray(transform.position + zOffset, Vector3.down);

        Debug.DrawRay(transform.position + xOffset, Vector3.down * 2.3f, Color.red, .1f);
        Debug.DrawRay(transform.position + zOffset, Vector3.down * 2.3f, Color.blue, .1f);

        xz *= moveSpeed;

        float ypos = transform.position.y;
        if (!Physics.Raycast(rayx, out var hitx, 2.3f))
        {
            xz.x = 0;
        }
        else
        {
            ypos = hitx.point.y;
        }
        if(!Physics.Raycast(rayz, out var hitz, 2.3f))
        {
            xz.z = 0;
        }
        else
        {
            ypos = hitz.point.y;
        }

        


        //add y jump/gravity stuff
        //xz.y = moveVector.y;

        cc.Move(xz * Time.deltaTime);

        Vector3 t = transform.position;
        t.y = ypos;
        transform.position = t;
        

    }
}
