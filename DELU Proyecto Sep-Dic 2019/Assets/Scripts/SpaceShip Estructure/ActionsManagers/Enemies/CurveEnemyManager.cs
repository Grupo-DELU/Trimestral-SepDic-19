using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CurveEnemyManager : ActionManager
{
    /// <summary>
    /// Curva/patron a seguir
    /// </summary>
    [SerializeField]
    private CurveScriptObject path;

    /// <summary>
    /// Indica si la nave se esta moviendo
    /// </summary>
    [SerializeField]
    private bool bIsMoving = true;

    /// <summary>
    /// Posicion del punto a moverse
    /// </summary>
    public Vector2 Target
    {
        get { return vTargetPos; }
    }

    /// <summary>
    /// Posicion del punto a moverse
    /// </summary>
    private Vector2 vTargetPos;
    /// <summary>
    /// Indice del punto a moverse en path
    /// </summary>
    private int iTargetIndex = 0;

    public float fMaxSpeed = 10;
    [Range(-1f, 1f)]
    public float fSpeed = 1;

    public Vector2 velocity = Vector2.zero;
    private Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();    
    }

    protected override void Start()
    {
        base.Start();
        vTargetPos = path.points[0];    
    }

    // Update is called once per frame
    void Update()
    {
        if (bIsMoving) executeAction("SimpleFollowCurve");
    }

    /// <summary>
    /// Setea/calcula el proximo punto target del ennemigo
    /// </summary>
    public void NextPoint()
    {
        iTargetIndex = (iTargetIndex + 1) % path.points.Length;
        vTargetPos = path.points[iTargetIndex];
    }

    public void MoveWithVel(Vector2 velocity)
    {
        this.velocity = velocity;
    }

    private void FixedUpdate()
    {
        if (velocity != Vector2.zero)
        {
            rb2d.MovePosition((Vector2)transform.position + velocity * Time.fixedDeltaTime);
            velocity = Vector2.zero;
        }
    }
}
