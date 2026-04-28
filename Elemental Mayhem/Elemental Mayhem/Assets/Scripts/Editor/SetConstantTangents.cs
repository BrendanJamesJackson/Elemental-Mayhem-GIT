using UnityEditor;
using UnityEngine;

public class SetConstantTangents
{
    [MenuItem("Tools/Set All Animations To Constant")]
    static void SetTangents()
    {
        var clips = Selection.GetFiltered<AnimationClip>(SelectionMode.DeepAssets);

        foreach (var clip in clips)
        {
            var bindings = AnimationUtility.GetCurveBindings(clip);

            foreach (var binding in bindings)
            {
                var curve = AnimationUtility.GetEditorCurve(clip, binding);

                for (int i = 0; i < curve.keys.Length; i++)
                {
                    AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Constant);
                    AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Constant);
                }

                AnimationUtility.SetEditorCurve(clip, binding, curve);
            }

            Debug.Log($"Fixed: {clip.name}");
        }
    }
}