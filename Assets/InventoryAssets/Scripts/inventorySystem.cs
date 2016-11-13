using UnityEngine;
using System.Collections;

[System.Serializable]
public class Slot{
    public string name;
    public Texture2D texture;
    
}

[System.Serializable]
public class Inventory
{
    [Tooltip("The key you press to open the inventory")]
    public KeyCode inventoryKey;
    [Tooltip("The key you press to organize the inventory")]
    public KeyCode organizeKey;
    [Tooltip("Size of the inventory squares")]
    public int iconSize = 0;
    [Space(5)]
    [Tooltip("Texture for selected slot in the HotBar")]
    public Texture2D selectedTexture;
    [Tooltip("Texture for any selected slot")]
    public Texture2D highlightedTexture;
    [Space(5)]
    public backpack Backpack;
    [Space(5)]
    public holster HotBar;

}

[System.Serializable]
public class backpack {
    [Tooltip("Max width of the slots in the inventory")]
    public int maxWidth;
    [Tooltip("Texture for the background of the inventory")]
    public Texture2D backpack_Backdrop;
    [Space(5)]
    public Slot[] slot;
}

[System.Serializable]
public class holster {
    [Tooltip("If true, does not allow you to move the hotbar around")]
    public bool HotBar_Static;
    [Tooltip("changes the offset of the position of the HotBar")]
    public Vector2 HotBar_Offset;
    [Space(5)]
    [Tooltip("keep the size between(1-9)")]
    public Slot[] curr_HotBar = new Slot[9];
    [Space(5)]
    [Tooltip("Textures for the hotbar slots")]
    public Texture2D[] HotBar_Textures = new Texture2D[9];
}

[System.Serializable]
public class Armour {
    [Tooltip("If true does not allow you to move the armour window around")]
    public bool armourStatic;
    [Tooltip("changes the offset of the armour")]
    public Vector2 armourOffset;
    [Space(5)]
    [Tooltip("The current armour")]
    public Slot[] curr_Armour = new Slot[3];
    [Space(5)]
    [Tooltip("Textures for the armour slots")]
    public Texture2D[] armourTextures = new Texture2D[3];
}

[System.Serializable]
public class statSpawn {
    [Tooltip("If true will respawn you where you started")]
    public bool respawnWherePlaced = false;
    [Tooltip("If true removes all items when you die")]
    public bool removeItemsOnDeath = false;
    [Tooltip("does same thing as removeItemsOnDeath, and also drops all items.")]
    public bool dropAllItemsOnDeath = false;
    [Tooltip("If spawnedWherePlaced is false, you can manually set up the spawn point")]
    public Vector3 spawnPoint;
}

[System.Serializable]
public class statImg {
    public Texture2D HealthBar;
    public Texture2D thirstBar;
    public Texture2D hungerBar;
    [Tooltip("This is the image that goes behind the stat bars")]
    public Texture2D statBackDrop;
}

[System.Serializable]
public class Stats {
    [Tooltip("(TR)-TopRight\n(TL)-TopLeft\n(BL)-BottomLeft\n(BR)-BottomRight")]
    public string statsPosition;
    [Space(10)]
    public statSpawn SpawnInformation;
    [Space(5)]
    public statImg Images;
    [Space(10)]
    [Tooltip("If true will deplete stats every depleteTime")]
    public bool autoDeplete = false;
    public float depleteTime;
    [Space(10)]
    [Tooltip("If true will place labels above the stats to show its percentage")]
    public bool statsLabels = false;
    [Space(10)]
    public int health;
    [Tooltip("If autoDeplete is true health will be decreased by healthDecay every depleteTime")]
    public int healthDecay;
    [Space(5)]
    public int hunger;
    [Tooltip("If autoDeplete is true hunger will be decreased by hungerDecay every depleteTime")]
    public int hungerDecay;
    [Space(5)]
    public int thirst;
    [Tooltip("If autoDeplete is true thirst will be decreased by thirstDecay every depleteTime")]
    public int thirstDecay;
}

[System.Serializable]
public class Items {
    public string name;
    [Tooltip("Required for armour\n(helmet)\n(chest)\n(boots)")]
    public string subType;
    [Tooltip("If it is a weapon, set its damage here, if not, leave blank")]
    public int damage;
    [Tooltip("For armour, set how many health points it protects, if not, leave blank")]
    public int protection;
    [Tooltip("Texture for the item")]
    public Texture2D texture;
    [Tooltip("The prefab that will be dropped.")]
    public GameObject prefabDrop;
    [Tooltip("The physical in game object")]
    public Renderer inGameObject;

}


public class inventorySystem : MonoBehaviour {
    [Tooltip("The Custom GUI skin for the inventory")]
    public GUISkin BackPackSkin;
    [Space(5)]
    [Tooltip("The Location where items will spawn")]
    public Transform dropLocation;

