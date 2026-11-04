using UnityEngine;

// 開始状態に合わせてスタート画面を表示・非表示にする。通信の方法は RemoteStartClient に任せる
public class StartScreen : MonoBehaviour
{
    public RemoteStartClient client;

    [Tooltip("表示を切り替える全画面のスタート画面")]
    public GameObject screen;

    private void Awake()
    {
        if (client == null || screen == null)
        {
            Debug.LogError("StartScreenの参照が設定されていません。インスペクタを確認してください。");
            return;
        }

        client.StartedChanged += ApplyStarted;
        ApplyStarted(client.IsStarted);
    }

    private void OnDestroy()
    {
        if (client != null)
            client.StartedChanged -= ApplyStarted;
    }

    private void ApplyStarted(bool started)
    {
        screen.SetActive(!started);
    }
}
