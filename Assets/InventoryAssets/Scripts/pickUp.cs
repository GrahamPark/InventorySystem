using UnityEngine;
using System.Collections;


public class pickUp : MonoBehaviour {

    private inventorySystem inven;

    public string name;

    private bool inZone = false;

    private bool donePickup = false;

    private bool hold = false;

    private Rigidbody rb;
    private Rigidbody playerVel;
    private SphereCollider sph;
    private MeshCollider mch;

	void Start () {
        inven = GameObject.FindWithTag("Player").GetComponent<inventorySystem>();
        
        rb = GetComponent<Rigidbody>();
        playerVel = inven.GetComponent<Rigidbody>();
        rb.velocity = playerVel.velocity;
        
	}

	void Update () {

        

        bool pickUpPress = Input.GetKeyDown(inven.pickUpKey);
        

        if (inZone && pickUpPress) {
            inven.moveItem = false;
            inven.tempSlot = 555;
            inven.prefabDrop = null;
            onPickUp();
        }
        
      


	}
    
    void OnTriggerEnter(Collider col) {
        if (col.tag == "Player") { 
            inZone = true;
            
        }
        
    }

    void OnTriggerExit(Collider col) {
        if (col.tag == "Player") {
            inZone = false;
            
        }
    }

    void onPickUp() {
        int chosenSlot = 0;
        for (int j = 0; j < inven.slotCount; j++) {
            if (inven.inventory.Backpack.slot[j].name == "" && donePickup == false) {
                    donePickup = true;
                    chosenSlot = j;
                    break;
            }
            
        }
        if (donePickup == false) {
            for (int j = 0; j < inven.holsterCount; j++) {
                if (inven.inventory.HotBar.curr_HotBar[j].name == "" && donePickup == false) {
                    donePickup = true;
                    chosenSlot = j;
                    break;
                }
            }
        }
        if (donePickup == true) {
            inven.inventory.Backpack.slot[chosenSlot].name = name;
            for (int i = 0; i < inven.itemCount; i++) {
                if (inven.items[i].name == name) {
                    inven.inventory.Backpack.slot[chosenSlot].texture = inven.items[i].texture;
                    break;
                }
                
            }
            Destroy(this.gameObject);
        }
        

        
        
    }
}
