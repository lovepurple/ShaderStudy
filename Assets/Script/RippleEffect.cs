/********************************************************************
	created:  2018-5-23 11:10:31
	filename: RippleEffect.cs
	author:	  songguangze@outlook.com
	
	purpose:  屏幕水波纹扭曲效果
*********************************************************************/
using UnityEngine;
using UnityEngine.Rendering;

public class RippleEffect : MonoBehaviour
{
    //波纹梯度曲线
    public AnimationCurve RippleCurve = new AnimationCurve(
        new Keyframe(0.00f, 0.50f, 0, 0),
        new Keyframe(0.05f, 1.00f, 0, 0),
        new Keyframe(0.15f, 0.10f, 0, 0),
        new Keyframe(0.25f, 0.80f, 0, 0),
        new Keyframe(0.35f, 0.30f, 0, 0),
        new Keyframe(0.45f, 0.60f, 0, 0),
        new Keyframe(0.55f, 0.40f, 0, 0),
        new Keyframe(0.65f, 0.55f, 0, 0),
        new Keyframe(0.75f, 0.46f, 0, 0),
        new Keyframe(0.85f, 0.52f, 0, 0),
        new Keyframe(0.99f, 0.50f, 0, 0)
    );

    //波纹间隔
    public float RippleInterval = 3f;

    //波纹反射的颜色
    public Color RippleReflectionColor = new Color(0.5f, 0.5f, 0.5f);



    private Texture2D m_rippleGradientTexture = null;
    private Material m_rippleEffectMaterial = null;


    /// <summary>
    /// 将AnimationCurve 转换为图像
    /// </summary>
    private void ConvertRippleCurveToTexture()
    {
        if (!m_rippleGradientTexture)
        {
            m_rippleGradientTexture = new Texture2D(128, 1, TextureFormat.Alpha8, false);
            m_rippleGradientTexture.filterMode = FilterMode.Bilinear;
            m_rippleGradientTexture.wrapMode = TextureWrapMode.Clamp;
        }

        for (int i = 0; i < m_rippleGradientTexture.width; ++i)
        {
            //整个curve 分成 1/width 份 width 越大梯度越精细
            float curveX = 1.0f / this.m_rippleGradientTexture.width * i;
            float curveValue = RippleCurve.Evaluate(curveX);

            this.m_rippleGradientTexture.SetPixel(i, 0, new Color(curveValue, curveValue, curveValue, curveValue));
        }
        this.m_rippleGradientTexture.Apply();
    }

    public void Update()
    {


    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        //CommandBuffer buffer =  Camera.main.AddCommandBuffer(CameraEvent.)

    }


    private void CheckAndMakeMaterial()
    {
        if (!this.m_rippleEffectMaterial)
            this.m_rippleEffectMaterial = new Material(Shader.Find("Hidden/RippleEffect"));

        this.m_rippleEffectMaterial.SetTexture("_GradientTex", this.m_rippleGradientTexture);

    }

}