    [Header("Inventory")]
    public Inventory inventory;
    //[Space(20)]
    [Header("Armour")]
    public bool includeArmour;
    public Armour armour;
    //[Space(20)]
    [Header("Stats")]
    public bool includeStats;
    public Stats stats;
    //[Space(20)]
    [Header("Items")]
    public KeyCode pickUpKey;
    public Items[] items;


    //private Variables
    private int selectedSlot = 1;
    [HideInInspector]
    public bool screenLook;
    private bool holster3x3;
    private float timeLeft;
    int inventoryWidth;
    int inventoryHeight;
    [HideInInspector]
    public int itemCount;
    [HideInInspector]
    public int holsterCount;
    [HideInInspector]
    public int slotCount;
    [HideInInspector]
    public bool moveItem = false;
    private string itemName;
    private Texture2D itemIcon;
    [HideInInspector]
    public int tempSlot = 555;
    private int typeOfMove = 0;
    private string name;
    private string type;
    private Texture2D texture;
    [HideInInspector]
    public GameObject prefabDrop;
    private GameObject prefabItem;
    private bool itemChange = false;
    private int statSizeX = 200;
    private int statSizeY = 25;
    private int healthSizeX = 200;
    private int healthSizeY = 25;
    private int hungerSizeX = 200;
    private int hungerSizeY = 25;
    private int thirstSizeX = 200;
    private int thirstSizeY = 25;
    private int statPosX;
    private int statPosY;
    private Rect holsterR;
    private Rect armourR;
    private Texture2D tempTexture;
    private GameObject tempPrefab;

    //important variables
    private int armourLevel;
    private int damageLevel;
    //important variables

    void Start()
    {

        timeLeft = stats.depleteTime;

        if (stats.SpawnInformation.respawnWherePlaced)
        {
            stats.SpawnInformation.spawnPoint = transform.position;
        }


        if (stats.statsPosition == "TL")
        {
            statPosX = 0;
            statPosY = 0;
        }
        else if (stats.statsPosition == "TR")
        {
            statPosX = Screen.width - statSizeX;
            statPosY = 0;
        }
        else if (stats.statsPosition == "BL")
        {
            statPosX = 0;
            statPosY = Screen.height - statSizeY * 3;
        }
        else if (stats.statsPosition == "BR")
        {
            statPosX = Screen.width - statSizeX;
            statPosY = Screen.height - statSizeY * 3;
        }

        stats.health *= 2;
        stats.hunger *= 2;
        stats.thirst *= 2;
        itemCount = items.Length;
        slotCount = inventory.Backpack.slot.Length;
        holsterCount = inventory.HotBar.curr_HotBar.Length;
        if (holster3x3 == true)
        {
            holsterR = new Rect((Screen.width / 2) - ((inventory.iconSize * 3) / 2) + inventory.HotBar.HotBar_Offset.x, (Screen.height / 2) + inventory.HotBar.HotBar_Offset.y + 200, (inventory.iconSize * 3), (inventory.iconSize * 3) + 10);
        }
        else
        {
            holsterR = new Rect((Screen.width / 2) - ((inventory.iconSize * holsterCount) / 2) + inventory.HotBar.HotBar_Offset.x, (Screen.height / 2) + inventory.HotBar.HotBar_Offset.y + 300, inventory.iconSize * holsterCount, inventory.iconSize + 10);
        }
        armourR = new Rect((Screen.width - (inventory.iconSize)) + armour.armourOffset.x, (Screen.height - (inventory.iconSize * 3 + 10)) + armour.armourOffset.y, inventory.iconSize, inventory.iconSize * 3 + 10);

        getSize();
        Cursor.visible = false;
        screenLook = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnGUI()
    {

        checkHealth();

        GUI.skin = BackPackSkin;



        if (screenLook == false)
        {
            GUI.DrawTexture(new Rect((Screen.width - inventoryWidth) / 2 - inventory.iconSize / 4, (Screen.height - inventoryHeight) / 2 - inventory.iconSize / 4, inventoryWidth + inventory.iconSize / 2, inventoryHeight - inventory.iconSize / 2), inventory.Backpack.backpack_Backdrop);
            drawInven();
            if (includeArmour)
            {
                armourR = stayInScreen(GUI.Window(1, armourR, armourWindow, ""));
            }

        }

        holsterR = stayInScreen(GUI.Window(0, holsterR, holsterWindow, ""));
        if (includeStats)
        {
            statsBarFunction();
        }



    }

    Rect stayInScreen(Rect r)
    {
        r.x = Mathf.Clamp(r.x, 0, Screen.width - r.width);
        r.y = Mathf.Clamp(r.y, 0, Screen.height - r.height);
        return r;
    }

    void Update()
    {

        dropIt();
        itemSetup();
        slotChange();
        toggleInventory();

        if (stats.autoDeplete)
        {
            deplete();
        }
        if (screenLook == false && Input.GetKeyDown(inventory.organizeKey)) {
            moveItem = false;
            tempSlot = 555;
            organize();
        }

        getArmourLevel();

       


    }

    void getArmourLevel() {
        armourLevel = 0;
        for (int i = 0; i < itemCount; i++) {
            if (items[i].name == armour.curr_Armour[0].name) {
                armourLevel += items[i].protection;
            }
            if (items[i].name == armour.curr_Armour[1].name)
            {
                armourLevel += items[i].protection;
            }
            if (items[i].name == armour.curr_Armour[2].name)
            {
                armourLevel += items[i].protection;
            }
        }
    }

    void organize() {

        int[] numItems;

        numItems = new int[slotCount];

        for (int i = 0; i < slotCount; i++) {
            numItems[i] = 0;
        }

        for (int j = 0; j < slotCount; j++) {

            if (inventory.Backpack.slot[j].name != "") {

                for (int i = 0; i < itemCount; i++) {

                    if (inventory.Backpack.slot[j].name == items[i].name) {

                        numItems[i] += 1;
                        break;

                    }
                }
            }          

        }
        clearBackPack();
        int itemNum = 0;
        for (int i = 0; i < slotCount;) {
            if (numItems[itemNum] > 0)
            {
                inventory.Backpack.slot[i].name = items[itemNum].name;
                inventory.Backpack.slot[i].texture = items[itemNum].texture;
                numItems[itemNum] -= 1;
                i++;
            }
            else {
                itemNum += 1;
                if (itemNum > itemCount) {
                    break;
                }
            }
        }  
             
    }

    void deplete()
    {

        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            timeLeft = stats.depleteTime;
            stats.hunger -= stats.hungerDecay;
            stats.thirst -= stats.thirstDecay;
            hungerSizeX -= stats.hungerDecay;
            thirstSizeX -= stats.thirstDecay;
            if (stats.hunger <= 0)
            {
                stats.health -= stats.healthDecay;
                healthSizeX -= stats.healthDecay;
            }
            if (stats.thirst <= 0)
            {
                stats.health -= stats.healthDecay;
                healthSizeX -= stats.healthDecay;
            }
        }


    }

