using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPause : MonoBehaviour
{
    public bool PauseGame;
    public GameObject pauseGameMenu;
    public GameObject player; // Добавьте ссылку на игрока

    private MonoBehaviour cameraController;

    private void Start()
    {
        // Предположим, что у вашего игрока есть компонент камеры, который вы хотите отключить
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
        Cursor.lockState = CursorLockMode.Locked; // Заблокировать курсор
        Cursor.visible = false; // Скрыть курсор
        cameraController.enabled = true; // Включить управление камерой
    }

    public void Pause()
    {
        pauseGameMenu.SetActive(true);
        Time.timeScale = 0f;
        PauseGame = true;
        Cursor.lockState = CursorLockMode.None; // Разблокировать курсор
        Cursor.visible = true; // Показать курсор
        cameraController.enabled = false; // Отключить управление камерой
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
