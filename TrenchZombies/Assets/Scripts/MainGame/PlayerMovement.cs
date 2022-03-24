using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Name: Ross Hutchins
//ID: HUT18001284

public class PlayerMovement : MonoBehaviour
{

    private CharacterController character;
    public MainGameScript mainGame;
    public Entity entity;

    public float moveSpeed = 10.0f;

    //Camera related variables.
    public float sensitivity = 90.0f;

    private float minLook = -60.0f;
    private float maxLook = 90.0f;

    [SerializeField] private Transform neckBone;
    [SerializeField] private Transform feetOrigin;
    [SerializeField] private Transform cameraTransform;

    private Animator animate;

    private Vector2 lookRotation = Vector2.zero;
    private Vector3 physicsForce = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        animate = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        //Calculating and minimizing rotation constraints.
        lookRotation += look * sensitivity * Time.deltaTime;
        lookRotation = new Vector2(lookRotation.x, Mathf.Min(maxLook, Mathf.Max(minLook, lookRotation.y)));

        //Apply gravity.
        bool isFalling = (!Physics.Raycast(feetOrigin.position, -Vector3.up, 0.2f) || physicsForce.y >= 0.0f);
        animate.SetBool("isFalling",isFalling);
        if (isFalling)
        {
            physicsForce += Physics.gravity * Time.deltaTime * 2.0f;
            character.Move(physicsForce * Time.deltaTime);
        }
        else
        {
            animate.SetBool("isJumping", false);
        }

        //Apply any other movements and rotations.
        character.Move((transform.forward * move.y * Time.deltaTime + transform.right * move.x * Time.deltaTime) * moveSpeed);
        animate.SetBool("isMoving", (move.magnitude >= 0.1f));

    }

    //We have to do the rotation transformation AFTER the animator finish writing all of its rotations and such.
    //Which is why I am using the lateupdate function here instead of update.
    private void LateUpdate()
    {
        transform.localRotation = Quaternion.Euler(0.0f, lookRotation.x, 0.0f);
        neckBone.localRotation = Quaternion.Euler(lookRotation.y, 0.0f, 0.0f);
    }

    public void Respawn()
    {
        moveSpeed = 10.0f;
        entity.Heal(999999);

        transform.position = new Vector3(4.0f, 0.1f, 0.0f);

        lookRotation = physicsForce = Vector3.zero;
        move = look = Vector2.zero;
    }

    //Unity only executes onmove and onlook if there is change, not constantly.
    //So I am storing the results into these two privates to make it constant.
    private Vector2 move = Vector2.zero;
    private Vector2 look = Vector2.zero;
    void OnMove(InputValue input)
    {
        move = input.Get<Vector2>();
    }

    void OnLook(InputValue input)
    {
        look = input.Get<Vector2>();
    }

    void OnJump(InputValue input)
    {
        if(Physics.Raycast(feetOrigin.position, -Vector3.up, 0.2f))
        {
            physicsForce = -Physics.gravity * 1.25f;
            animate.SetBool("isJumping", true);
        }
    }

    void OnUse(InputValue input)
    {
        if (entity.equipItem) {
            //Use the item they got equipped instead.
            entity.setMountTrigger(input.isPressed);
        }
        else if(input.isPressed)
        {
            //Empty handed, maybe they are trying to interact with objects in the scene.
            RaycastHit hit;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 3.0f, LayerMask.GetMask("Interactable")))
            {
                //See if it has an interactable script attached to it, if so execute its interact function passing the player as itself.
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                    interactable.Interact(this);
            }
        }
    }

    void OnOpenInventory(InputValue input)
    {
        entity.openInventory();
    }

    void OnInventoryScroll(InputValue input)
    {
        Vector2 direction = input.Get<Vector2>();
        if(direction.magnitude > 0.0f)
            entity.scrollItem((direction.y < 0.0f ? 1 : -1));
    }

    void OnPause(InputValue input)
    {
        if (input.isPressed)
            mainGame.onPause();
    }

}