    void statsBarFunction()
    {
        GUI.DrawTexture(new Rect(statPosX, statPosY, 200, 25), stats.Images.statBackDrop);
        GUI.DrawTexture(new Rect(statPosX, statPosY, healthSizeX, healthSizeY), stats.Images.HealthBar);

        GUI.DrawTexture(new Rect(statPosX, statPosY + statSizeY, 200, 25), stats.Images.statBackDrop);
        GUI.DrawTexture(new Rect(statPosX, statPosY + statSizeY, hungerSizeX, hungerSizeY), stats.Images.hungerBar);

        GUI.DrawTexture(new Rect(statPosX, statPosY + 2 * statSizeY, 200, 25), stats.Images.statBackDrop);
        GUI.DrawTexture(new Rect(statPosX, statPosY + 2 * statSizeY, thirstSizeX, thirstSizeY), stats.Images.thirstBar);

        if (stats.statsLabels)
        {
            GUI.Label(new Rect(statPosX + 91, statPosY + 3, 200, 25), "" + stats.health / 2);
            GUI.Label(new Rect(statPosX + 91, statPosY + statSizeY + 3, 200, 25), "" + stats.hunger / 2);
            GUI.Label(new Rect(statPosX + 91, statPosY + 2 * statSizeY + 3, 200, 25), "" + stats.thirst / 2);
        }


    }

    void checkHealth()
    {

        if (stats.health == 0)
        {

            if (stats.SpawnInformation.dropAllItemsOnDeath)
            {
                dropAllItems();
                clearInven();
            }

            if (stats.SpawnInformation.removeItemsOnDeath)
            {
                clearInven();
            }
            stats.health = 200;
            healthSizeX = 200;

            stats.hunger = 200;
            hungerSizeX = 200;

            stats.thirst = 200;
            thirstSizeX = 200;

            transform.position = stats.SpawnInformation.spawnPoint;
        }

    }

    void clearInven()
    {
        for (int i = 0; i < slotCount; i++)
        {
            inventory.Backpack.slot[i].name = "";
            inventory.Backpack.slot[i].texture = null;
        }
        for (int i = 0; i < holsterCount; i++)
        {
            inventory.HotBar.curr_HotBar[i].name = "";
            inventory.HotBar.curr_HotBar[i].texture = null;
        }
        for (int i = 0; i < 3; i++)
        {
            armour.curr_Armour[i].name = "";
            armour.curr_Armour[i].texture = null;
        }
    }

