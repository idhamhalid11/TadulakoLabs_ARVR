using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class VRGazeInteraction : MonoBehaviour
{
    private Image reticleImage;
    private GameObject infoPanelObj;
    private TextMeshProUGUI infoText;

    private float rotX = 0f;
    private float rotY = 0f;
    private Vector3 lastMousePos;
    private GameObject lastTarget = null;
    private float lastBackPressTime = 0f;

    // Kontrol Gerak
    private float moveX = 0f;
    private float moveZ = 0f;
    public float moveSpeed = 3.5f;

    void Start()
    {
        Input.backButtonLeavesApp = false;

        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }

        // Buat UI Titik Merah, Info Panel, Tombol Jalan & Tombol Kembali otomatis
        BuatUIKomplitOtomatis();
    }

    void Update()
    {
        // 1. GERAKAN KAMERA (MAJU, MUNDUR, KIRI, KANAN)
        Vector3 moveDir = (transform.forward * moveZ + transform.right * moveX);
        moveDir.y = 0; // Tetap di tanah
        transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;

        // 2. USAP LAYAR (SWIPE JARI) UNTUK MENOLEH 360 DERAJAT
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            // Abaikan jika menyentuh bagian tombol navigasi bawah
            if (Input.mousePosition.y > 250)
            {
                Vector3 delta = Input.mousePosition - lastMousePos;
                lastMousePos = Input.mousePosition;

                rotY += delta.x * 0.2f;
                rotX -= delta.y * 0.2f;
                rotX = Mathf.Clamp(rotX, -80f, 80f);

                transform.rotation = Quaternion.Euler(rotX, rotY, 0f);
            }
        }
        else if (SystemInfo.supportsGyroscope && Input.gyro.enabled)
        {
            // Putaran Gyro jika HP digerakkan
            transform.Rotate(-Input.gyro.rotationRateUnbiased.x, -Input.gyro.rotationRateUnbiased.y, 0);
        }

        // 3. GAZE INTERACTION (TITIK MERAH MENATAP TUMBLER)
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 30f))
        {
            VRTumblerItem tumbler = hit.collider.GetComponentInParent<VRTumblerItem>();
            if (tumbler == null) tumbler = hit.collider.GetComponent<VRTumblerItem>();

            if (tumbler != null)
            {
                if (reticleImage != null) reticleImage.color = Color.green;

                if (lastTarget != hit.collider.gameObject)
                {
                    lastTarget = hit.collider.gameObject;

                    // HAPTIC FEEDBACK (HP Bergetar!)
                    Handheld.Vibrate();

                    // Tampilkan Info
                    if (infoPanelObj != null) infoPanelObj.SetActive(true);
                    if (infoText != null)
                    {
                        infoText.text = "<b><size=32>" + tumbler.namaProduk + "</size></b>\n" +
                                        "<color=yellow><b>Harga: " + tumbler.harga + "</b></color>\n" +
                                        "<size=22>" + tumbler.deskripsi + "</size>";
                    }
                }
                goto CekTombolBack;
            }
        }

        // Jika tidak menatap tumbler
        if (reticleImage != null) reticleImage.color = Color.red;
        if (lastTarget != null)
        {
            lastTarget = null;
            if (infoPanelObj != null) infoPanelObj.SetActive(false);
        }

    CekTombolBack:
        // 4. TOMBOL BACK BAWAAN ANDROID (Tekan 2x)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.time - lastBackPressTime < 2.0f)
            {
                KembaliKeAR();
            }
            else
            {
                lastBackPressTime = Time.time;
                Handheld.Vibrate();
            }
        }
    }

    public void KembaliKeAR()
    {
        Handheld.Vibrate();
        SceneManager.LoadScene("Scene_AR");
    }

    private void BuatUIKomplitOtomatis()
    {
        GameObject canvasObj = new GameObject("Canvas_VR_Auto");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 1. Titik Merah di Tengah
        GameObject dotObj = new GameObject("Reticle_Dot");
        dotObj.transform.SetParent(canvasObj.transform, false);
        reticleImage = dotObj.AddComponent<Image>();
        reticleImage.color = Color.red;
        reticleImage.rectTransform.sizeDelta = new Vector2(25, 25);
        reticleImage.rectTransform.anchoredPosition = Vector2.zero;

        // 2. Tombol Kembali di Pojok Kiri Atas [ ◀ Kembali ]
        GameObject btnBackObj = new GameObject("Btn_BackToAR");
        btnBackObj.transform.SetParent(canvasObj.transform, false);
        Image btnBg = btnBackObj.AddComponent<Image>();
        btnBg.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);
        Button btn = btnBackObj.AddComponent<Button>();
        btn.onClick.AddListener(KembaliKeAR);

        RectTransform btnRect = btnBackObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0f, 1f);
        btnRect.anchorMax = new Vector2(0f, 1f);
        btnRect.pivot = new Vector2(0f, 1f);
        btnRect.anchoredPosition = new Vector2(30, -30);
        btnRect.sizeDelta = new Vector2(200, 70);

        GameObject btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(btnBackObj.transform, false);
        TextMeshProUGUI bTxt = btnTextObj.AddComponent<TextMeshProUGUI>();
        bTxt.text = "◀ Kembali";
        bTxt.fontSize = 24;
        bTxt.alignment = TextAlignmentOptions.Center;
        bTxt.color = Color.white;
        RectTransform bTxtRect = btnTextObj.GetComponent<RectTransform>();
        bTxtRect.anchorMin = Vector2.zero;
        bTxtRect.anchorMax = Vector2.one;
        bTxtRect.sizeDelta = Vector2.zero;

        // 3. Panel Info Produk (Bawah Layar)
        infoPanelObj = new GameObject("Panel_Info_VR");
        infoPanelObj.transform.SetParent(canvasObj.transform, false);
        Image pBg = infoPanelObj.AddComponent<Image>();
        pBg.color = new Color(0, 0, 0, 0.85f);

        RectTransform pRect = infoPanelObj.GetComponent<RectTransform>();
        pRect.anchorMin = new Vector2(0.5f, 0f);
        pRect.anchorMax = new Vector2(0.5f, 0f);
        pRect.pivot = new Vector2(0.5f, 0f);
        pRect.anchoredPosition = new Vector2(0, 180);
        pRect.sizeDelta = new Vector2(750, 200);

        GameObject infoTextObj = new GameObject("Text_Info");
        infoTextObj.transform.SetParent(infoPanelObj.transform, false);
        infoText = infoTextObj.AddComponent<TextMeshProUGUI>();
        infoText.fontSize = 22;
        infoText.alignment = TextAlignmentOptions.Center;
        infoText.color = Color.white;
        RectTransform iTxtRect = infoTextObj.GetComponent<RectTransform>();
        iTxtRect.anchorMin = Vector2.zero;
        iTxtRect.anchorMax = Vector2.one;
        iTxtRect.sizeDelta = Vector2.zero;
        infoPanelObj.SetActive(false);

        // 4. Tombol Navigasi Berjalan (Maju, Mundur, Kiri, Kanan)
        BuatTombolGerak(canvasObj.transform, "▲", new Vector2(0, 100), () => moveZ = 1f, () => moveZ = 0f);
        BuatTombolGerak(canvasObj.transform, "▼", new Vector2(0, 20), () => moveZ = -1f, () => moveZ = 0f);
        BuatTombolGerak(canvasObj.transform, "◀", new Vector2(-80, 20), () => moveX = -1f, () => moveX = 0f);
        BuatTombolGerak(canvasObj.transform, "▶", new Vector2(80, 20), () => moveX = 1f, () => moveX = 0f);
    }

    private void BuatTombolGerak(Transform parent, string label, Vector2 pos, System.Action onPress, System.Action onRelease)
    {
        GameObject bObj = new GameObject("Btn_" + label);
        bObj.transform.SetParent(parent, false);
        Image bg = bObj.AddComponent<Image>();
        bg.color = new Color(1, 1, 1, 0.25f);

        RectTransform r = bObj.GetComponent<RectTransform>();
        r.anchorMin = new Vector2(0.5f, 0f);
        r.anchorMax = new Vector2(0.5f, 0f);
        r.pivot = new Vector2(0.5f, 0f);
        r.anchoredPosition = pos;
        r.sizeDelta = new Vector2(70, 70);

        GameObject tObj = new GameObject("Text");
        tObj.transform.SetParent(bObj.transform, false);
        TextMeshProUGUI txt = tObj.AddComponent<TextMeshProUGUI>();
        txt.text = label;
        txt.fontSize = 28;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;
        RectTransform tRect = tObj.GetComponent<RectTransform>();
        tRect.anchorMin = Vector2.zero;
        tRect.anchorMax = Vector2.one;
        tRect.sizeDelta = Vector2.zero;

        // Event Trigger Tekan dan Lepas
        UnityEngine.EventSystems.EventTrigger trigger = bObj.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        var entryDown = new UnityEngine.EventSystems.EventTrigger.Entry();
        entryDown.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
        entryDown.callback.AddListener((data) => onPress());
        trigger.triggers.Add(entryDown);

        var entryUp = new UnityEngine.EventSystems.EventTrigger.Entry();
        entryUp.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
        entryUp.callback.AddListener((data) => onRelease());
        trigger.triggers.Add(entryUp);
    }
}