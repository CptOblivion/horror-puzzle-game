using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToMove : MonoBehaviour
{
  public LayerMask ClickMask;
  public float height = 0;
  public bool continuous = false;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if ((Input.GetMouseButtonDown(0)) || (Input.GetMouseButton(0) && continuous))
    {
      Vector3 LastPos = transform.position;
      //Ray ray = rayCam.ScreenPointToRay(Input.mousePosition);
      Vector3 MousePosition = Input.mousePosition;
      MousePosition.x = MousePosition.x / Screen.width;
      MousePosition.y = MousePosition.y / Screen.height;
      Ray ray = GlobalTools.currentCam.ViewportPointToRay(MousePosition);
      RaycastHit hit;
      if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, ClickMask))
      {
        GameObject hitOb = hit.collider.gameObject;
        if (hitOb.layer == 10) //if clickable
        {
          transform.position = hit.point + new Vector3(0, height, 0);
          if (transform.position != LastPos)
          {
            LastPos.y = transform.position.y;
            transform.LookAt(LastPos);
            transform.Rotate(0, 180, 0);
          }
        }
      }
      //cast ray (layermask: clickable and clickocclude)
      //if hit object is on clickable (but not clickocclude) we can walk there
    }
  }
}