    void clearBackPack() {

        for (int i = 0; i < slotCount; i++)
        {
            inventory.Backpack.slot[i].name = "";
            inventory.Backpack.slot[i].texture = null;
        }

    }

    void dropIt()
    {
        if (moveItem == true && Input.GetMouseButtonDown(1))
        {
            for (int i = 0; i < itemCount; i++)
            {
                if (items[i].name == itemName)
                {
                    prefabDrop = items[i].prefabDrop;
                    Instantiate(prefabDrop, dropLocation.position, dropLocation.rotation);
                    moveItem = false;
                    break;
                }
            }

            if (typeOfMove == 0)
            {
                inventory.HotBar.curr_HotBar[tempSlot].name = "";
                inventory.HotBar.curr_HotBar[tempSlot].texture = null;
            }
            else if (typeOfMove == 1)
            {
                inventory.Backpack.slot[tempSlot].name = "";
                inventory.Backpack.slot[tempSlot].texture = null;
            }

            //tempSlot = 555;
            //prefabDrop = null;

        }
    }

    void toggleInventory()
    {
        if (Input.GetKeyDown(inventory.inventoryKey))
        {
            moveItem = false;
            tempSlot = 555;
            screenLook = !screenLook;
            Cursor.visible = !Cursor.visible;
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

        }
    }

    void FixedUpdate()
    {
        resetWeapon();
    }

    Texture2D changeTexture(int slot)
    {
        if (armour.curr_Armour[slot].name == "")
        {
            return armour.armourTextures[slot];
        }
        else
        {
            return armour.curr_Armour[slot].texture;
        }
    }

    void armourWindow(int windowID)
    {
        Texture2D tempTexture;
        bool fSlot = false;
        int iSlot = 0;

        if (armour.curr_Armour[0].name == "")
        {
            tempTexture = armour.armourTextures[0];
        }
        else
        {
            tempTexture = armour.curr_Armour[0].texture;
        }

        if (GUI.Button(new Rect(0, 10, inventory.iconSize, inventory.iconSize), tempTexture))
        {
            iSlot = findItemSlot(itemName);
            if (moveItem == true && armour.curr_Armour[0].name == "" && items[iSlot].subType == "helmet")
            {

                if (typeOfMove == 0)
                {
                    inventory.HotBar.curr_HotBar[tempSlot].name = "";
                    inventory.HotBar.curr_HotBar[tempSlot].texture = null;
                }
                else if (typeOfMove == 1)
                {
                    inventory.Backpack.slot[tempSlot].name = "";
                    inventory.Backpack.slot[tempSlot].texture = null;
                }





                armour.curr_Armour[0].name = itemName;
                armour.curr_Armour[0].texture = itemIcon;
                tempSlot = 555;
                moveItem = false;
            }
            else if (moveItem == false)
            {

                for (int i = 0; i < slotCount; i++)
                {
                    if (inventory.Backpack.slot[i].name == "")
                    {
                        fSlot = true;
                        inventory.Backpack.slot[i].name = armour.curr_Armour[0].name;
                        inventory.Backpack.slot[i].texture = armour.curr_Armour[0].texture;
                        armour.curr_Armour[0].name = "";
                        armour.curr_Armour[0].texture = null;
                        break;
                    }
                }
                if (fSlot == false)
                {
                    for (int i = 0; i < holsterCount; i++)
                    {
                        if (inventory.HotBar.curr_HotBar[i].name == "")
                        {
                            fSlot = true;
                            inventory.HotBar.curr_HotBar[i].name = armour.curr_Armour[0].name;
                            inventory.HotBar.curr_HotBar[i].texture = armour.curr_Armour[0].texture;
                            armour.curr_Armour[0].name = "";
                            armour.curr_Armour[0].texture = null;
                            break;
                        }
                    }
                }

            }

        }

        if (armour.curr_Armour[1].name == "")
        {
            tempTexture = armour.armourTextures[1];
        }
        else
        {
            tempTexture = armour.curr_Armour[1].texture;
        }

        if (GUI.Button(new Rect(0, 10 + inventory.iconSize, inventory.iconSize, inventory.iconSize), tempTexture))
        {
            iSlot = findItemSlot(itemName);
            if (moveItem == true && armour.curr_Armour[1].name == "" && items[iSlot].subType == "chest")
            {

                if (typeOfMove == 0)
                {
                    inventory.HotBar.curr_HotBar[tempSlot].name = "";
                    inventory.HotBar.curr_HotBar[tempSlot].texture = null;
                }
                else if (typeOfMove == 1)
                {
                    inventory.Backpack.slot[tempSlot].name = "";
                    inventory.Backpack.slot[tempSlot].texture = null;
                }





                armour.curr_Armour[1].name = itemName;
                armour.curr_Armour[1].texture = itemIcon;
                tempSlot = 555;
                moveItem = false;
            }
            else if (moveItem == false)
            {

                for (int i = 0; i < slotCount; i++)
                {
                    if (inventory.Backpack.slot[i].name == "")
                    {
                        fSlot = true;
                        inventory.Backpack.slot[i].name = armour.curr_Armour[1].name;
                        inventory.Backpack.slot[i].texture = armour.curr_Armour[1].texture;
                        armour.curr_Armour[1].name = "";
                        armour.curr_Armour[1].texture = null;
                        break;
                    }
                }
                if (fSlot == false)
                {
                    for (int i = 0; i < holsterCount; i++)
                    {
                        if (inventory.HotBar.curr_HotBar[i].name == "")
                        {
                            fSlot = true;
                            inventory.HotBar.curr_HotBar[i].name = armour.curr_Armour[1].name;
                            inventory.HotBar.curr_HotBar[i].texture = armour.curr_Armour[1].texture;
                            armour.curr_Armour[1].name = "";
                            armour.curr_Armour[1].texture = null;
                            break;
                        }
                    }
                }

            }

        }

        if (armour.curr_Armour[2].name == "")
        {
            tempTexture = armour.armourTextures[2];
        }
        else
        {
            tempTexture = armour.curr_Armour[2].texture;
        }

        if (GUI.Button(new Rect(0, 10 + 2 * inventory.iconSize, inventory.iconSize, inventory.iconSize), tempTexture))
        {
            iSlot = findItemSlot(itemName);
            if (moveItem == true && armour.curr_Armour[2].name == "" && items[iSlot].subType == "boots")
            {

                if (typeOfMove == 0)
                {
                    inventory.HotBar.curr_HotBar[tempSlot].name = "";
                    inventory.HotBar.curr_HotBar[tempSlot].texture = null;
                }
                else if (typeOfMove == 1)
                {
                    inventory.Backpack.slot[tempSlot].name = "";
                    inventory.Backpack.slot[tempSlot].texture = null;
                }





                armour.curr_Armour[2].name = itemName;
                armour.curr_Armour[2].texture = itemIcon;
                tempSlot = 555;
                moveItem = false;
            }
            else if (moveItem == false)
            {

                for (int i = 0; i < slotCount; i++)
                {
                    if (inventory.Backpack.slot[i].name == "")
                    {
                        fSlot = true;
                        inventory.Backpack.slot[i].name = armour.curr_Armour[2].name;
                        inventory.Backpack.slot[i].texture = armour.curr_Armour[2].texture;
                        armour.curr_Armour[2].name = "";
                        armour.curr_Armour[2].texture = null;
                        break;
                    }
                }
                if (fSlot == false)
                {
                    for (int i = 0; i < holsterCount; i++)
                    {
                        if (inventory.HotBar.curr_HotBar[i].name == "")
                        {
                            fSlot = true;
                            inventory.HotBar.curr_HotBar[i].name = armour.curr_Armour[2].name;
                            inventory.HotBar.curr_HotBar[i].texture = armour.curr_Armour[2].texture;
                            armour.curr_Armour[2].name = "";
                            armour.curr_Armour[2].texture = null;
                            break;
                        }
                    }
                }

            }

        }

        if (!armour.armourStatic)
        {
            GUI.DragWindow();
        }

    }

