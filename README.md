\# TadulakoLabs AR/VR Smart Tumbler Showcase



Aplikasi mobile berbasis \*\*Augmented Reality (AR)\*\* dan \*\*Virtual Reality (VR)\*\* yang dikembangkan menggunakan \*\*Unity 6 (6000.6.9f1)\*\* dan \*\*Vuforia Engine\*\* untuk menampilkan produk \*TadulakoLabs Smart Tumbler\* secara interaktif.



\## Fitur Utama

\* \*\*AR Cylinder Target Tracking (`Scene\_AR`):\*\* Mendeteksi permukaan silinder tumbler fisik menggunakan kamera smartphone dan menampilkan informasi produk interaktif beserta tombol navigasi ke mode VR.

\* \*\*VR Product Showcase (`Scene\_VR`):\*\* Menampilkan 3 varian produk 3D (\*Tumbler Ori Tadulako\*, \*Tumbler Red Edition\*, dan \*Tumbler Matte Black\*) dengan kontrol rotasi \*Gyroscope\* maupun usap layar (\*Touch Swipe\*).

\* \*\*Gaze Reticle \& Haptic Feedback:\*\* Titik bidik (\*reticle\*) berubah warna dari merah ke hijau saat diarahkan ke objek tumbler, memicu getaran perangkat (\*haptic\*), dan menampilkan panel detail nama, harga, serta deskripsi produk.

\* \*\*Navigasi Kembali ke AR:\*\* Menekan tombol \*Back\* Android dua kali pada mode VR akan mengembalikan pengguna ke `Scene\_AR`.



\## Spesifikasi Pengembangan

\* \*\*Unity Version:\*\* Unity 6 (`6000.6.9f1`)

\* \*\*Render Pipeline:\*\* Built-in Render Pipeline

\* \*\*Target Platform:\*\* Android (Minimum API sesuai konfigurasi project)

\* \*\*SDK \& Package Utama:\*\*

&#x20; \* Vuforia Engine (`11.4.4`)

&#x20; \* TextMeshPro (`TMP`)

&#x20; \* Git LFS (untuk paket `.tgz` Vuforia berukuran > 100 MB)



\## Struktur Scene \& Script

\* `Assets/Scenes/Scene\_AR.unity` — Scene utama Augmented Reality (Cylinder Target).

\* `Assets/Scenes/Scene\_VR.unity` — Scene Virtual Reality showcase 3D tumbler.

\* `Assets/ARTargetHandler.cs` — Mengatur deteksi target AR dan perpindahan ke `Scene\_VR`.

\* `Assets/VRGazeInteraction.cs` — Mengatur kontrol kamera (Gyroscope/Swipe), \*gaze raycast\*, \*haptic feedback\*, UI otomatis, dan tombol \*Back\*.

\* `Assets/VRTumblerItem.cs` — Menyimpan data nama produk, harga, dan deskripsi tiap varian tumbler.



\---



\## Panduan Instalasi \& Menjalankan di Perangkat Android



\### Prasyarat di Komputer Penerima

1\. \*\*Git\*\* dan \*\*Git LFS\*\* sudah terinstal di komputer.

2\. \*\*Unity Hub\*\* dengan \*\*Unity Editor versi `6000.6.9f1`\*\* terinstal.

3\. Modul \*\*Android Build Support\*\* (termasuk \*\*Android SDK \& NDK Tools\*\* serta \*\*OpenJDK\*\*) terpasang pada Unity `6000.6.9f1` di Unity Hub.

4\. Perangkat \*\*HP Android\*\* yang mendukung ARCore/kamera dengan \*\*USB Debugging\*\* aktif.



\### Langkah 1: Clone Repository (Wajib dengan Git LFS)

1\. Buka Terminal / PowerShell di folder tujuan penyimpanan project.

2\. Pastikan Git LFS aktif:

&#x20;  ```bash

&#x20;  git lfs install



