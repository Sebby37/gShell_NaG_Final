using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleTarget : MonoBehaviour
{
    public float mouseTargetDist = 1.5f;
    public float graphicSpinSpeed = 180;
    public GameObject selectedGraphic;
    public List<Sprite> selectedSprites = new List<Sprite>();
    int curSprite = 0;
    bool selected = false;
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SelectAnimation", 0.5f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Vector2.Distance((Vector2) transform.position, mousePos) <= mouseTargetDist)
            MakeTarget();
        else
            DitchTarget();
    }

    public void MakeTarget()
    {
        // Already selected, who cares
        if (selected || PlayerMove.isGrappling) return;
        
        // Set the target
        selected = true;
        PlayerMove.grappleTarget = gameObject;

        // Show the graphic
        selectedGraphic.SetActive(true);
    }

    public void DitchTarget()
    {
        // Not even selected anymore, who cares
        if (!selected || PlayerMove.isGrappling) return;
        
        // Unset as the target
        selected = false;
        if (PlayerMove.grappleTarget == gameObject)
            PlayerMove.grappleTarget = null;

        // Hide graphic
        selectedGraphic.SetActive(false);
    }

    void SelectAnimation()
    {
        // No bother
        if (!selected)
            return;

        // Update that index!
        curSprite += 1;
        if (curSprite >= selectedSprites.Count)
            curSprite = 0;

        // Change the sprite
        selectedGraphic.GetComponent<SpriteRenderer>().sprite = selectedSprites[curSprite];
    }
}
