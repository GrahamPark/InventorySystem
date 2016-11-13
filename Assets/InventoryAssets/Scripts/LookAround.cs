using UnityEngine;
using System.Collections;

public class LookAround : MonoBehaviour {


    float mouseX;
    float mouseY;

    private float rotationX = 0f;
    private float sensX = 2f;

    public float mouseSen;

    public GameObject cam;

    [HideInInspector]
    public inventorySystem inventorySystem;


    void Start () {
        Cursor.visible = false;
    }
	

	void Update () {

        inventorySystem = gameObject.GetComponent<inventorySystem>();

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        if (inventorySystem.screenLook == true)
            mouseLook(mouseX, mouseY);

    }

    void mouseLook(float mouseX, float mouseY)
    {

        rotationX += mouseY * sensX;
        rotationX = Mathf.Clamp(rotationX, -90, 90);

        if (mouseX > 0)
        {
            transform.Rotate(Vector3.up * mouseSen * mouseX);
        }
        if (mouseX < 0)
        {
            transform.Rotate(-Vector3.up * mouseSen * -mouseX);
        }
        cam.transform.localEulerAngles = new Vector3(-rotationX, cam.transform.localEulerAngles.y, cam.transform.localEulerAngles.z);
    }
}
