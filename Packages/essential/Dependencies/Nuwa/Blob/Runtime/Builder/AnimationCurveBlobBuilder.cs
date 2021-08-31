#if ENABLE_UNITY_ANIMATION

using Unity.Animation;
using Unity.Animation.Hybrid;
using Unity.Entities;
using AnimationCurve = UnityEngine.AnimationCurve;

namespace Nuwa.Blob
{
    public class AnimationCurveBlobBuilder : Builder<AnimationCurveBlob>
    {
        public AnimationCurve Value;
        public override void Build(BlobBuilder builder, ref AnimationCurveBlob data)
        {
            CurveConversion.FillAnimationCurveBlob(Value, ref builder, ref data);
        }
    }
}

#endif