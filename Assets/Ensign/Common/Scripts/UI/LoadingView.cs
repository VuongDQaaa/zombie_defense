using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ensign;
using Ensign.Unity.Mvc;
using UnityEngine.UI;

namespace Ensign.Unity
{
    public class LoadingView : View
    {
        [SerializeField]
        private Texture loadingTexture;
        [SerializeField]
        private GameObject blockInput;
        [SerializeField]
        private Text lblText;
        [SerializeField]
        private float size = 120f;
        [SerializeField]
        private float rotSpeed = 300f;
        [SerializeField]
        private float timeUpdateText = 0.5f;

        public string text = "Loading";

#if UNITY_EDITOR
        [SerializeField]
        private bool isLoading;
#endif
        bool _isLoading;
        private float rotAngle = 0f;

        public void Configure()
        {
        }
        void Update()
        {
#if UNITY_EDITOR
            SetLoading(isLoading);
#endif

            if (_isLoading)
            {
                rotAngle += rotSpeed * Time.deltaTime;

                if(lblText != null && !string.IsNullOrWhiteSpace(text))
                {
                    countdownUpdateText -= Time.deltaTime;
                    if (countdownUpdateText <= 0)
                    {
                        if (txtExtenstion == ".")
                            txtExtenstion = "..";
                        else if (txtExtenstion == "..")
                            txtExtenstion = "...";
                        else
                            txtExtenstion = ".";
                        
                        countdownUpdateText = timeUpdateText;
                    }
                    lblText.text = text + txtExtenstion;
                }
            }
        }
        float countdownUpdateText = 0;
        string txtExtenstion=string.Empty;

        private void OnGUI()
        {
            if (_isLoading)
            {
                Vector2 pivot = new Vector2(Screen.width / 2, Screen.height / 2);
                GUIUtility.RotateAroundPivot(rotAngle % 360, pivot);
                GUI.DrawTexture(new Rect((Screen.width - size) / 2, (Screen.height - size) / 2, size, size), loadingTexture);
            }
        }

        private LoadingView SetLoading(bool value)
        {
            if (_isLoading != value)
            {
                lblText.text = "Loading";
#if UNITY_EDITOR
                isLoading = value;
#endif
                SetBlockInput(value);
                _isLoading = value;
            }
            return this;
        }

        private LoadingView SetBlockInput(bool value)
        {
            if (blockInput != null)
                blockInput.SetActive(value);
            return this;
        }

        public static LoadingController GetInstance()
        {
            if(ModuleHandler.Presenter.TryGetController<LoadingController>(out LoadingController controller))
            {
                return controller;
            }
            return  ModuleHandler.Presenter.Present<LoadingController>().Controller;
        }

        public override void OnBindData(CommandEventArgs command)
        {
            if(command.TryGetCmdArgs(out int intValue, out string strValue))
			{
				this.text = strValue;
			}
            else if(command.TryGetCmdArgs(out int commandValue, out bool boolValue))
            {
                switch(commandValue)
                {
                    case 1:
                        SetLoading(boolValue);
                        break;
                    default:
                        SetBlockInput(boolValue);
                        break;
                }
            }
        }
    }

    public class LoadingController : ViewController<LoadingView>
    {
        public LoadingController(IPresentContext context)
			: base(context)
		{
            View.Configure();
        }

        public LoadingController SetText(string text)
        {
            this.BindCommand(0, text);
            return this;
        }

        public LoadingController SetLoading(bool value)
        {
            this.BindCommand(1, value);
            return this;
        }

        public LoadingController SetBlockInput(bool value)
        {
            this.BindCommand(0, value);
            return this;
        }

        public void Show()
        {
            View.Enabled = true;
        }

        public void Hide()
        {
            View.Enabled = false;
        }

    }
}
