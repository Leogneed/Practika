using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPause : MonoBehaviour
{
    public bool PauseGame;
    public GameObject pauseGameMenu;
    public GameObject player; // �������� ������ �� ������

    private MonoBehaviour cameraController;

    private void Start()
    {
        // �����������, ��� � ������ ������ ���� ��������� ������, ������� �� ������ ���������
        cameraController = player.GetComponent<MonoBehaviour>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseGame)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseGameMenu.SetActive(false);
        Time.timeScale = 1.0f;
        PauseGame = false;
        Cursor.lockState = CursorLockMode.Locked; // ������������� ������
        Cursor.visible = false; // ������ ������
        cameraController.enabled = true; // �������� ���������� �������
    }

    public void Pause()
    {
        pauseGameMenu.SetActive(true);
        Time.timeScale = 0f;
        PauseGame = true;
        Cursor.lockState = CursorLockMode.None; // �������������� ������
        Cursor.visible = true; // �������� ������
        cameraController.enabled = false; // ��������� ���������� �������
    }

    public void LoadMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Menu");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1.0f;
    }
}