    void holsterWindow(int windowID)
    {
        int image = 0;
        if (holster3x3 == true)
        {

            for (int i = 0; i < holsterCount / 3; i++)
            {
                for (int j = 0; j < holsterCount / 3; j++)
                {
                    if (inventory.HotBar.curr_HotBar[image].name == "")
                    {
                        tempTexture = inventory.HotBar.HotBar_Textures[image];
                    }
                    else
                    {
                        tempTexture = inventory.HotBar.curr_HotBar[image].texture;
                    }
                    if (GUI.Button(new Rect((0) + (inventory.iconSize * j), (10) + (inventory.iconSize * i), inventory.iconSize, inventory.iconSize), tempTexture) && Event.current.button == 0)
                    {
                        if (moveItem == true)
                        {

                            if (typeOfMove == 0)
                            {
                                inventory.HotBar.curr_HotBar[tempSlot].name = inventory.HotBar.curr_HotBar[image].name;
                                inventory.HotBar.curr_HotBar[tempSlot].texture = inventory.HotBar.curr_HotBar[image].texture;
                            }
                            else if (typeOfMove == 1)
                            {
                                inventory.Backpack.slot[tempSlot].name = inventory.HotBar.curr_HotBar[image].name;
                                inventory.Backpack.slot[tempSlot].texture = inventory.HotBar.curr_HotBar[image].texture;
                            }





                            inventory.HotBar.curr_HotBar[image].name = itemName;
                            inventory.HotBar.curr_HotBar[image].texture = itemIcon;
                            tempSlot = 555;
                            moveItem = false;
                        }
                        else
                        {
                            itemName = inventory.HotBar.curr_HotBar[image].name;
                            itemIcon = inventory.HotBar.curr_HotBar[image].texture;
                            moveItem = true;
                            tempSlot = image;
                            typeOfMove = 0;
                        }
                    }
                    if (image + 1 == selectedSlot)
                    {
                        GUI.DrawTexture(new Rect((0) + (inventory.iconSize * j), (10) + (inventory.iconSize * i), inventory.iconSize, inventory.iconSize), inventory.selectedTexture);
                    }
                    if (image == tempSlot && typeOfMove == 0)
                    {
                        GUI.DrawTexture(new Rect((0) + (inventory.iconSize * j), (10) + (inventory.iconSize * i), inventory.iconSize, inventory.iconSize), inventory.highlightedTexture);
                    }


                    image++;
                }

            }
        }
        else
        {
            for (int i = 0; i < holsterCount; i++)
            {
                if (inventory.HotBar.curr_HotBar[i].name == "")
                {
                    tempTexture = inventory.HotBar.HotBar_Textures[i];
                }
                else
                {
                    tempTexture = inventory.HotBar.curr_HotBar[i].texture;
                }
                if (GUI.Button(new Rect(inventory.iconSize * i, inventory.iconSize - (inventory.iconSize - 10), inventory.iconSize, inventory.iconSize), tempTexture) && Event.current.button == 0)
                {
                    if (moveItem == true)
                    {
                        if (typeOfMove == 0)
                        {
                            inventory.HotBar.curr_HotBar[tempSlot].name = inventory.HotBar.curr_HotBar[i].name;
                            inventory.HotBar.curr_HotBar[tempSlot].texture = inventory.HotBar.curr_HotBar[i].texture;
                        }
                        else if (typeOfMove == 1)
                        {
                            inventory.Backpack.slot[tempSlot].name = inventory.HotBar.curr_HotBar[i].name;
                            inventory.Backpack.slot[tempSlot].texture = inventory.HotBar.curr_HotBar[i].texture;
                        }



                        inventory.HotBar.curr_HotBar[i].name = itemName;
                        inventory.HotBar.curr_HotBar[i].texture = itemIcon;
                        tempSlot = 555;
                        moveItem = false;
                    }
                    else
                    {
                        itemName = inventory.HotBar.curr_HotBar[i].name;
                        itemIcon = inventory.HotBar.curr_HotBar[i].texture;
                        moveItem = true;
                        tempSlot = i;
                        typeOfMove = 0;
                    }
                }
                if (i + 1 == selectedSlot)
                {
                    GUI.DrawTexture(new Rect(inventory.iconSize * i, inventory.iconSize - (inventory.iconSize - 10), inventory.iconSize, inventory.iconSize), inventory.selectedTexture);
                }
                if (i == tempSlot && typeOfMove == 0)
                {
                    GUI.DrawTexture(new Rect(inventory.iconSize * i, inventory.iconSize - (inventory.iconSize - 10), inventory.iconSize, inventory.iconSize), inventory.highlightedTexture);
                }
            }

        }
        if (screenLook == false && inventory.HotBar.HotBar_Static == false)
        {
            GUI.DragWindow();
        }


    }

