using System;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Color _defaultHoloColor = new Color(0.19f, 0.48f, 0.725f, 1f);
    public float _defaultReflectPower;
    public Color _defaultRimColor = new Color(0.075f, 0.22f, 0.34f, 1f);
    public float _defaultRimPower = 1f;
    public float _defaultSliceAmount;
    public AnimationManager animationManager;
    public bool customInput;
    public bool startDieing;

    private void Start()
    {
		base.GetComponent<Renderer>().material.shader.name="Bumped Diffuse";
        //base.get_renderer().get_material().set_shader(Shader.Find("Bumped Diffuse"));
        this._defaultHoloColor = new Color(0.19f, 0.48f, 0.725f, 1f);
        this._defaultRimColor = new Color(0.075f, 0.22f, 0.34f, 1f);
        this._defaultRimPower = 1f;
        this._defaultSliceAmount = 0f;
        this._defaultReflectPower = 0f;
    }

    private void Update()
    {
		
        base.GetComponent<Renderer>().material.SetColor("_HoloColor", this._defaultHoloColor);
        //base.get_renderer().get_material().SetColor("_RimColor", this._defaultRimColor);
		 base.GetComponent<Renderer>().material.SetFloat("_RimPower", this._defaultRimPower);
       // base.get_renderer().get_material().SetFloat("_RimPower", this._defaultRimPower);
		base.GetComponent<Renderer>().material.SetFloat("_SliceAmount", this._defaultSliceAmount);
        //base.get_renderer().get_material().SetFloat("_SliceAmount", this._defaultSliceAmount);
		base.GetComponent<Renderer>().material.SetFloat("_ReflectPower", this._defaultReflectPower);
        //base.get_renderer().get_material().SetFloat("_ReflectPower", this._defaultReflectPower);
        if (!this.customInput)
        {
            if (this.startDieing && (this._defaultSliceAmount <= 1f))
            {
				
				 this._defaultSliceAmount += 0.1f * Time.deltaTime;
                //this._defaultSliceAmount += 0.1f * Time.get_deltaTime();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (this.startDieing)
                {
                    this.startDieing = false;
                }
                else
                {
					base.GetComponent<Renderer>().material.shader.name="Custom/DissolveElectricity";
                    //base.get_renderer().get_material().set_shader(Shader.Find("Custom/DissolveElectricity"));
                    this.startDieing = true;
                }
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                this.startDieing = false;
				base.GetComponent<Renderer>().material.shader.name="Bumped Diffuse";
                //base.get_renderer().get_material().set_shader(Shader.Find("Bumped Diffuse"));
                this._defaultSliceAmount = 0f;
            }
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (this.customInput)
            {
                this.customInput = false;
				base.GetComponent<Renderer>().material.shader.name="Bumped Diffuse";
                //base.get_renderer().get_material().set_shader(Shader.Find("Bumped Diffuse"));
            }
            else
            {
                this.customInput = true;
				base.GetComponent<Renderer>().material.shader.name="Custom/DissolveElectricity";
                //base.get_renderer().get_material().set_shader(Shader.Find("Custom/DissolveElectricity"));
            }
        }
        if (this.customInput)
        {
			this.animationManager=GameObject.FindObjectOfType(typeof(AnimationManager))as AnimationManager;
            //this.animationManager = Object.FindObjectOfType(typeof(AnimationManager)) as AnimationManager;
            this._defaultHoloColor = this.animationManager.holoColorSlider;
            this._defaultRimColor = this.animationManager.rimColorSlider;
            this._defaultRimPower = this.animationManager.rimPowerSlider;
            this._defaultSliceAmount = this.animationManager.sliceAmounSlider;
            this._defaultReflectPower = this.animationManager.reflectSlider;
            if (Input.GetKeyDown(KeyCode.D))
            {
                this.animationManager.holoColorSlider = new Color(0.19f, 0.48f, 0.725f, 1f);
                this.animationManager.rimColorSlider = new Color(0.075f, 0.22f, 0.34f, 1f);
                this.animationManager.rimPowerSlider = 1f;
                this.animationManager.sliceAmounSlider = 0f;
                this.animationManager.reflectSlider = 0f;
            }
        }
    }
}

