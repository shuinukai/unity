using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

// Firebase Realtime Databaseの値を定期的に確認し、開始状態が変わったら通知する
public class RemoteStartClient : MonoBehaviour
{
    [Header("接続先")]
    [Tooltip("Realtime DatabaseのURL。空なら通信せず、メニューからのテストだけ行える")]
    public string databaseUrl;
    public string roomId = "demo";

    [Header("通信")]
    [Min(0.2f)]
    public float pollInterval = 1f;
    [Min(1)]
    public int timeoutSeconds = 5;

    public event Action<bool> StartedChanged;

    public bool IsStarted { get; private set; }

    private string StartedUrl =>
        $"{databaseUrl.Trim().TrimEnd('/')}/rooms/{UnityWebRequest.EscapeURL(roomId)}/started.json";

    private void Start()
    {
        if (string.IsNullOrWhiteSpace(databaseUrl))
        {
            Debug.LogWarning("RemoteStartClientのDatabase Urlが未設定のため、通信しません。");
            return;
        }

        StartCoroutine(PollLoop());
    }

    private IEnumerator PollLoop()
    {
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(pollInterval);
        bool lastRequestFailed = false;

        while (true)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(StartedUrl))
            {
                request.timeout = timeoutSeconds;

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    lastRequestFailed = false;

                    // 値が未作成のときはnullが返るので、trueのときだけ開始扱いにする
                    SetStarted(request.downloadHandler.text.Trim() == "true");
                }
                else if (!lastRequestFailed)
                {
                    // 通信が切れている間、同じ警告を出し続けない
                    lastRequestFailed = true;
                    Debug.LogWarning($"開始状態を取得できませんでした: {request.error}");
                }
            }

            yield return wait;
        }
    }

    private void SetStarted(bool started)
    {
        if (IsStarted == started)
            return;

        IsStarted = started;
        StartedChanged?.Invoke(started);
    }

    // ---------- 通信なしのテスト用（再生中にコンポーネントのメニューから実行） ----------

    [ContextMenu("テスト：開始")]
    private void TestStart()
    {
        SetStarted(true);
    }

    [ContextMenu("テスト：リセット")]
    private void TestReset()
    {
        SetStarted(false);
    }
}
