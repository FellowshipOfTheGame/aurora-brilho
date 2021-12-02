using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaleaoMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float accelerationTime;
    [SerializeField] float decelerationTime;
    [SerializeField] float topSpeed;
    [SerializeField] [Range(0, 1)] float airControl;
    [SerializeField] float jumpVelocity;
    float acceleration;
    float deceleration;
    
    [Header("Linguona!")]
    [SerializeField] Camera mainCamera;
    [SerializeField] float distMaxLingua;
    bool linguaAncorada;
    Vector2 springAnchor;
    LineRenderer lineRenderer;

    
    //Current state of the game
    bool grudado;
    bool pulando;
    bool[] walls = new bool[4];
    bool[] corners = new bool[4];
    float magVel = 0;


    [Header("Ground Check")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] [Range(0, 0.05f)] float groundCheckWidth = 0.02f; // tested value -> 0.015f

    Vector3[][] wallChecks = new Vector3[4][];
    Vector3[][] cornerChecks = new Vector3[4][];


    //Input Variables
    Vector2Int xyInput;
    Vector2 mousePos;
    bool atirandoLingua;

    [Header("Ossos")]
    [SerializeField] Transform Osso1;


    //Cached Components
    //[Header("Cached Components")]
    CircleCollider2D boxCollider;
    Rigidbody2D rb;
    SpringJoint2D springJoint;

    [Header("Debug")]
    [SerializeField] bool recalculateVariables;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<CircleCollider2D>();
        springJoint = GetComponent<SpringJoint2D>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        RecalculateVariables();
        lineRenderer.positionCount = 2;

    }

    void Update()
    {
        GetInput();
        if (recalculateVariables){
            RecalculateVariables();
        }

        lineRenderer.enabled = linguaAncorada;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, springAnchor);
        

    }

    void FixedUpdate()
    {
        GerarWallChecks(boxCollider.bounds.center);
        grudado = CheckWalls();
        AtualizarOssos();
        Movement();
    }

    private void AtualizarOssos(){
        float[] wallsAng = {180, 90, 0, 270};
        float Ang = 0;
        for (int i=0; i<4; i++)
        {
            if (walls[i]){
                Ang = wallsAng[i];
            }
        }
        Osso1.eulerAngles = new Vector3(0,0,Ang);
    }
    private void Movement()
    {   
        Vector2 mov = AvaliarMovimento();

        AtirarLingua();

        if (grudado)
        {   
            rb.gravityScale = 0;
            bool inputInDirectionOfMovement = ((mov.x == 1 && rb.velocity.x >= 0) || (mov.x == -1 && rb.velocity.x <= 0)||
                                            (mov.y == 1 && rb.velocity.y >= 0) || (mov.y == -1 && rb.velocity.y <= 0));
            
            linguaAncorada = false;
            springJoint.enabled = false;

            if (inputInDirectionOfMovement && rb.velocity.magnitude <= topSpeed)
            {
                Accelerate();
            }
            else
            {
                Decelerate();
            }

            if (pulando) 
            {
                Jump();
            }
            else
            {
                //Avalia se na velocidade atual o camaleão saira da parede
                // e diminue a vel neste caso
                for (int i=0; i<4; i++)
                {
                    GerarWallChecks((Vector2)boxCollider.bounds.center + rb.velocity*Time.deltaTime);
                    if (!CheckWalls())
                    {   
                        Debug.Log("Para");

                        rb.velocity *= 0.3f; //Vector2.zero;
                        if (i==3)rb.velocity*=0;
                    }
                    else break;
                }
            }

        }
        else
        {
            rb.gravityScale = 1;
            pulando = false;

            rb.velocity += (Vector2)xyInput*topSpeed*airControl*Time.deltaTime;
            
            springJoint.enabled = linguaAncorada;

        }
        Debug.Log("atirandoLingua: "+atirandoLingua);
        Debug.Log("linguaAncorada: "+linguaAncorada);

    }

    void Accelerate()
    {
        Vector2 Velocity = rb.velocity;

        float velocityChange = acceleration * Time.deltaTime;
        if (!grudado) velocityChange *= airControl;

        magVel += velocityChange;

        if (magVel > topSpeed)
            magVel -= magVel - topSpeed;
        
        rb.velocity = AvaliarMovimento()*magVel;
    }

    void Decelerate()
    {
        float velocityChange = deceleration * Time.deltaTime;
        if (!grudado) velocityChange *= airControl;

        magVel -= velocityChange;
        if (magVel<0) magVel = 0;

        rb.velocity = rb.velocity.normalized*magVel; 
    }

    void Jump()
    {
        //float[] wallsAng = {0.75f, 1, 0.5f, 0};
        float[] wallsAng = {1.5f, 1f, 0.5f, 0};
        float Ang = 0;
        for (int i=0; i<4; i++)
        {
            if (walls[i]){
                Ang = Mathf.PI*wallsAng[i];
            }
        }
        rb.velocity+= new Vector2(Mathf.Cos(Ang), Mathf.Sin(Ang))*jumpVelocity;

    }

    void AtirarLingua()
    {   
        if (atirandoLingua)
        {
            if (!linguaAncorada)
            {
                Vector2 dir = mousePos-(Vector2)transform.position;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir.normalized, distMaxLingua, groundMask);

                if (hit.point!=null)
                {
                    springJoint.connectedAnchor = hit.point;
                    springAnchor = springJoint.connectedAnchor;
                    linguaAncorada = true;
                }
            }
        }
        else linguaAncorada = false;
    }

    Vector2 AvaliarMovimento()
    {   
        Vector2 movIdeal;
        if (grudado)
        {        
            movIdeal = AvaliarMovimentoSimples();
            int cont=0;
            for (int i=0; i<4; i++){
                if (corners[i]) cont++;
                if (walls[i]) cont++;
            }

            if (cont == 1)
            {
                if (corners[0]){
                    if (movIdeal.x<0) movIdeal.x = 0;
                    if (movIdeal.y<0) movIdeal.y = 0;
                }
                if (corners[1]){
                    if (movIdeal.x<0) movIdeal.x = 0;
                    if (movIdeal.y>0) movIdeal.y = 0;
                }
                if (corners[2]){
                    if (movIdeal.x>0) movIdeal.x = 0;
                    if (movIdeal.y>0) movIdeal.y = 0;
                }
                if (corners[3]){
                    if (movIdeal.x>0) movIdeal.x = 0;
                    if (movIdeal.y<0) movIdeal.y = 0;
                }
            }
        }
        else
        {
            movIdeal = xyInput;
        }
        return movIdeal;
    }

    Vector2 AvaliarMovimentoSimples()
    {
        Vector2 movIdeal = new Vector2(0, 0);
        if (xyInput.x != 0)
        {
            if (walls[0])
            {
                movIdeal += new Vector2(0.5f, 0);
            }
            else if (walls[2])
            {
                movIdeal += new Vector2(0.5f, 0);
            }
        }
        if (xyInput.x > 0)
        {
            if (corners[0] || corners[1])
            {
                movIdeal += new Vector2(0.5f, 0);
            }
        }
        if (xyInput.x < 0)
        {
            if (corners[2] || corners[3])
            {
                movIdeal += new Vector2(0.5f, 0);
            }
            movIdeal.x *= -1;
        }

        if (xyInput.y != 0)
        {
            if (walls[1])
            {
                movIdeal += new Vector2(0, 0.5f);
            }
            else if (walls[3])
            {
                movIdeal += new Vector2(0, 0.5f);
            }
        }
        if (xyInput.y > 0)
        {
            if (corners[0] || corners[3])
            {
                movIdeal += new Vector2(0, 0.5f);
            }
        }
        if (xyInput.y < 0)
        {
            if (corners[1] || corners[2])
            {
                movIdeal += new Vector2(0, 0.5f);
            }
            movIdeal.y *= -1;
        }
        if(Mathf.Abs(movIdeal.x)==0.5f){
            movIdeal.x*=2;
        }
        if(Mathf.Abs(movIdeal.y)==0.5f){
            movIdeal.y*=2;
        }
        //if(movIdeal.magnitude!=0) movIdeal = movIdeal.normalized;
        return movIdeal;
    }

    void GerarWallChecks(Vector3 centro)
    {
        Vector3 pointA, pointB;
        //Norte
        pointA = new Vector3(centro.x - boxCollider.bounds.extents.x + groundCheckWidth,
                             centro.y + boxCollider.bounds.extents.y + groundCheckWidth);
        pointB = new Vector3(centro.x + boxCollider.bounds.extents.x - groundCheckWidth,
                             centro.y + boxCollider.bounds.extents.y - groundCheckWidth);
        wallChecks[0] = new Vector3[2] {pointA, pointB};

        //Leste
        pointA = new Vector3(centro.x + boxCollider.bounds.extents.x - groundCheckWidth,
                             centro.y + boxCollider.bounds.extents.y - groundCheckWidth);
        pointB = new Vector3(centro.x + boxCollider.bounds.extents.x + groundCheckWidth,
                             centro.y - boxCollider.bounds.extents.y + groundCheckWidth);
        wallChecks[1] = new Vector3[2] {pointA, pointB};

        //Sul
        pointA = new Vector3(centro.x - boxCollider.bounds.extents.x + groundCheckWidth,
                             centro.y - boxCollider.bounds.extents.y + groundCheckWidth);
        pointB = new Vector3(centro.x + boxCollider.bounds.extents.x - groundCheckWidth,
                             centro.y - boxCollider.bounds.extents.y - groundCheckWidth);
        wallChecks[2] = new Vector3[2] {pointA, pointB};

        //Oeste
        pointA = new Vector3(centro.x - boxCollider.bounds.extents.x - groundCheckWidth,
                             centro.y + boxCollider.bounds.extents.y - groundCheckWidth);
        pointB = new Vector3(centro.x - boxCollider.bounds.extents.x + groundCheckWidth,
                             centro.y - boxCollider.bounds.extents.y + groundCheckWidth);
        wallChecks[3] = new Vector3[2] {pointA, pointB};

        //Nordeste
        pointA = centro + boxCollider.bounds.extents;
        pointB = pointA + new Vector3(groundCheckWidth, groundCheckWidth);
        cornerChecks[0] = new Vector3[2] {pointA, pointB};

        //Sudeste
        pointA = centro + new Vector3(boxCollider.bounds.extents.x, -boxCollider.bounds.extents.y);
        pointB = pointA + new Vector3(groundCheckWidth, -groundCheckWidth);
        cornerChecks[1] = new Vector3[2] {pointA, pointB};

        //Sudoeste
        pointA = centro - boxCollider.bounds.extents;
        pointB = pointA - new Vector3(groundCheckWidth, groundCheckWidth);
        cornerChecks[2] = new Vector3[2] {pointA, pointB};

        //Noroeste
        pointA = centro + new Vector3(-boxCollider.bounds.extents.x, boxCollider.bounds.extents.y);
        pointB = pointA + new Vector3(-groundCheckWidth, groundCheckWidth);
        cornerChecks[3] = new Vector3[2] {pointA, pointB};
    }

    bool CheckWalls()
    {
        bool flag = false;
        for (int i=0; i<4; i++){
            if (Physics2D.OverlapArea(wallChecks[i][0], wallChecks[i][1], groundMask))
            {
                flag = true;
                walls[i] = true;
            }
            else
            {
                walls[i] = false;
            }
        }
        for (int i=0; i<4; i++){
            if (Physics2D.OverlapArea(cornerChecks[i][0], cornerChecks[i][1], groundMask))
            {
                flag = true;
                corners[i] = true;
            }
            else
            {
                corners[i] = false;
            }
        }
        return flag;
    }

    void GetInput()
    {
        float xAxis = Input.GetAxisRaw("Horizontal");
        float yAxis = Input.GetAxisRaw("Vertical");

        // Controller joystick snap
        xAxis = (xAxis > 0.1f) ? 1 : (xAxis < -0.1f) ? -1 : 0;
        yAxis = (yAxis > 0.1f) ? 1 : (yAxis < -0.1f) ? -1 : 0;

        xyInput = new Vector2Int((int)xAxis, (int)yAxis);

        if (Input.GetButtonDown("Jump")) pulando = true;

        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        atirandoLingua = Input.GetMouseButton(0);

    }

    void RecalculateVariables()
    {
        // Initialize horizontal movement variables
        acceleration = topSpeed / accelerationTime;
        deceleration = topSpeed / decelerationTime;
    }


//Gizmos
    void OnDrawGizmos()
    {      
        Gizmos.color = Color.blue;
        for(int i=0; i<4; i++){
            if (wallChecks[i]!=null && walls[i]){
                DrawBox(wallChecks[i][0], wallChecks[i][1]);
            }
        }
        Gizmos.color = Color.red;
        for(int i=0; i<4; i++){
            if (cornerChecks[i]!=null && corners[i]){
                DrawBox(cornerChecks[i][0], cornerChecks[i][1]);
            }
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(mousePos, 1);

        if(linguaAncorada){
            Gizmos.DrawLine(transform.position, springAnchor);
        }
    }

    void DrawBox(Vector3 PontoA, Vector3 PontoB)
    {
        //Gizmos.DrawLine(PontoA, PontoB);
        Gizmos.DrawLine(PontoA, PontoA - new Vector3(PontoA.x-PontoB.x, 0));
        Gizmos.DrawLine(PontoB, PontoB - new Vector3(PontoB.x-PontoA.x, 0));
        Gizmos.DrawLine(PontoB, PontoB - new Vector3(0, PontoB.y-PontoA.y));
        Gizmos.DrawLine(PontoA, PontoA - new Vector3(0, PontoA.y-PontoB.y));
    }

}
