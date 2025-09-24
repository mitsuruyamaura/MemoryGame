using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStateManager : AbstractSingleton<SceneStateManager> {

    [SerializeField]
    private Fade fade;

    [SerializeField, Header("フェードの時間")]
    private float fadeDuration = 1.0f;


    public async UniTask FadeIn() {
        fade.FadeIn(fadeDuration, () => FadeOut());
        await UniTask.Delay((int)fadeDuration * 100);
    }


    public void FadeOut() {
        fade.FadeOut(fadeDuration);
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