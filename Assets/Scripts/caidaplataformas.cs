using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingPlatform : MonoBehaviour
{
    [Header("Detect")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool onlyFromAbove = true;
    [SerializeField] private float delayBeforeFall = 0.75f; // 0.5 - 1.0

    [Header("Fall")]
    [SerializeField] private float fallSpeed = 6f;
    [SerializeField] private float minY = 0f;

    [Header("Reset")]
    [SerializeField] private float waitAtBottom = 1f;
    [SerializeField] private float riseSpeed = 6f;

    private Rigidbody rb;
    private Vector3 startPos;

    private enum State { Idle, Countdown, Falling, BottomWait, Rising }
    private State state = State.Idle;

    private float stateStartTime;

    // Para “anclar” al jugador
    private Transform attachedPlayerRoot;
    private Transform attachedOriginalParent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.freezeRotation = true;

        startPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryAttachPlayer(collision);

        // Solo dispara la caída desde Idle (una vez por ciclo)
        if (state != State.Idle) return;

        Transform playerRoot = GetPlayerRoot(collision);
        if (playerRoot == null) return;
        if (!playerRoot.CompareTag(playerTag)) return;

        if (onlyFromAbove && !IsFromAbove(collision)) return;

        state = State.Countdown;
        stateStartTime = Time.time;
    }

    private void OnCollisionStay(Collision collision)
    {
        // Por si OnCollisionEnter no pilló bien el contacto desde arriba,
        // aquí reintentamos el “attach” mientras esté encima.
        TryAttachPlayer(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        Transform playerRoot = GetPlayerRoot(collision);
        if (playerRoot == null) return;
        if (!playerRoot.CompareTag(playerTag)) return;

        DetachPlayer();
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Idle:
                // nada
                break;

            case State.Countdown:
                if (Time.time >= stateStartTime + delayBeforeFall)
                {
                    state = State.Falling;
                }
                break;

            case State.Falling:
            {
                Vector3 pos = transform.position;
                pos += Vector3.down * fallSpeed * Time.fixedDeltaTime;

                if (pos.y <= minY)
                {
                    pos.y = minY;
                    state = State.BottomWait;
                    stateStartTime = Time.time;
                }

                rb.MovePosition(pos);
                break;
            }

            case State.BottomWait:
                if (Time.time >= stateStartTime + waitAtBottom)
                {
                    state = State.Rising;
                }
                break;

            case State.Rising:
            {
                Vector3 pos = transform.position;
                Vector3 target = new Vector3(startPos.x, startPos.y, startPos.z);

                pos = Vector3.MoveTowards(pos, target, riseSpeed * Time.fixedDeltaTime);
                rb.MovePosition(pos);

                if (Vector3.Distance(pos, target) < 0.001f)
                {
                    // Reinicio completo: vuelve a funcionar como al principio
                    state = State.Idle;
                    DetachPlayer(); // importante: suelta al final del ciclo
                }
                break;
            }
        }
    }

    // ----------------- Helpers -----------------

    private Transform GetPlayerRoot(Collision collision)
    {
        // Si el collider del player está en un hijo, collision.transform no es el root.
        // El Rigidbody es lo más fiable para llegar al root real.
        if (collision.rigidbody != null) return collision.rigidbody.transform;
        return collision.transform;
    }

    private bool IsFromAbove(Collision collision)
    {
        // Normal.y alta = contacto desde arriba
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y > 0.2f) return true;
        }
        return false;
    }

    private void TryAttachPlayer(Collision collision)
    {
        Transform playerRoot = GetPlayerRoot(collision);
        if (playerRoot == null) return;
        if (!playerRoot.CompareTag(playerTag)) return;

        if (onlyFromAbove && !IsFromAbove(collision)) return;

        // Ya está anclado a ESTA plataforma
        if (attachedPlayerRoot == playerRoot && playerRoot.parent == transform) return;

        // Si no hay nadie anclado, lo anclamos
        if (attachedPlayerRoot == null)
        {
            attachedPlayerRoot = playerRoot;
            attachedOriginalParent = playerRoot.parent;
            playerRoot.SetParent(transform, true);
        }
    }

    private void DetachPlayer()
    {
        if (attachedPlayerRoot == null) return;

        // Solo desparentamos si sigue siendo hijo nuestro
        if (attachedPlayerRoot.parent == transform)
            attachedPlayerRoot.SetParent(attachedOriginalParent, true);

        attachedPlayerRoot = null;
        attachedOriginalParent = null;
    }
}
