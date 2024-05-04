using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPlayer : CharacterProperty
{
    int hashJumpAttack;
    Vector2 inputDir;
    Vector2 targetDir;

    bool isComboCheck = false;
    
    // Start is called before the first frame update
    void Start()
    {
        hashJumpAttack = Animator.StringToHash("JumpAttack");
    }

    // Update is called once per frame
    void Update()
    {
        targetDir.x = Input.GetAxis("Horizontal");
        targetDir.y = Input.GetAxis("Vertical");

        inputDir = Vector2.Lerp(inputDir, targetDir, Time.deltaTime * 10.0f);

        myAnim.SetFloat("x", inputDir.x);
        myAnim.SetFloat("y", inputDir.y);

        if(Input.GetKeyDown(KeyCode.F1))
        {
            myAnim.SetTrigger(hashJumpAttack);
        }

        if(!myAnim.GetBool("IsAttack") && Input.GetMouseButtonDown(0))
        {
            myAnim.SetTrigger("Attack");
        }
    }

    public void ComboCheck(bool v)
    {
        if(v)
        {
            StartCoroutine(ComboChecking());
        }
        else
        {
            isComboCheck = false;
        }
    }

    IEnumerator ComboChecking()
    {
        int clickCount = 0;
        isComboCheck = true;
        myAnim.SetBool("IsComboFailed", false);
        while (isComboCheck)
        {
            if(Input.GetMouseButtonDown(0))
            {
                clickCount++;
            }
            yield return null;
        }

        if(clickCount == 0)
        {
            myAnim.SetBool("IsComboFailed", true);
        }
    }
}
