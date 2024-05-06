using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public enum Garde
{
    Common,
    Rare,
    Legend
}

public enum Size
{
    POT64,
    POT128,
    POT256,
    POT512,
    POT1024
}

public class Capture : MonoBehaviour
{
    [Tooltip("사진을 한번 찍고 다시 사진을 찍으면 이전 사진은 날아감. 주의!")]
    public Camera cam;
    public RenderTexture rt;
    public Image image;

    public Garde garde;
    public Size size;

    public GameObject[] obj;
    int nowCount = 0;

    private void Start() 
    {
        cam = Camera.main;
        SettingColor();
        SettingSize();
    }

    public void Create()
    {
        StartCoroutine(CaptureImage());
    }

    IEnumerator CaptureImage()
    {
        yield return null;

        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false, true);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

        yield return null;

        var data = tex.EncodeToPNG();
        string name = "Test";
        string extention = ".png";
        string path = Application.dataPath + "/Capture/";

        Debug.Log(path);

        if(!Directory.Exists(path)) Directory.CreateDirectory(path);

        File.WriteAllBytes(path + name + extention, data);

        yield return null;
    }

    IEnumerator AllCaptureImage()
    {
        while(nowCount < obj.Length)
        {
            var nowObj = Instantiate(obj[nowCount].gameObject);

            Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false, true);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

            yield return null;

            var data = tex.EncodeToPNG();
            string name = $"Thumbnail_{obj[nowCount].gameObject.name}";
            string extention = ".png";
            string path = Application.dataPath + "/Capture/";

            Debug.Log(path);

            if(!Directory.Exists(path)) Directory.CreateDirectory(path);

            File.WriteAllBytes(path + name + extention, data);

            yield return null;

            DestroyImmediate(nowObj);
            nowCount++;

            yield return null;
        }
    }

    void SettingColor()
    {
        switch (garde)
        {
            case Garde.Common:
            cam.backgroundColor = Color.white;
            image.color = Color.white;
            break;
            case Garde.Rare:
            cam.backgroundColor = Color.green;
            image.color = Color.green;
            break;
            case Garde.Legend:
            cam.backgroundColor = Color.yellow;
            image.color = Color.yellow;
            break;
        }
    }

    void SettingSize()
    {
        switch(size)
        {
            case Size.POT64:
            rt.width = 64;
            rt.height = 64;
            break;
            case Size.POT128:
            rt.width = 128;
            rt.height = 128;
            break;
            case Size.POT256:
            rt.width = 256;
            rt.height = 256;
            break;
            case Size.POT512:
            rt.width = 512;
            rt.height = 512;
            break;
            case Size.POT1024:
            rt.width = 1024;
            rt.height = 1024;
            break;
        }
    }
}
