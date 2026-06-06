using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UVSF.ComponentsCollection.Animations

{
	/// <summary>
	/// 由LeftControl控制的自动隐藏的侧边栏
	/// </summary>
	public class LeftControlHidenWindow : MonoBehaviour
	{
		public enum Direction { up, down, left, right }

		[Tooltip("要移动的UI对象")]
		public RectTransform moveObj;
		[Tooltip("隐藏方向")]
		public Direction dir = Direction.right;
		[Tooltip("动画时间")]
		public float duration = 0.4f;
		[Range(0, 1), Tooltip("延伸出边框的比例")]
		public float extendScale = 0.25f;

		private enum State
		{
            Hiden,
            Showing,
            Shown,
            Hiding
        }
		private State state;
		private Tweener tweener;

        private float height;//去掉延伸长度后的高
		private float width;//去掉延伸长度后的宽
		private float extend = 20;//延伸出边框的长度，自动计算
		private Vector2 originPos = new Vector2(0, 0);

		private void Start()
		{
			OnRectTransformDimensionsChange();
		}

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl))
			{
				switch (state)
				{
					case State.Hiden:
						Show();
						break;
					case State.Showing:
					case State.Shown:
                        break;
					case State.Hiding:
						tweener.Kill();
						Show();
						break;
                }
			}
			else
			{
				switch (state)
				{
					case State.Hiden:
					case State.Hiding:
						break;
					case State.Shown:
						Hide();
						break;
					case State.Showing:
                        tweener.Kill();
                        Hide();
                        break;
                }
			}
        }

        /// <summary>
        /// 每次更改分辨率都要重算锚点和延伸长度
        /// </summary>
        private void OnRectTransformDimensionsChange()
		{
			if (!enabled) extendScale = 1f;
			switch (dir)
			{
				case Direction.up:
				case Direction.down:
					extend = moveObj.rect.height * extendScale;
					break;
				case Direction.left:
				case Direction.right:
					extend = moveObj.rect.width * extendScale;
					break;
			}
			height = moveObj.rect.height - extend;
			width = moveObj.rect.width - extend;
			moveObj.DOAnchorPos(originPos, duration);
			Hide();
		}

		private void StateToNext()
		{
			state = state switch
            {
                State.Hiden => State.Showing,
                State.Showing => State.Shown,
                State.Shown => State.Hiding,
                State.Hiding => State.Hiden,
                _ => state
            };
        }
        /// <summary>显示侧边栏</summary>
        private void Show()
		{
            state = State.Showing;

            switch (dir)
			{
				case Direction.up:
				case Direction.down:
                    tweener = moveObj.DOAnchorPosY(originPos.y, duration)
						.SetEase(Ease.OutQuart)
						.OnComplete(StateToNext);
					break;
				case Direction.left:
				case Direction.right:
                    tweener = moveObj.DOAnchorPosX(originPos.x, duration)
						.SetEase(Ease.OutQuart)
						.OnComplete(StateToNext);
					break;
				default:
					break;
			}

		}

		/// <summary>隐藏侧边栏</summary>
		private void Hide()
		{
			state = State.Hiding;
            switch (dir)
			{
				case Direction.up:
                    tweener = moveObj.DOAnchorPosY(originPos.y + height, duration)
						.SetEase(Ease.OutQuart)
						.OnComplete(StateToNext);
					break;
				case Direction.down:
                    tweener = moveObj.DOAnchorPosY(originPos.y - height, duration)
						.SetEase(Ease.OutQuart)
						.OnComplete(StateToNext);
					break;
				case Direction.left:
                    tweener = moveObj.DOAnchorPosX(originPos.x - width, duration)
						.SetEase(Ease.OutQuart)
						.OnComplete(StateToNext);
					break;
				case Direction.right:
                    tweener = moveObj.DOAnchorPosX(originPos.x + width, duration)
						.SetEase(Ease.OutQuart)
						.OnComplete(StateToNext);
					break;
				default:
					break;
			}

		}

		/// <summary>外部控制显示一下Tips</summary>
		public void ToShow()
		{
			Show();
		}

		public void ToHide()
		{
			Hide();
		}
	}
}
