using Unity.VisualScripting;
using UnityEngine;

public class ThrowObjects : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform attckPoint;
    public GameObject objectToThrow;

    [Header("Settings")]
    public int totalThrow;
    public float throwCooldown;
    public float pickUpRange = 3f;
    public LayerMask pickUpLayer;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public KeyCode pickUpKey = KeyCode.E;
    public float throwForce;
    public float throwUpwardForce;

    private bool readyToThrow;
    private Animator animator;

    private void Start()
    {
        readyToThrow = true;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Throwing
        if (Input.GetKeyDown(throwKey) && readyToThrow && totalThrow > 0)
        {
            Throw();
        }

        // Picking up throwable objects
        if (Input.GetKeyDown(pickUpKey))
        {
            PickUpObject();
        }
    }

    private void Throw()
    {
        readyToThrow = false;

        // Animation
        animator.SetTrigger("Throw");

        // Instantiate projectile
        GameObject projectile = Instantiate(objectToThrow, attckPoint.position, cam.rotation);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // Calculate throw direction
        Vector3 forceDirection = cam.transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attckPoint.position).normalized;
        }

        // Apply force
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
        totalThrow--;

        // Reset throw
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }

    private void PickUpObject()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickUpRange, pickUpLayer);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Throwable"))
            {
                Destroy(hitCollider.gameObject);
                totalThrow++;
                Debug.Log("Picked up a throwable object!");
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickUpRange);
    }
}
