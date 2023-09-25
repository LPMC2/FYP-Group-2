using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement1 : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpforce = 5f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        rb.velocity = new Vector3(horizontalInput * speed, rb.velocity.y, verticalInput * speed);
        if (Input.GetButtonDown("Jump"))
        {
           rb.velocity = new Vector3(rb.velocity.x, jumpforce, rb.velocity.z);
        }
    }
}
