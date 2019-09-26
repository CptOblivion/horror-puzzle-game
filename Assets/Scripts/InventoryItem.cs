using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public bool Interact = true;
    public string ItemName = "Empty Item";
    public string ItemDescription = "No Description";
    public string UseOnNothing = "I can't use that here.";
    public string UseOnWrongThing = "I can't use this on that.";
}
