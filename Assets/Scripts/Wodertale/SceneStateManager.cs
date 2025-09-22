using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStateManager : MonoBehaviour {

    public static SceneStateManager instance;

    public Stage stage;

    [SerializeField]
    private Fade fade;

    [SerializeField, Header("フェードの時間")]
    private float fadeDuration = 1.0f;

    [SerializeField]
    private UnityEngine.UI.Image imgMask;


    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public async UniTask FadeIn() {
        fade.FadeIn(fadeDuration, () => FadeOut());
        await UniTask.Delay((int)fadeDuration * 100);
    }


    public void FadeOut() {
        fade.FadeOut(fadeDuration);
    }

    /// <summary>
    /// Stage シーンへ遷移
    /// </summary>
    /// <returns></returns>
    private void ActiveStageScene() {

        string oldSceneName = SceneManager.GetActiveScene().name;

        //Scene scene = SceneManager.GetSceneByName(nextLoadSceneName.ToString());

        //while (!scene.isLoaded) {
        //    yield return null;
        //}

        //SceneManager.SetActiveScene(scene);

        //stage.gameObject.SetActive(true);

        fade.FadeOut(fadeDuration);

        //SceneManager.UnloadSceneAsync(oldSceneName);
    }

    /// <summary>
    /// Battle シーンへの遷移
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadBattleScene() {

        SceneManager.LoadScene(SceneName.Battle.ToString(), LoadSceneMode.Additive);

        Scene scene = SceneManager.GetSceneByName(SceneName.Battle.ToString());

        yield return new WaitUntil(() => scene.isLoaded);

        stage.gameObject.SetActive(false);

        fade.FadeOut(fadeDuration);

        SceneManager.SetActiveScene(scene);
    }

    /// <summary>
    /// 指定したシーンへ遷移準備
    /// </summary>
    /// <param name="nextLoadSceneName"></param>
    public void PrepareteNextScene(SceneName nextLoadSceneName) {

        if (!fade) {
            // フェードインなし
            StartCoroutine(LoadNextScene(nextLoadSceneName));
        } else {
            // フェードインあり
            fade.FadeIn(fadeDuration, () => { StartCoroutine(LoadNextScene(nextLoadSceneName)); });
        }
    }

    /// <summary>
    /// 指定したシーンへ遷移
    /// </summary>
    /// <param name="nextLoadSceneName"></param>
    /// <returns></returns>
    private IEnumerator LoadNextScene(SceneName nextLoadSceneName) {

        SceneManager.LoadScene(nextLoadSceneName.ToString());
    
        // フェードインしている場合には
        if (fade) {
            // シーンの読み込み終了を待つ
            Scene scene = SceneManager.GetSceneByName(nextLoadSceneName.ToString());
            yield return new WaitUntil(() => scene.isLoaded);

            // フェードアウト
            fade.FadeOut(fadeDuration);
        }
    }

    public Scene GetScene(SceneName nextLoadSceneName) {
        return SceneManager.GetSceneByName(nextLoadSceneName.ToString());
    }
}