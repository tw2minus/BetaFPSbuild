using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMovement : MonoBehaviour
{
    public Animator anim;
    public Transform orientation;
    public float gravity = -9.81f;
    private Rigidbody rb;


    public CharacterController controller;
    //movement
    public float moveSpeed =5;

    //ground
    bool isGrounded;
    public Transform groundCheck;
    public float groundDistance =1f;
    public LayerMask LayerGround;

    //jump
    public float jumpHeight = 2f;

    //input
    float inputH, inputV;
    bool run;
    
    void Awake()
    {
        //anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        run = false;
    }

    //called every frame
    
    private void Update()
    {
        MyInput();
        CheckGround();
        Jumping();
        Run();
    }

    //basic input
    Vector3 velocity;
    private void MyInput()
    {
        inputH = Input.GetAxisRaw("Horizontal");
        inputV = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * inputH + transform.forward *inputV;

        anim.SetFloat("InputH", inputH);
        anim.SetFloat("InputV", inputV);

        controller.Move(move * moveSpeed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
    //Jumping
    private void Jumping()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -3f * gravity);

            anim.Play("BasicMotions@Jump01 - MidAir");
        }
      
    }
    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, LayerGround);
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    
    //Running
    private void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift)&& isGrounded)
        {
            run = true;
            Vector3 running = transform.right * inputH + transform.forward * inputV;
            controller.Move(running *8.0f*Time.deltaTime);
            controller.Move(velocity * Time.deltaTime);
            velocity.y += gravity * Time.deltaTime;
            
            anim.SetBool("Run", true);
        }
        else
        {
            run = false;
            anim.SetBool("Run", false);
        }
    }
    
}
