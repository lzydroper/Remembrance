using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Network
{
    public class LoadingCircleAnime : MonoBehaviour
    {
        [SerializeField] private Image loginCircleImg;
        private Tween _animeTween;
    
        [ContextMenu("StartAni")]
        public void StartAni()
        {
            // 暂停已有动画
            if (_animeTween != null &&
                _animeTween.IsActive())
            {
                _animeTween.Kill();
            }

            _animeTween = loginCircleImg.transform.DORotate(
                    new Vector3(0, 0, -360),
                    1f,                 // 一圈的时间，调整速度
                    RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear)
                .SetRelative(true);
        }

        [ContextMenu("StopAni")]
        public void StopAni()
        {
            if (_animeTween != null &&
                _animeTween.IsActive())
            {
                _animeTween.Kill();
                _animeTween = null;
            }
        }
    }
}