    void drawInven()
    {
        int horiz = slotCount;
        int newHoriz = 0;
        int chSlot;
        //GUI.Box(new Rect((Screen.width - inventoryWidth) / 2, (Screen.height - inventoryHeight) / 2, inventoryWidth, inventoryHeight), "");
        if (slotCount >= inventory.Backpack.maxWidth)
        {
            for (int i = 0; i < ((slotCount / inventory.Backpack.maxWidth) + 1); i++)
            {
                if (horiz <= inventory.Backpack.maxWidth)
                {
                    newHoriz = horiz;
                }
                else
                {
                    newHoriz = inventory.Backpack.maxWidth;
                }
                horiz -= inventory.Backpack.maxWidth;
                for (int j = 0; j < newHoriz; j++)
                {
                    chSlot = (i * inventory.Backpack.maxWidth) + j;
                    if (GUI.Button(new Rect(((Screen.width - inventoryWidth) / 2) + (inventory.iconSize * j), ((Screen.height - inventoryHeight) / 2) + (inventory.iconSize * i), inventory.iconSize, inventory.iconSize), inventory.Backpack.slot[chSlot].texture) && Event.current.button == 0)
                    {
                        if (moveItem == true)
                        {

                            if (typeOfMove == 1)
                            {
                                inventory.Backpack.slot[tempSlot].name = inventory.Backpack.slot[chSlot].name;
                                inventory.Backpack.slot[tempSlot].texture = inventory.Backpack.slot[chSlot].texture;
                            }
                            else if (typeOfMove == 0)
                            {
                                inventory.HotBar.curr_HotBar[tempSlot].name = inventory.Backpack.slot[chSlot].name;
                                inventory.HotBar.curr_HotBar[tempSlot].texture = inventory.Backpack.slot[chSlot].texture;
                            }


                            inventory.Backpack.slot[chSlot].name = itemName;
                            inventory.Backpack.slot[chSlot].texture = itemIcon;
                            tempSlot = 555;
                            moveItem = false;
                        }
                        else
                        {
                            itemName = inventory.Backpack.slot[chSlot].name;
                            itemIcon = inventory.Backpack.slot[chSlot].texture;
                            moveItem = true;
                            tempSlot = chSlot;
                            typeOfMove = 1;
                        }
                    }
                    if (chSlot == tempSlot && typeOfMove == 1)
                    {
                        GUI.DrawTexture(new Rect(((Screen.width - inventoryWidth) / 2) + (inventory.iconSize * j), ((Screen.height - inventoryHeight) / 2) + (inventory.iconSize * i), inventory.iconSize, inventory.iconSize), inventory.highlightedTexture);
                    }

                }
            }
        }
        else if (slotCount < inventory.Backpack.maxWidth)
        {
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < slotCount; j++)
                {
                    if (GUI.Button(new Rect(((Screen.width - inventoryWidth) / 2) + (inventory.iconSize * j), ((Screen.height - inventoryHeight) / 2) + (inventory.iconSize * i), inventory.iconSize, inventory.iconSize), inventory.Backpack.slot[j].texture) && Event.current.button == 0)
                    {
                        if (moveItem == true)
                        {
                            if (typeOfMove == 1)
                            {
                                inventory.Backpack.slot[tempSlot].name = inventory.Backpack.slot[j].name;
                                inventory.Backpack.slot[tempSlot].texture = inventory.Backpack.slot[j].texture;
                            }
                            else if (typeOfMove == 0)
                            {
                                inventory.HotBar.curr_HotBar[tempSlot].name = inventory.Backpack.slot[j].name;
                                inventory.HotBar.curr_HotBar[tempSlot].texture = inventory.Backpack.slot[j].texture;
                            }


                            inventory.Backpack.slot[j].name = itemName;
                            inventory.Backpack.slot[j].texture = itemIcon;
                            tempSlot = 555;
                            moveItem = false;
                        }
                        else
                        {
                            itemName = inventory.Backpack.slot[j].name;
                            itemIcon = inventory.Backpack.slot[j].texture;
                            moveItem = true;
                            tempSlot = j;
                            typeOfMove = 1;
                        }
                    }
                    if (j == tempSlot && typeOfMove == 1)
                    {
                        GUI.DrawTexture(new Rect(((Screen.width - inventoryWidth) / 2) + (inventory.iconSize * j), ((Screen.height - inventoryHeight) / 2) + (inventory.iconSize * i), inventory.iconSize, inventory.iconSize), inventory.highlightedTexture);
                    }


                }
            }
        }

    }

    void getSize()
    {

        if (slotCount >= inventory.Backpack.maxWidth)
        {
            inventoryWidth = (inventory.Backpack.maxWidth * inventory.iconSize);
        }
        else if (slotCount < inventory.Backpack.maxWidth)
        {
            inventoryWidth = (slotCount * inventory.iconSize);
        }
        if (slotCount > inventory.Backpack.maxWidth)
        {
            inventoryHeight = (((slotCount / inventory.Backpack.maxWidth) + 1) * inventory.iconSize);


        }
        else if (slotCount <= inventory.Backpack.maxWidth)
        {
            inventoryHeight = (inventory.iconSize);
        }
    }

    void resetWeapon()
    {
        name = inventory.HotBar.curr_HotBar[selectedSlot - 1].name;
        for (int i = 0; i < itemCount; i++)
        {
            items[i].inGameObject.enabled = false;
            //disable other weapons
        }
        for (int i = 0; i < itemCount; i++)
        {
            if (items[i].name == name)
            {
                items[i].inGameObject.enabled = true;
                //enable one weapon
                break;
            }
        }
    }

    void slotChange()
    {
        //holsterCount
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedSlot = 1;
            resetWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (holsterCount >= 2)
            {
                selectedSlot = 2;
                resetWeapon();
            }

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (holsterCount >= 3)
            {
                selectedSlot = 3;
                resetWeapon();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (holsterCount >= 4)
            {
                selectedSlot = 4;
                resetWeapon();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (holsterCount >= 5)
            {
                selectedSlot = 5;
                resetWeapon();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (holsterCount >= 6)
            {
                selectedSlot = 6;
                resetWeapon();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (holsterCount >= 7)
            {
                selectedSlot = 7;
                resetWeapon();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            if (holsterCount >= 8)
            {
                selectedSlot = 8;
                resetWeapon();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (holsterCount >= 9)
            {
                selectedSlot = 9;
                resetWeapon();
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            if (selectedSlot != 1) {
                selectedSlot -= 1;
                resetWeapon();
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (selectedSlot != 9)
            {
                selectedSlot += 1;
                resetWeapon();
            }
        }
        name = inventory.HotBar.curr_HotBar[selectedSlot - 1].name;
        for (int i = 0; i < itemCount; i++)
        {

            if (items[i].name == name)
            {
                
                damageLevel = items[i].damage;
                tempPrefab = items[i].prefabDrop;
            }
        }

    }

    int findItemSlot(string name)
    {
        int slotNum = 0;
        bool found = false;
        for (int i = 0; i < itemCount; i++)
        {
            if (name == items[i].name)
            {
                found = true;
                slotNum = i;
                break;
            }
        }
        return slotNum;

    }

    void dropItem()
    {

        Instantiate(tempPrefab, dropLocation.position, dropLocation.rotation);
        deleteItem();
    }

    void deleteItem()
    {
        inventory.HotBar.curr_HotBar[selectedSlot - 1].name = "";
        inventory.HotBar.curr_HotBar[selectedSlot - 1].texture = null;
    }

    void dropAllItems() {
        int itemSlot;
        for (int i = 0; i < slotCount; i++)
        {
            if (inventory.Backpack.slot[i].name != "") {
                itemSlot = findItemSlot(inventory.Backpack.slot[i].name);
                tempPrefab = items[itemSlot].prefabDrop;
                Instantiate(tempPrefab, dropLocation.position, dropLocation.rotation);
            }
        }
        for (int i = 0; i < holsterCount; i++)
        {
            if (inventory.HotBar.curr_HotBar[i].name != "") {

                itemSlot = findItemSlot(inventory.HotBar.curr_HotBar[i].name);
                tempPrefab = items[itemSlot].prefabDrop;
                Instantiate(tempPrefab, dropLocation.position, dropLocation.rotation);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (armour.curr_Armour[i].name != "")
            {

                itemSlot = findItemSlot(armour.curr_Armour[i].name);
                tempPrefab = items[itemSlot].prefabDrop;
                Instantiate(tempPrefab, dropLocation.position, dropLocation.rotation);
            }
        }
        tempPrefab = null;
    }

    void addHealth(int add)
    {
        stats.health += add * 2;
        healthSizeX += add * 2;
        if (healthSizeX > 200)
        {
            healthSizeX = 200;
        }
        if (stats.health > 200)
        {
            stats.health = 200;
        }
    }
    void addHunger(int add)
    {
        stats.hunger += add * 2;
        hungerSizeX += add * 2;
        if (hungerSizeX > 200)
        {
            hungerSizeX = 200;
        }
        if (stats.hunger > 200)
        {
            stats.hunger = 200;
        }
    }
    void addThirst(int add)
    {
        stats.thirst += add * 2;
        thirstSizeX += add * 2;
        if (thirstSizeX > 200)
        {
            thirstSizeX = 200;
        }
        if (stats.hunger > 200)
        {
            stats.hunger = 200;
        }
    }

    void subHealth(int sub)
    {
        stats.health -= sub * 2 - (armourLevel*2);
        healthSizeX -= sub * 2 - (armourLevel*2);
        if (healthSizeX < 0)
        {
            healthSizeX = 0;
        }
        if (stats.health < 0)
        {
            stats.health = 0;
        }
    }
    void subHunger(int sub)
    {
        stats.hunger -= sub * 2;
        hungerSizeX -= sub * 2;
        if (hungerSizeX < 0)
        {
            hungerSizeX = 0;
        }
        if (stats.hunger < 0)
        {
            stats.hunger = 0;
        }
    }
    void subThirst(int sub)
    {
        stats.thirst -= sub * 2;
        thirstSizeX -= sub * 2;
        if (thirstSizeX < 0)
        {
            thirstSizeX = 0;
        }
        if (stats.hunger < 0)
        {
            stats.hunger = 0;
        }
    }


    /***********SETUP ITEMS HERE***************/

    void itemSetup()
    {
        if (screenLook == true)
        {
            if (name == "flashlight")
            {
                if (Input.GetMouseButtonDown(0))
                {
                    //if you press the left mouse button

                }
                if (Input.GetMouseButtonDown(1))
                {
                    //if you press the right mouse button
                    
                }
                if (Input.GetMouseButtonDown(2))
                {
                    //if you press the middle mouse button
                    
                }


            }

            if (name == "helmet")
            {
                if (Input.GetMouseButtonDown(0))
                {
                    //if you press the left mouse button

                }
                if (Input.GetMouseButtonDown(1))
                {
                    //if you press the right mouse button
                    
                }
                if (Input.GetMouseButtonDown(2))
                {
                    //if you press the middle mouse button
                    
                }


            }
        }

    }
}
