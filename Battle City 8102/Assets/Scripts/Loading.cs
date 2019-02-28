using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using FairyGUI;

public class Globe
{
    public static string nextSceneName;
}

public class Loading : MonoBehaviour
{
    private GComponent loadingComponent;
    private GProgressBar loadingBar;
    //private GButton loadingButton;

    private float loadingSpeed = 1;

    private float targetValue;

    private AsyncOperation operation;

    // Use this for initialization
    void Start()
    {
        loadingComponent = GetComponent<UIPanel>().ui;
        loadingBar = loadingComponent.GetChild("loadingBar").asProgress;
        //loadingButton = loadingComponent.GetChild("loadingButton").asButton;

        //loadingButton.onClick.Add(() => { loadingBar.value += 10; });

        loadingBar.value = 0.0f;
        Debug.Log(loadingBar.value);

        if (SceneManager.GetActiveScene().name == "Loading")
        {
            //启动协程
            StartCoroutine(AsyncLoading());
        }
    }

    IEnumerator AsyncLoading()
    {
        operation = SceneManager.LoadSceneAsync(Globe.nextSceneName);
        //阻止当加载完成自动切换
        operation.allowSceneActivation = false;

        yield return operation;
    }

    // Update is called once per frame
    void Update()
    {
        targetValue = operation.progress * 100;

        if (operation.progress >= 0.9f)
        {
            //operation.progress的值最大为0.9
            targetValue = 1.0f * 100;
        }

        if (targetValue != loadingBar.value)
        {
            //插值运算
            loadingBar.value = Mathf.Lerp((float)loadingBar.value, targetValue, Time.deltaTime * loadingSpeed);
            if (Mathf.Abs((float)loadingBar.value - targetValue) < 0.01f)
            {
                loadingBar.value = targetValue;
            }
        }

        if ((int)loadingBar.value == 100)
        {
            //允许异步加载完毕后自动切换场景
            operation.allowSceneActivation = true;
        }
    }
}

