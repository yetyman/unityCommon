using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public int movementSpeed = 1;
    public Rigidbody2D Body;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        var vec = Vector2.ClampMagnitude(new Vector2(x, y), 1);
        Body.MovePosition(Body.position + vec * movementSpeed * Time.fixedDeltaTime);
    }
}
