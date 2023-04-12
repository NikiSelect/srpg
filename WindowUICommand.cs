using UnityEngine;
public class WindowUICommand : MonoBehaviour
{
    //アニメーション
    private Animator myAnimator;
    public Animator MyAnimator => myAnimator ? myAnimator : (myAnimator = GetComponent<Animator>());
}
