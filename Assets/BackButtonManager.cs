using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class BackButtonManager : MonoBehaviour
{
    [Header("Settings")]
    public bool isVRScene = false; // Centang jika script ini dipasang di Scene_VR
    public float doubleTapThreshold = 2.0f; // Jeda maksimal 2 detik antar ketukan

    [Header("UI Opsional (Tombol Fisik di Layar)")]
    public Button btnBackLayar; // Tombol panah kembali di pojok kiri atas (jika ada)

    private float lastBackPressTime = 0f;

    void Start()
    {
        if (btnBackLayar != null)
        {
            btnBackLayar.onClick.AddListener(EksekusiKembali);
        }
    }

    void Update()
    {
        // Deteksi tombol BACK bawaan Android (KeyCode.Escape)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.time - lastBackPressTime < doubleTapThreshold)
            {
                // Tekanan kedua berhasil dalam 2 detik
                EksekusiKembali();
            }
            else
            {
                // Tekanan pertama
                lastBackPressTime = Time.time;
                Handheld.Vibrate(); // Getar kecil tanda perintah terbaca
                Debug.Log("[Sistem] Tekan sekali lagi untuk keluar/kembali");
            }
        }
    }

    public void EksekusiKembali()
    {
        Handheld.Vibrate();

        if (isVRScene)
        {
            // Jika sedang di VR -> Kembali ke AR
            SceneManager.LoadScene("Scene_AR");
        }
        else
        {
            // Jika sedang di AR -> Keluar Aplikasi
            Application.Quit();
            Debug.Log("[Sistem] Keluar dari aplikasi.");
        }
    }
}