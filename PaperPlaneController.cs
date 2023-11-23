using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PaperPlaneController : MonoBehaviour
{

    public float throwForce = 10f;
    private bool isThrown = false;
    private Rigidbody rb;
    public Vector3 windForce = new Vector3(1f, 0f, 0f); // Adjust the wind force as needed

    private Vector3 firstPos = Vector3.zero; // Adjust the wind force as needed
    private Quaternion firstRot = Quaternion.identity; // Adjust the wind force as needed

    [SerializeField] private UnityEvent throwEvent;
    [SerializeField] private UnityEvent resetEvent;

    private Coroutine cooldown;
    [SerializeField] private float cooldownTime = 5f;
    
    
    public float glideForce = 2f; // Adjust the glide force as needed
    public float drag = 10f; // Adjust the drag as needed
    public float reducedGravity = 2f; // Adjust the reduced gravity as needed

    void Start()
    {
        // Get the Rigidbody component of the paper plane
        rb = GetComponent<Rigidbody>();
        isThrown = false;
        rb.velocity = Vector3.zero;
        firstPos = transform.position;
        firstRot = transform.rotation;
        
        rb.drag = drag; // Set drag in Start method
    }

    void Update()
    {
        // Check for user input to throw the paper plane
        if (Input.GetKeyDown(KeyCode.Space) && !isThrown)
        {
            ThrowPaperPlane();
        }

        if (Input.GetKeyDown(KeyCode.R) && isThrown)
        {
            RestartPlane();
        }
    }


    public void ThrowPaperPlane(){
        if (cooldown != null)
        {
            StopCoroutine(cooldown);
        }

        cooldown = StartCoroutine(RespawnPlane());
        throwEvent.Invoke();
        isThrown = true;
        rb.isKinematic = false;
        
        //rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        
        
        // Reduce the initial force and add a glide force
        rb.AddForce(transform.forward * throwForce * 0.5f, ForceMode.Impulse);
        rb.AddForce(transform.up * glideForce, ForceMode.Impulse);
        
    }
    

    IEnumerator RespawnPlane(){
        yield return new WaitForSeconds(cooldownTime);
        RestartPlane();
    }

    public void RestartPlane()
    {
        resetEvent.Invoke();
        isThrown = false;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        transform.position = firstPos;
        transform.rotation = firstRot;
    }

    void FixedUpdate()
    {
        // Check if the paper plane is in the air
        if (isThrown)
        {
            ApplyWindForce();
        }
    }


    void ApplyWindForce()
    {
        // Apply the wind force continuously while the paper plane is in the air
        rb.AddForce(windForce, ForceMode.Force);
        //rb.AddForce(Physics.gravity * rb.mass, ForceMode.Force);
        rb.AddForce(reducedGravity * Physics.gravity * rb.mass, ForceMode.Force); // Apply reduced gravity
        // Rotate the object to face the direction it's moving
       
        
        //transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);
    
        // Reduce the rotation change to make it more realistic
        float rotationSpeed = 2.0f;
        Quaternion rotation = Quaternion.LookRotation(rb.velocity.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.fixedDeltaTime * rotationSpeed);
    
    
    
    }
}
