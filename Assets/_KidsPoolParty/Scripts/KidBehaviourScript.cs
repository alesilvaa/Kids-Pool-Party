using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KidBehaviourScript : MonoBehaviour
{
    public SOKids kid;
    private string kidName;
    private bool isJumping = false;
    private ObjectMover objectMover;
   
    
    [SerializeField] private List<GameObject> _trailWater;
    
    public string kidNames=> kidName;
    private void Awake()
    {
        kidName = kid.name;
        objectMover = GetComponent<ObjectMover>();
    }
    
    public IEnumerator JumpToHerMom(Transform momPosition)
    {
        yield return new WaitForSeconds(0.1f);
        if (!isJumping)
        {
            isJumping = true;
            Debug.Log("Kid: " + kidName + " is jumping to her mom");
            SoundManager.Instance.PlaySoundJump();
            transform.DOJump(momPosition.position, 2f, 1, .25f)
                .SetEase(Ease.InOutSine);
            gameObject.GetComponent<BoxCollider>().enabled = false;
            gameObject.transform.SetParent(momPosition);
        }
    }
    
    public void DisableMovement()
    {
        objectMover.enabled = false;
        foreach (var trail in _trailWater)
        {
            trail.SetActive(false);
        }
    }
    
    
}