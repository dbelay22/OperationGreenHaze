using UnityEngine;
using UnityEngine.SceneManagement;

namespace Yxp.Helpers
{
    public static class SceneHelper
    {
        public static void ReloadCurrentScene()
        {
            Debug.Log("About to reload current scene");
            // reload scene
            string curSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(curSceneName);
        }

    }
}