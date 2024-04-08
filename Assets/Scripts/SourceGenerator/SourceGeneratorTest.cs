using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceGeneratorTest : MonoBehaviour
{
    private void Start()
    {
        // 생성된 코드 사용
        string testText = SourceGenerated.Example.GetTestText();
        Debug.Log(testText);

        //
    }
}