using System;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public AnimationController animationController;
    public Color holoColorSlider;
    public Light light;
    public float reflectSlider;
    public Color rimColorSlider;
    public float rimPowerSlider;
    public float sliceAmounSlider;
    public Quaternion tempRot;

    private void OnGUI()
    {
		this.animationController = GameObject.FindObjectOfType(typeof(AnimationController)) as AnimationController;

        //this.animationController = Object.FindObjectOfType(typeof(AnimationController)) as AnimationController;
        if (this.animationController.customInput)
        {
            GUI.Box(new Rect(10f, 10f, 500f, 650f), string.Empty);
            this.holoColorSlider.r = GUI.HorizontalSlider(new Rect(25f, 150f, 300f, 10f), this.holoColorSlider.r, 0f, 1f);
            GUI.Label(new Rect(350f, 145f, 1000f, 20f), "Holographic Color: Red");
            this.holoColorSlider.g = GUI.HorizontalSlider(new Rect(25f, 180f, 300f, 10f), this.holoColorSlider.g, 0f, 1f);
            GUI.Label(new Rect(350f, 175f, 1000f, 20f), "Holographic Color: Green");
            this.holoColorSlider.b = GUI.HorizontalSlider(new Rect(25f, 210f, 300f, 10f), this.holoColorSlider.b, 0f, 1f);
            GUI.Label(new Rect(350f, 205f, 1000f, 20f), "Holographic Color: Blue");
            this.rimColorSlider.r = GUI.HorizontalSlider(new Rect(25f, 260f, 300f, 10f), this.rimColorSlider.r, 0f, 1f);
            GUI.Label(new Rect(350f, 255f, 1000f, 20f), "Rim Color: Red");
            this.rimColorSlider.g = GUI.HorizontalSlider(new Rect(25f, 290f, 300f, 10f), this.rimColorSlider.g, 0f, 1f);
            GUI.Label(new Rect(350f, 285f, 1000f, 20f), "Rim Color: Green");
            this.rimColorSlider.b = GUI.HorizontalSlider(new Rect(25f, 310f, 300f, 10f), this.rimColorSlider.b, 0f, 1f);
            GUI.Label(new Rect(350f, 305f, 1000f, 20f), "Rim Color: Blue");
            this.rimPowerSlider = GUI.HorizontalSlider(new Rect(25f, 360f, 300f, 10f), this.rimPowerSlider, 0f, 1f);
            GUI.Label(new Rect(350f, 355f, 1000f, 20f), "Rim Power");
            this.reflectSlider = GUI.HorizontalSlider(new Rect(25f, 390f, 300f, 10f), this.reflectSlider, 0f, 1f);
            GUI.Label(new Rect(350f, 385f, 1000f, 20f), "Reflect Power");
            this.sliceAmounSlider = GUI.HorizontalSlider(new Rect(25f, 420f, 300f, 10f), this.sliceAmounSlider, 0f, 1f);
            GUI.Label(new Rect(350f, 415f, 1000f, 20f), "Slice");
            this.tempRot.x = GUI.HorizontalSlider(new Rect(25f, 500f, 300f, 10f), this.tempRot.x, 0f, 2f);
            GUI.Label(new Rect(350f, 495f, 1000f, 20f), "Directional Light Rotation: X");
            this.tempRot.y = GUI.HorizontalSlider(new Rect(25f, 530f, 300f, 10f), this.tempRot.y, 0f, 2f);
            GUI.Label(new Rect(350f, 525f, 1000f, 20f), "Directional Light Rotation: Y");
        }
        GUI.Label(new Rect(100f, 30f, 1000f, 20f), "Press Space to start or pause the shader");
        GUI.Label(new Rect(100f, 50f, 1000f, 20f), "(If you are in the slider menu, close it with T, and then press Space");
        GUI.Label(new Rect(100f, 70f, 1000f, 20f), "Press P to restart the shader");
        GUI.Label(new Rect(100f, 90f, 1000f, 20f), "Press T for a slider Menu");
        GUI.Label(new Rect(100f, 110f, 1000f, 20f), "Press M to return the default slider settings");
    }

    private void Start()
    {
        this.holoColorSlider = new Color(0.19f, 0.48f, 0.725f, 1f);
        this.rimColorSlider = new Color(0.075f, 0.22f, 0.34f, 1f);
        this.rimPowerSlider = 1f;
        this.sliceAmounSlider = 0f;
        this.reflectSlider = 0f;
        //this.tempRot = this.light.get_transform().get_rotation();
		this.tempRot=this.light.transform.rotation;
    }

    private void Update()
    {
        //this.light.get_transform().set_rotation(this.tempRot);
		this.light.transform.rotation=this.tempRot;
    }
}

