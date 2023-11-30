using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
using UnityEditor;
#endif

#region Classes
[Serializable]
public class AIPassthroughData
{
    public string passthroughString;
    public int passthroughInt;
    public float passthroughFloat;
}

[Serializable]
public class ChatResponse
{
    public string id;
    //public int created;
    public string model;
    public ChatOption[] choices;
}

[Serializable]
public class ChatOption
{
    public ChatMessage message;
    public int index;
    public string finish_reason;
}

[Serializable]
public class ChatMessage
{
    public string role;
    public string content;
}

[Serializable]
public class ImageStatus
{
    public string id;
    public string status;
    public ImageResult[] output;
}

[Serializable]
public class ImageResult
{
    public int seed;
    public string image;
}

public enum AudioVoice
{
    MollyNeural,
    AnnetteNeural,
    CarlyNeural,
    DarrenNeural,
    DuncanNeural,
    ElsieNeural,
    FreyaNeural,
    JoanneNeural,
    KenNeural,
    KimNeural,
    NatashaNeural,
    NeilNeural,
    TimNeural,
    TinaNeural,
    WilliamNeural
}

[Serializable]
public class VoiceParams
{
    public string speed = "20%";
    public AudioVoice voice = AudioVoice.MollyNeural;
    public string pitch = "-5%";
}
#endregion

[ExecuteInEditMode]
public class AIManager : MonoBehaviour
{
    public static AIManager Instance;

    public bool test = false;
    public bool gpt4 = false;

