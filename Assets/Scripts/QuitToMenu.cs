using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitToMenu : MonoBehaviour

{
    public void QuittoMenu(string name)
    {
        SceneManager.LoadScene(name);
    }
}
