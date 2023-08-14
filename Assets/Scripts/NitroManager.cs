using UnityEngine;
using UnityEngine.UI;

public class NitroManager : MonoBehaviour
{
    public static NitroManager Instance { get; set; }
    public KeyCode key = KeyCode.N;
    public float maxNitroAmount = 100;
    public float currentNitroAmount = 100;
    public float decreaseSpeed = 5f;
    public bool autoRefill = true;
    public float refillSpeed = 2;
    public FastMotionBlur cameraMotionBlur;
    [Header("UI")]
    public Image nitroFillBar;
    public RectTransform nitroFillBarFlare;
    public Button nitroTriggerBtn;

    private bool usingNitro = false;

    private Rigidbody playerRb;
    private VehicleNitro playerNitro;

  

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    GameObject player = null;
    public PowerUpsHandler powerUpsHandler = null;

    private void FindPlayer()
    {
        if (!playerRb)
        {
            //GameObject player = GameObject.FindGameObjectWithTag ("Player");
            if (GameController.instance)
                player = GameController.instance.CurrentPlayer;
            if (player) playerRb = player.GetComponent<Rigidbody>();
            if (playerRb) playerNitro = playerRb.GetComponent<VehicleNitro>();
            if (player && player.GetComponent<PowerUpsHandler>())
                powerUpsHandler = player.GetComponent<PowerUpsHandler>();
        }
    }

    void NitroInput()
    {
        // if ((UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.GetButtonDown ("NOS") || Input.GetKeyDown (key)) /*  && playerRb.velocity.magnitude < playerNitro.maxVelocity*/ ) {
        if (Input.GetKeyDown(key))
            UseNitro();
    }

    void Update()
    {

        if (!playerRb) { FindPlayer(); return; }

        NitroInput();

        if (usingNitro)
        {
            currentNitroAmount -= Time.deltaTime * decreaseSpeed;

            if (currentNitroAmount <= 0)
            {
                currentNitroAmount = 0;
                // usingNitro = false;
                DisableNitro();
            }

            if (playerRb.velocity.magnitude <= 10f ||
                GameManager.Instance.GameStatus == "Loose" ||
                GameManager.Instance.GameStatus == "Win")
                DisableNitro();
            // UpdateNitroBar ();
        }
        else
        {
            if (autoRefill && currentNitroAmount < maxNitroAmount)
            {
                currentNitroAmount += Time.deltaTime * refillSpeed;
                if (currentNitroAmount > maxNitroAmount)
                    currentNitroAmount = maxNitroAmount;
                // UpdateNitroBar ();
            }
        }

        UpdateNitroBar();

    }

    void UpdateNitroBar()
    {
        nitroFillBar.fillAmount = currentNitroAmount / maxNitroAmount;
        if (currentNitroAmount > 0)
        {
            nitroTriggerBtn.gameObject.SetActive(true);
            nitroTriggerBtn.interactable = true;
        }
        else
        {
            nitroTriggerBtn.gameObject.SetActive(false);
            nitroTriggerBtn.interactable = false;
        }

        if (usingNitro && nitroFillBarFlare)
        {

            if (!nitroFillBarFlare.gameObject.activeInHierarchy) nitroFillBarFlare.gameObject.SetActive(true);

            nitroFillBarFlare.anchoredPosition = new Vector2(nitroFillBar.fillAmount * 632f, nitroFillBarFlare.position.y);
        }
        else
        {
            if (nitroFillBarFlare.gameObject.activeInHierarchy) nitroFillBarFlare.gameObject.SetActive(false);
        }
    }

    public void AddNitroAmount(float amt)
    {
        Debug.Log("Called boost");
        currentNitroAmount += amt;
        if (currentNitroAmount > maxNitroAmount)
        {
            currentNitroAmount = maxNitroAmount;
        }
    }

    void UseNitro()
    {
        if (powerUpsHandler && powerUpsHandler.currentState == PowerUpsHandler.CurrentState.Car)
        {
            usingNitro = true;
            playerNitro.NOSBool = true;

            cameraMotionBlur.enabled = true;
        }
    }
    void DisableNitro()
    {
        usingNitro = false;
        playerNitro.NOSBool = false;

        cameraMotionBlur.enabled = false;
    }
}