    private void Awake()
    {
        Instance = this;
    }
    #region Text
    public void GetText(string input, Action<string, AIPassthroughData> callback, AIPassthroughData data)
    {
        StartCoroutine(GetTextCoroutine(input, callback, data));
    }
    private IEnumerator GetTextCoroutine(string input, Action<string, AIPassthroughData> callback, AIPassthroughData data)
    {
        string submitText = cleanForJSON(input);
        string url = "https://api.openai.com/v1/chat/completions";
        string model = gpt4 ? "gpt-4" : "gpt-3.5-turbo";
        string secret = "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";
        string json = "{" +
            "\"messages\": [{\"role\": \"user\", \"content\": \"" + submitText + "\"}]," +
            "\"temperature\": 0.7," +
            "\"max_tokens\": 2000," +
            "\"top_p\": 1," +
            "\"frequency_penalty\": 0.3," +
            "\"presence_penalty\": 0.3," +
            "\"model\": \"" + model + "\"" +
            "}";
        Debug.Log(json);
        using(UnityWebRequest request = new UnityWebRequest(url))
        {
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + secret);
            request.disposeUploadHandlerOnDispose = true;
            request.disposeDownloadHandlerOnDispose = true;
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                ChatResponse chatResponse = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
                callback(chatResponse.choices[0].message.content, data);
            }
        }
    }
    #endregion
    #region Images
    public void GetImage(string input, Action<Sprite, AIPassthroughData> callback, AIPassthroughData data)
    {
        StartCoroutine(GetImage(input, callback, null, data));
    }
    public void GetImage(string input, Action<Texture, AIPassthroughData> callback, AIPassthroughData data)
    {
        StartCoroutine(GetImage(input, null, callback, data));
    }
    private IEnumerator GetImage(string input, Action<Sprite, AIPassthroughData> callbackSprite, Action<Texture, AIPassthroughData> callbackTexture, AIPassthroughData data)
    {
        input = cleanForJSON(input);
        int seed = Mathf.FloorToInt(UnityEngine.Random.value * 10000);
        string url = "https://api.runpod.ai/v1/stable-diffusion-v1/run";
        string key = "G0U3LW2ZF1FM8DKQRZY8U0HOK5LZ2ENLQ5UE9ISD";
        string json = "{" +
            "\"input\": {\"prompt\": \"" +
            input + "\"" +
            ((", \"seed\": " + seed)) +
            ", \"negative_prompt\": \"big boobs\"" +
            ", \"num_outputs\": 1" +
            ", \"num_inference_steps\": 48" +
            "}}";
        using(UnityWebRequest request = new UnityWebRequest(url))
        {
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + key);
            request.disposeUploadHandlerOnDispose = true;
            request.disposeDownloadHandlerOnDispose = true;
            yield return request.SendWebRequest();

            ImageStatus response = JsonUtility.FromJson<ImageStatus>(request?.downloadHandler?.text);
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(input + ": " + request.error);
            }
            else
            {
                response = JsonUtility.FromJson<ImageStatus>(request.downloadHandler.text);
            }
            url = "https://api.runpod.ai/v1/stable-diffusion-v1/status/" + response.id;

            while (response.output == null)
            {
#if UNITY_EDITOR
                yield return new EditorWaitForSeconds(2f);
#else
                yield return new WaitForSeconds(2f);
#endif
                using(UnityWebRequest request2 = new UnityWebRequest(url))
                {
                    request2.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
                    request2.downloadHandler = new DownloadHandlerBuffer();
                    request2.method = UnityWebRequest.kHttpVerbPOST;
                    request2.SetRequestHeader("Content-Type", "application/json");
                    request2.SetRequestHeader("Authorization", "Bearer " + key);
                    request2.disposeUploadHandlerOnDispose = true;
                    request2.disposeDownloadHandlerOnDispose = true;
                    yield return request2.SendWebRequest();
                    if (request2.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError(input + ": " + request2.error);
                    }
                    else
                    {
                        response = JsonUtility.FromJson<ImageStatus>(request2.downloadHandler.text);
                    }
                }
            }

            url = response.output[0].image;
            using(UnityWebRequest wwwTex = UnityWebRequestTexture.GetTexture(url))
            {
                yield return wwwTex.SendWebRequest();

                if (wwwTex.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(input + ": " + wwwTex.error);
                }
                else
                {
                    Texture myTexture = ((DownloadHandlerTexture) wwwTex.downloadHandler).texture;
                    if (callbackSprite != null)
                    {
                        callbackSprite(Sprite.Create((Texture2D) myTexture, new Rect(0.0f, 0.0f, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f)), data);
                    }
                    if (callbackTexture != null)
                    {
                        callbackTexture(myTexture, data);
                    }
                }
            }
        }
    }
    #endregion
    #region Voice
    public static readonly string FetchTokenUri =
        "https://eastus.api.cognitive.microsoft.com/sts/v1.0/issuetoken";
    private string token = "";

    private Dictionary<AudioVoice, string> voices = new Dictionary<AudioVoice, string>()
    { { AudioVoice.MollyNeural, "<voice xml:lang='en-NZ' xml:gender='Female' name='en-NZ-MollyNeural'>" }, { AudioVoice.AnnetteNeural, "<voice xml:lang='en-AU' xml:gender='Female' name='en-AU-AnnetteNeural'>" }, { AudioVoice.CarlyNeural, "<voice xml:lang='en-AU' xml:gender='Female' name='en-AU-CarlyNeural'>" }, { AudioVoice.ElsieNeural, "<voice xml:lang='en-AU' xml:gender='Female' name='en-AU-ElsieNeural'>" }, { AudioVoice.FreyaNeural, "<voice xml:lang='en-AU' xml:gender='Female' name='en-AU-FreyaNeural'>" }, { AudioVoice.JoanneNeural, "<voice xml:lang='en-AU' xml:gender='Female' name='en-AU-JoanneNeural'>" }, { AudioVoice.KimNeural, "<voice xml:lang='en-AU' xml:gender='Female' name='en-AU-KimNeural'>" }, { AudioVoice.NatashaNeural, "<voice xml:lang='en-AU' xml:gender='Female' name='en-AU-NatashaNeural'>" }, { AudioVoice.TinaNeural, "<voice xml:lang='en-AU' xml:gender='Female' name='en-AU-TinaNeural'>" }, { AudioVoice.DarrenNeural, "<voice xml:lang='en-AU' xml:gender='Male' name='en-AU-DarrenNeural'>" }, { AudioVoice.DuncanNeural, "<voice xml:lang='en-AU' xml:gender='Male' name='en-AU-DuncanNeural'>" }, { AudioVoice.KenNeural, "<voice xml:lang='en-AU' xml:gender='Male' name='en-AU-KenNeural'>" }, { AudioVoice.NeilNeural, "<voice xml:lang='en-AU' xml:gender='Male' name='en-AU-NeilNeural'>" }, { AudioVoice.TimNeural, "<voice xml:lang='en-AU' xml:gender='Male' name='en-AU-TimNeural'>" }, { AudioVoice.WilliamNeural, "<voice xml:lang='en-AU' xml:gender='Male' name='en-AU-WilliamNeural'>" },
    };

    public void Authentication(string subscriptionKey, string input, Action<AudioClip> callback, Action<byte[]> callbackWav, VoiceParams voiceParams)
    {
        StartCoroutine(FetchToken(FetchTokenUri, subscriptionKey, input, callback, callbackWav, voiceParams));
    }

    IEnumerator FetchToken(string url, string subscriptionKey, string input, Action<AudioClip> callback, Action<byte[]> callbackWav, VoiceParams voiceParams)
    {
        UnityWebRequest request = new UnityWebRequest(url);
        request.SetRequestHeader("Ocp-Apim-Subscription-Key", subscriptionKey);
        request.SetRequestHeader("Content-type", "application/x-www-form-urlencoded");
        request.uploadHandler = new UploadHandlerRaw(new byte[0]);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.method = UnityWebRequest.kHttpVerbPOST;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            token = request.downloadHandler.text;
            StartCoroutine(GetVoiceCoroutine(input, callback, callbackWav, voiceParams));
            request.Dispose();
        }
    }
    public void GetVoice(string input, Action<AudioClip> callback, Action<byte[]> callbackWav, VoiceParams voiceParams = null)
    {
        if (token == "")
        {
            string key = "72fda69caa0e40959a34dcb6a4622374";
            Authentication(key, input, callback, callbackWav, voiceParams);
        }
        else
        {
            StartCoroutine(GetVoiceCoroutine(input, callback, callbackWav, voiceParams));
        }
    }
    public void GetVoice(string input, Action<AudioClip> callback, VoiceParams voiceParams = null)
    {
        GetVoice(input, callback, null, voiceParams);
    }
    private IEnumerator GetVoiceCoroutine(string input, Action<AudioClip> callback, Action<byte[]> callbackWav, VoiceParams voiceParams = null)
    {
        /*
        POST /cognitiveservices/v1 HTTP/1.1

        X-Microsoft-OutputFormat: riff-24khz-16bit-mono-pcm
        Content-Type: application/ssml+xml
        Host: westus.tts.speech.microsoft.com
        Content-Length: <Length>
        Authorization: Bearer [Base64 access_token]
        User-Agent: <Your application name>

        <speak version='1.0' xml:lang='en-US'><voice xml:lang='en-US' xml:gender='Male'
            name='en-US-ChristopherNeural'>
                Microsoft Speech Service Text-to-Speech API
        </voice></speak>


        https://eastus.api.cognitive.microsoft.com/sts/v1.0/issueToken
        */

        if (voiceParams == null)
        {
            voiceParams = new VoiceParams();
        }
        string submitText = input.Replace("&", "and").Replace("!", ".").Replace("<", "less than").Replace(">", "greather than");
        string url = "https://eastus.tts.speech.microsoft.com/cognitiveservices/v1";
        string xml = "<speak version='1.0' xml:lang='en-NZ'>" +
            voices[(voiceParams == null ? AudioVoice.MollyNeural : voiceParams.voice)] +
            "<prosody rate='" +
            (voiceParams == null ? "20%" : voiceParams.speed) +
            "' pitch='" +
            (voiceParams == null ? "-5%" : voiceParams.pitch) +
            "'>" +
            submitText +
            "</prosody></voice></speak>";

        using(UnityWebRequest request = new UnityWebRequest(url))
        {
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(xml));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("X-Microsoft-OutputFormat", "riff-24khz-16bit-mono-pcm");
            request.SetRequestHeader("Content-Type", "application/ssml+xml");
            request.SetRequestHeader("Authorization", "Bearer " + token);
            request.SetRequestHeader("User-Agent", "dgeisertTTS");
            request.disposeUploadHandlerOnDispose = true;
            request.disposeDownloadHandlerOnDispose = true;
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                byte[] buffer = request.downloadHandler.data;
                if (callbackWav != null)
                {
                    callbackWav(buffer);
                }
                if (callback != null)
                {
                    AudioClip clip = AudioClip.Create(name, buffer.Length, 1, 24000, false);
                    int bytesPerSample = 2; // e.g. 2 bytes per sample (16 bit sound mono)
                    int sampleCount = buffer.Length / bytesPerSample;

                    // Allocate memory (supporting left channel only)
                    float[] unityData = new float[sampleCount];

                    int pos = 0;
                    // Write to double array/s:
                    int i = 0;
                    while (pos < buffer.Length)
                    {
                        unityData[i] = BytesToFloat(buffer[pos], buffer[pos + 1]);
                        pos += 2;
                        i++;
                    }
                    clip.SetData(unityData, 0);
                    callback(clip);
                }
            }
        }
    }
    #endregion
    #region Util

    private static float BytesToFloat(byte firstByte, byte secondByte)
    {
        // Convert two bytes to one short (little endian)
        short s = (short) ((secondByte << 8) | firstByte);

        // Convert to range from -1 to (just below) 1
        return s / 32768.0F;
    }
    public static string cleanForJSON(string s)
    {
        if (s == null || s.Length == 0)
        {
            return "";
        }

        char c = '\0';
        int i;
        int len = s.Length;
        StringBuilder sb = new StringBuilder(len + 4);
        String t;

        for (i = 0; i < len; i += 1)
        {
            c = s[i];
            switch (c)
            {
                case '\\':
                case '"':
                    sb.Append('\\');
                    sb.Append(c);
                    break;
                case '/':
                    sb.Append('\\');
                    sb.Append(c);
                    break;
                case '\b':
                    sb.Append("\\b");
                    break;
                case '\t':
                    sb.Append("\\t");
                    break;
                case '\n':
                    sb.Append("\\n");
                    break;
                case '\f':
                    sb.Append("\\f");
                    break;
                case '\r':
                    sb.Append("\\r");
                    break;
                default:
                    if (c < ' ')
                    {
                        t = "000" + String.Format("X", c);
                        sb.Append("\\u" + t.Substring(t.Length - 4));
                    }
                    else
                    {
                        sb.Append(c);
                    }
                    break;
            }
        }
        return sb.ToString();
    }
    #endregion
}