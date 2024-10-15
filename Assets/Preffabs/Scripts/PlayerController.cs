using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 10f;
    public float airJumpForceMultiplier = 0.5f;
    private Rigidbody2D rb;
    private bool enPlaneta = false;
    private Transform currentPlanet;
    private Vector3 startPosition;
    public Text gameOverText;
    public float planetRadiusOffset = 0.5f;
    public Camera mainCamera;
    public Vector3 cameraOffset;
    public GameObject canvasFinDeJuego;
    // Variables para la vida
    public int maxHealth = 3; // Total de vidas
    private int currentHealth; // Vidas actuales
    public Image healthBar; // Referencia a la barra de vida
    public GameObject vida;
    public GameObject vida1;
    public GameObject vida2;
    // Variables para los puntos
    public int totalPoints = 0; // Puntos totales
    public int pointsPerItem = 100; // Puntos por ítem
    public Image scoreBar; // Referencia a la barra de puntos
    public Text pointsText; // Referencia al texto de puntos en pantalla

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        startPosition = transform.position;

        currentHealth = maxHealth; // Inicializamos la vida al máximo
        UpdateHealthBar();
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        UpdateHealthBar();
        UpdateScoreBar(); // Inicializa la barra de puntos al inicio
        UpdatePointsText(); // Inicializa el texto de puntos al inicio
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (enPlaneta && currentPlanet != null)
        {
            Vector3 directionToPlanet = (transform.position - currentPlanet.position).normalized;
            transform.position = currentPlanet.position + directionToPlanet * (currentPlanet.localScale.x / 2 + planetRadiusOffset);

            Vector3 gravityDirection = (currentPlanet.position - transform.position).normalized;
            float angle = Mathf.Atan2(gravityDirection.y, gravityDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90);
        }

        if (mainCamera != null)
        {
            mainCamera.transform.position = transform.position + cameraOffset;
        }
    }

    void Jump()
    {
        enPlaneta = false;
        transform.SetParent(null);

        Vector2 jumpDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.A)) jumpDirection = Vector2.left;
        if (Input.GetKey(KeyCode.D)) jumpDirection = Vector2.right;
        if (Input.GetKey(KeyCode.W)) jumpDirection = Vector2.up;
        if (Input.GetKey(KeyCode.S)) jumpDirection = Vector2.down;

        if (jumpDirection == Vector2.zero) jumpDirection = Vector2.up;

        rb.velocity = Vector2.zero;

        float appliedJumpForce = enPlaneta ? jumpForce : jumpForce * airJumpForceMultiplier;
        rb.AddForce(jumpDirection * appliedJumpForce, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colisión con: " + collision.gameObject.tag);

        if (collision.gameObject.CompareTag("Planet"))
        {
            enPlaneta = true;
            rb.velocity = Vector2.zero;
            currentPlanet = collision.transform;

            transform.SetParent(currentPlanet);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Colisión con enemigo");
            TakeDamage(35); // Resta una vida al colisionar con un enemigo
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item")) // Verifica si colisiona con un ítem (corregido el tag a "Item")
        {
            totalPoints += pointsPerItem; // Suma los puntos por ítem
            UpdateScoreBar(); // Actualiza la barra de puntuación
            UpdatePointsText(); // Actualiza el texto en pantalla
            Destroy(collision.gameObject); // Destruye el ítem recolectado
            Debug.Log("Puntos totales: " + totalPoints); // Muestra los puntos en consola
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage; // Resta la vida
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Limita la vida entre 0 y el máximo
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Debug.Log("Juego terminado");
            GameOver();
        }
        else
        {
            ResetPosition();
        }
    }

    void ResetPosition()
    {
        transform.position = startPosition;
        rb.velocity = Vector2.zero;
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            Debug.Log("Actualizando barra de vida: " + currentHealth);
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
        switch(currentHealth)
        {
            case 100:
                vida.SetActive(true);
                vida1.SetActive(true);
                vida2.SetActive(true);
                break;
            case 65:
                vida.SetActive(true);
                vida1.SetActive(true);
                vida2.SetActive(false);
                break;
            case 30:
                vida.SetActive(true);
                vida1.SetActive(false);
                vida2.SetActive(false);
                break;
            case 0:
                vida.SetActive(false);
                vida1.SetActive(false);
                vida2.SetActive(false);
                break;
        }
    }

    void UpdateScoreBar()
    {
        if (scoreBar != null)
        {
            scoreBar.fillAmount = (float)totalPoints / 1000f; // Ajusta el divisor según el máximo que quieras mostrar en la barra.
        }
    }

    void UpdatePointsText()
    {
        if (pointsText != null)
        {
            pointsText.text = "Points: " + totalPoints; // Actualiza el texto con los puntos totales
        }
    }

    void GameOver()
    {
       canvasFinDeJuego.SetActive(true);
        Time.timeScale = 0f; // Pausar el juego
    }
}

