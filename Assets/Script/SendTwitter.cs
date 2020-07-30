using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Twitter;
using Battlehub.RTHandles;

public class SendTwitter : MonoBehaviour
{
    [SerializeField] string CONSUMER_KEY;
    [SerializeField] string CONSUMER_SECRET;
    [SerializeField] Text Text;
    [SerializeField] InputField Input;
    [SerializeField] Image Image;
    [SerializeField] Sprite Sprite;
    [SerializeField] RuntimeSceneComponent RSC;
    [SerializeField] GameObject UI;

    RequestTokenResponse Request;
    AccessTokenResponse Access;
    GameObject SendObj;

    string TAG_URL = "\n#VRMお人形遊び https://120byte.booth.pm/items/1654585";

    string FileName = "Twitter";

    string Ready = "Send";
    string Unready = "Get PIN";
    string Authentication = "Send PIN";
    string PlaceholderReady = "Tweet";
    string PlaceholderUnready = "Please input PIN here.";

    void Start()
    {
        if (!File.Exists(FileName))
        {
            return;
        }

        // 読込
        string config = File.ReadAllText(FileName);
        config = AESCryption.Decrypt(config);
        string[] token = config.Split(',');

        Access = new AccessTokenResponse
        {
            Token = token[0],
            TokenSecret = token[1],
            UserId = token[2],
            ScreenName = token[3]
        };

        // 認証済
        Text.text = Ready;
        Input.placeholder.GetComponent<Text>().text = PlaceholderReady;
    }

    void Update()
    {
        var text = Input.placeholder.GetComponent<Text>().text;
        if (text == PlaceholderUnready)
        {
            // PIN入力要求
            return;
        }

        var active = RSC.Editor.Selection.activeGameObject;
        if (active == null || !active.name.Contains("Photo"))
        {
            // 画像オブジェクト未選択
            Image.sprite = Sprite;
        }
        else
        {
            // 送信可能
            Text.text = Ready;
            var tex = (Texture2D)active.GetComponent<Renderer>().material.GetTexture("_MainTex");
            Image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        }
    }

    public void OnChanged()
    {
        var text = Input.placeholder.GetComponent<Text>().text;
        if (text == PlaceholderUnready)
        {
            // 未認証
            if (Input.text == "")
            {
                Text.text = Unready;
            }
            else
            {
                Text.text = Authentication;
            }
        }
    }

    public void OnClick()
    {
        if (Text.text == Unready)
        {
            // 未認証→認証画面
            StartCoroutine(API.GetRequestToken(CONSUMER_KEY, CONSUMER_SECRET, new RequestTokenCallback(this.OnRequestTokenCallback)));
        }
        if (Text.text == Authentication)
        {
            // 認証
            StartCoroutine(API.GetAccessToken(CONSUMER_KEY, CONSUMER_SECRET, Request.Token, Input.text, new AccessTokenCallback(this.OnAccessTokenCallback)));
        }
        if (Text.text == Ready)
        {
            // 認証済
            var active = RSC.Editor.Selection.activeGameObject;
            if (active != null && active.name.Contains("Photo"))
            {
                // 送信
                var path = active.GetComponent<ImageFile>().Path;
                StartCoroutine(API.PostTweetWithMedia(Input.text + TAG_URL, path, CONSUMER_KEY, CONSUMER_SECRET, Access, new PostTweetCallback(OnPostTweet)));
                SendObj = active;
            }
        }
    }

    void OnRequestTokenCallback(bool success, Twitter.RequestTokenResponse response)
    {
        if (success)
        {
            // 認証要求
            Request = response;
            API.OpenAuthorizationPage(response.Token);
        }
    }

    void OnAccessTokenCallback(bool success, Twitter.AccessTokenResponse response)
    {
        if (success)
        {
            // 認証
            Access = response;
            Text.text = Ready;
            Input.placeholder.GetComponent<Text>().text = PlaceholderReady;
            Input.text = "";

            // 保存
            string config = "";
            config += Access.Token + ",";
            config += Access.TokenSecret + ",";
            config += Access.UserId + ",";
            config += Access.ScreenName + ",";
            config = AESCryption.Encrypt(config);
            File.WriteAllText(FileName, config);
        }
    }

    void OnPostTweet(bool success)
    {
        if (success)
        {
            // 送信完了
            Input.text = "";
            UI.SetActive(false);
            Destroy(SendObj);
        }
    }
}
