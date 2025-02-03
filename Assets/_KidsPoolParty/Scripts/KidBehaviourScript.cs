using System.Collections;
using UnityEngine;
using DG.Tweening;

public class KidBehaviourScript : MonoBehaviour
{
    public SOKids kid;
    private string kidName;
    private bool isJumping = false;

    public string kidNames=> kidName;
    private void Awake()
    {
        kidName = kid.name;
    }
    
    public IEnumerator JumpToHerMom(Transform momPosition)
    {
        yield return new WaitForSeconds(0.4f);
        if (!isJumping)
        {
            isJumping = true;
            Debug.Log("Kid: " + kidName + " is jumping to her mom");

            transform.DOJump(momPosition.position, 2f, 1, 1f)
                .SetEase(Ease.InOutSine);
            gameObject.transform.SetParent(momPosition);
        }
    }
}