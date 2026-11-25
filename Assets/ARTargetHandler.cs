using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Vuforia;

public class ARTargetHandler : MonoBehaviour
{
    [Header("UI & Panel")]
    public GameObject productInfoPanel; // Panel teks info di atas
    public Button btnJelajahiVR;        // Tombol di samping kanan

    [Header("Scene Settings")]
    public string vrSceneName = "Scene_VR";

    private ObserverBehaviour observerBehaviour;
    private AudioSource audioSource;
    private bool isTargetTracked = false;
    private float lostTimer = 0f;
    private bool isCountdownActive = false;
    private float lastBackPressTime = 0f;

    void Start()
    {
        // PENTING: Izinkan Unity membaca tombol Back HP Android
        Input.backButtonLeavesApp = false;

        // Setup suara bip otomatis
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Sembunyikan panel di awal
        if (productInfoPanel != null) productInfoPanel.SetActive(false);
        if (btnJelajahiVR != null)
        {
            btnJelajahiVR.gameObject.SetActive(false);
            btnJelajahiVR.onClick.AddListener(BukaDuniaVR);
        }

        observerBehaviour = GetComponent<ObserverBehaviour>();
        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    private void OnDestroy()
    {
        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    void Update()
    {
        // 1. SISTEM HITUNG MUNDUR 5 DETIK SETELAH TARGET HILANG
        if (isCountdownActive)
        {
            lostTimer += Time.deltaTime;
            if (lostTimer >= 5.0f) // Setelah 5 detik tidak terlihat
            {
                if (productInfoPanel != null) productInfoPanel.SetActive(false);
                if (btnJelajahiVR != null) btnJelajahiVR.gameObject.SetActive(false);
                isCountdownActive = false;
                lostTimer = 0f;
            }
        }

        // 2. TOMBOL BACK 2X UNTUK KELUAR APLIKASI
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.time - lastBackPressTime < 2.0f)
            {
                Application.Quit(); // Keluar
            }
            else
            {
                lastBackPressTime = Time.time;
                Handheld.Vibrate(); // Getar tanda tombol terbaca
            }
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        // Cek apakah stiker tumbler terlihat kamera
        if (targetStatus.Status == Status.TRACKED)
        {
            isTargetTracked = true;
            isCountdownActive = false; // Batalkan hitungan hilang jika stiker terlihat lagi
            lostTimer = 0f;

            // LANGSUNG TAMPILKAN INFO
            if (productInfoPanel != null && !productInfoPanel.activeSelf)
            {
                productInfoPanel.SetActive(true);
                if (btnJelajahiVR != null) btnJelajahiVR.gameObject.SetActive(true);

                // Bunyi Ting & Getar HP
                MainkanSuaraBeep();
                Handheld.Vibrate();
            }
        }
        else
        {
            // Saat stiker lepas dari kamera -> mulai hitung mundur 5 detik
            if (isTargetTracked)
            {
                isTargetTracked = false;
                isCountdownActive = true;
                lostTimer = 0f;
            }
        }
    }

    public void BukaDuniaVR()
    {
        Handheld.Vibrate();
        SceneManager.LoadScene(vrSceneName);
    }

    private void MainkanSuaraBeep()
    {
        int sampleRate = 44100;
        float durasi = 0.25f;
        int samples = (int)(sampleRate * durasi);
        float frekuensi = 1000f; // Nada Ting 1000Hz

        float[] dataSuara = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            dataSuara[i] = Mathf.Sin(2 * Mathf.PI * frekuensi * t) * (1.0f - (t / durasi));
        }

        AudioClip clip = AudioClip.Create("Beep", samples, 1, sampleRate, false);
        clip.SetData(dataSuara, 0);
        audioSource.PlayOneShot(clip);
    }
}