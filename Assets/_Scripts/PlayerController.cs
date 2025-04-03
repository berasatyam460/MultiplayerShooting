
using Photon.Pun;

using UnityEngine;


public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("CameraRef")]
    [SerializeField]Transform viewPoint;
    [SerializeField]float mouseSensitivity = 1f;
     private Camera mainCamera;
    private float verticalRotStore;
    private Vector2 mouseInput;
    [SerializeField]bool invertLook;
    
    [Header("MovementVar")]
    [SerializeField]float walkSpeed=5f,runSpeed=10f;
    private float activeMoveSpeed;
    Vector3 moveDir;
    Vector3 movement;
    
    [Header("JumpVar")]
    [SerializeField]float jumpForce;
    [SerializeField]float gravityMod=2f;
    [Header("GroundCheckVar")]
    [SerializeField]Transform groundCheckPos;
    [SerializeField]LayerMask groundLayer;
    bool isGrounded;
    
    private float timeBetweenShorts=0.1f;
    private float shootCounter;

    [SerializeField]GameObject bulletImapactPrefab;
    private CharacterController charController;
    [SerializeField]GameObject playerHitImpact;
    void Awake()
    {
        charController=GetComponent<CharacterController>();
        mainCamera=Camera.main;
    }
   
    void Start()
    {
        Cursor.lockState=CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine){
            
        mouseInput=new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"))*mouseSensitivity;
        transform.rotation=Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.z);
        verticalRotStore+=mouseInput.y;
        verticalRotStore=Mathf.Clamp(verticalRotStore,-60f,60f);
        if(invertLook){
            viewPoint.rotation=Quaternion.Euler(verticalRotStore,viewPoint.rotation.eulerAngles.y,viewPoint.rotation.eulerAngles.z);
        }else{
            viewPoint.rotation=Quaternion.Euler(-verticalRotStore,viewPoint.rotation.eulerAngles.y,viewPoint.rotation.eulerAngles.z);
        }
        if(Input.GetKey(KeyCode.LeftShift)){
            activeMoveSpeed=runSpeed;
        }else{ 
            activeMoveSpeed=walkSpeed;
        }
        isGrounded=Physics.Raycast(groundCheckPos.position,Vector3.down,.25f,groundLayer);
        if(Input.GetButtonDown("Jump") && isGrounded){
           movement.y=jumpForce;
        }
         
        if(Input.GetKeyDown(KeyCode.Escape)){
            Cursor.lockState=CursorLockMode.None;
        }else if(Cursor.lockState==CursorLockMode.None){
            if(Input.GetMouseButtonDown(0)){
                Cursor.lockState=CursorLockMode.Locked;
            }
        }
        if(Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
        if(Input.GetMouseButton(0)){
            shootCounter-=Time .deltaTime;
            if(shootCounter<=0){
                Shoot();
            }
            

        }
        }
    }

    void FixedUpdate()
    {
        if(photonView.IsMine){
        moveDir=new Vector3(Input.GetAxis("Horizontal"),0f,Input.GetAxis("Vertical"));
        float yVel=movement.y;
        movement=((transform.forward*moveDir.z)+(transform.right*moveDir.x)).normalized*activeMoveSpeed;
        movement.y=yVel;
        if(charController.isGrounded){
            movement.y=0;
        }
        charController.Move(movement*Time.fixedDeltaTime);
        movement.y+=Physics.gravity.y*Time.fixedDeltaTime*gravityMod;
        }
    }
    void LateUpdate()
    {
        if(photonView.IsMine){
            mainCamera.transform.position=viewPoint.transform.position;
            mainCamera.transform.rotation=viewPoint.transform.rotation;
        }
        
    }
    void Shoot(){
        Ray ray=mainCamera.ViewportPointToRay(new Vector3( .5f, .5f, 0f));
        ray.origin=mainCamera.transform.position;
        if(Physics.Raycast(ray,out RaycastHit hitInfo)){
            if(hitInfo.collider.gameObject.tag=="Player"){
                
            }else{
                             GameObject bulletImpactEffect=Instantiate(bulletImapactPrefab,hitInfo.point+(hitInfo.normal*0.002f),Quaternion.LookRotation(hitInfo.normal,Vector3.up));
            Destroy(bulletImpactEffect,2f);
            }

        }
        shootCounter=timeBetweenShorts;
    }
}
