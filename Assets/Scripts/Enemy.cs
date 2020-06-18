using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    public bool isGrounded;
    public Vector3 velocity;
    private World world;
    private float verticalMomentum = 0;
    private float gravity = -9.8f;
    public float jumpForce = 0.1f;
    private float jumpDelay;
    private bool colide = false;
    private GameObject player;
    private new Rigidbody rigidbody;

    void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();
        player = GameObject.Find("Player");
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        CalculateVelocity();
        transform.Translate(velocity, Space.World);
    }

    private void Update()
    {
        if (!colide)
        {
            var playerPosition = player.transform.position - new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
            transform.rotation = Quaternion.LookRotation(playerPosition);

            var normVector = playerPosition.normalized * 2;
            rigidbody.velocity = new Vector3(normVector.x, rigidbody.velocity.y, normVector.z);

            if ((rigidbody.velocity.z > 0 && front) || (rigidbody.velocity.z < 0 && back))
            {
                rigidbody.velocity = new Vector3(normVector.x, rigidbody.velocity.y, 0);
                Jump();
            }

            if ((rigidbody.velocity.x > 0 && right) || (rigidbody.velocity.x < 0 && left))
            {
                rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, normVector.z);
                Jump();
            }
        }
        else
            rigidbody.velocity = new Vector3();

        if (Vector3.Distance(transform.position, GameObject.Find("Player").transform.position) < 5 && !colide)
        {
            Invoke("SynchroCor", 2);
            colide = true;
        }

        jumpDelay -= Time.deltaTime;
    }

    void Jump()
    {
        if (jumpDelay <= 0) {
            verticalMomentum = jumpForce;
            jumpDelay = 0.5f;
            isGrounded = false;
        }
    }

    void SynchroCor()
    {
        if (Vector3.Distance(transform.position, GameObject.Find("Player").transform.position) < 5) {
            var player = GameObject.Find("Player").GetComponent<Player>();
            player.lifes -= 1;
            GameObject.Find("Heart" + player.lifes).SetActive(false);
        }

        DestroyBlock();
        gameObject.transform.position = new Vector3(GameObject.Find("Player").transform.position.x + UnityEngine.Random.Range(-50f, 50f), GameObject.Find("Player").transform.position.y, GameObject.Find("Player").transform.position.z + UnityEngine.Random.Range(-50f, 50f));
        colide = false;
    }

    private void DestroyBlock()
    {
        world.DestroySphereChunk(new Vector3(transform.position.x, transform.position.y, transform.position.z));
    }

    private void CalculateVelocity()
    {
        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;

        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        if ((velocity.z > 0 && front) || (velocity.z < 0 && back))
            velocity.z = 0;
        if ((velocity.x > 0 && right) || (velocity.x < 0 && left))
            velocity.x = 0;

        if (velocity.y < 0)
            velocity.y = checkDownSpeed(velocity.y);
        else if (velocity.y > 0)
            velocity.y = checkUpSpeed(velocity.y);
    }

    private float checkDownSpeed(float downSpeed)
    {
        if (
            world.CheckForVoxel(new Vector3(transform.position.x - 1, transform.position.y + downSpeed, transform.position.z - 1)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + 1, transform.position.y + downSpeed, transform.position.z - 1)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + 1, transform.position.y + downSpeed, transform.position.z + 1)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - 1, transform.position.y + downSpeed, transform.position.z + 1))
           ) {
            isGrounded = true;
            return 0;
        } else {
            isGrounded = false;
            return downSpeed;
        }
    }

    private float checkUpSpeed(float upSpeed)
    {
        if (
            world.CheckForVoxel(new Vector3(transform.position.x - 1, transform.position.y + 2f + upSpeed, transform.position.z - 1)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + 1, transform.position.y + 2f + upSpeed, transform.position.z - 1)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + 1, transform.position.y + 2f + upSpeed, transform.position.z + 1)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - 1, transform.position.y + 2f + upSpeed, transform.position.z + 1))
           ) {
            return 0;
        } else {
            return upSpeed - 0.06f;
        }

    }

    public bool front
    {
        get {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + 1)) ||
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + 1))
                )
                return true;
            else
                return false;
        }
    }

    public bool back
    {
        get {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - 1)) ||
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - 1))
                )
                return true;
            else
                return false;
        }
    }

    public bool left
    {
        get {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z)) ||
                world.CheckForVoxel(new Vector3(transform.position.x - 1, transform.position.y + 1f, transform.position.z))
                )
                return true;
            else
                return false;
        }
    }

    public bool right
    {
        get {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x + 1, transform.position.y, transform.position.z)) ||
                world.CheckForVoxel(new Vector3(transform.position.x + 1, transform.position.y + 1f, transform.position.z))
                )
                return true;
            else
                return false;
        }
    }
}
