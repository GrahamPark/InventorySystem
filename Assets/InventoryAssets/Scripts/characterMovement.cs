using UnityEngine;
using System.Collections;

public class characterMovement : MonoBehaviour {

    public float walkSpeed;
    public float runSpeed;
    private float moveSpeed;

    public int jumpForce;
    private bool onGround = false;


    private Rigidbody rb;

    inventorySystem inv;

	// Use this for initialization
	void Start () {
        Cursor.visible = false;
        rb = GetComponent <Rigidbody>();
        inv = GetComponent<inventorySystem>();
        

    }

    bool run;
    bool jump;
    bool jumpcon;

    float xAxis;
    float yAxis;

	
	// Update is called once per frame
	void Update () {

        run = Input.GetKey(KeyCode.LeftShift);
        jump = Input.GetKeyDown(KeyCode.Space);
        jumpcon = Input.GetKey(KeyCode.Space);

        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");

       

        //movement(run, xAxis, yAxis);

        jumping(jump);
        if (jump) {
            onGround = false;
        }
        //jetpack(jumpcon);
             
            if (Physics.Raycast(inv.transform.position, transform.TransformDirection(Vector3.down), 3)) {
                onGround = true;
               
            }

        
	}
    void FixedUpdate() {
       
        movement(run, xAxis, yAxis);
        // jumping(jump);
    }

    void movement(bool run,float xAxis,float yAxis) {
        if (run) {
            moveSpeed = runSpeed;
        }
        else {
            moveSpeed = walkSpeed;
        }
        if (xAxis > 0) {
            transform.Translate(Vector3.right * moveSpeed * xAxis * Time.deltaTime);
        }
        if (xAxis < 0) {
            transform.Translate(-Vector3.right * moveSpeed * -xAxis * Time.deltaTime);
        }
        if (yAxis > 0) {
            transform.Translate(Vector3.forward * moveSpeed * yAxis * Time.deltaTime);
        }
        if (yAxis < 0) {
            transform.Translate(-Vector3.forward * moveSpeed * -yAxis * Time.deltaTime);
        }
    }

    void jetpack(bool jumpcon) {
        if (jumpcon) {
            transform.Translate(Vector3.up * 50 * Time.deltaTime);
        }
    }

    void jumping(bool jump){
        if (jump && onGround) {
             onGround = false;
             rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
             
             
            
        }
    }

    

    
    
}
