using Android.Content;
using Android.Util;
using Android.Widget;

namespace DenunciaFitosanitaria.CustomViews
{
    /// <summary>
    /// Square image view.
    /// </summary>
    public class SquareImageView : ImageView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:DenunciaFitosanitaria.CustomViews.SquareImageView"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="attrs">Attrs.</param>
        public SquareImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DenunciaFitosanitaria.CustomViews.SquareImageView"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="attrs">Attrs.</param>
        /// <param name="defStyleAttr">Def style attr.</param>
        /// <param name="defStyleRes">Def style res.</param>
        public SquareImageView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        /// <summary>
        /// Ons the measure.
        /// </summary>
        /// <param name="widthMeasureSpec">Width measure spec.</param>
        /// <param name="heightMeasureSpec">Height measure spec.</param>
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            SetMeasuredDimension(MeasuredWidth, MeasuredHeight);
        }
    }
}
