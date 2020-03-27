using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.Presets;
#endif

namespace hatuxes.InputManagerPreset
{
    public class Controller_Cube : MonoBehaviour
    {
        string _controllerName;
        [SerializeField] Preset _XBoxPreset;
        [SerializeField] Preset _PS4Preset;

        Vector4 _inputStickAxisValue;

        Vector2 _inputTriggerValue;

        bool _inputRStickPressed;

        bool _inputLTriggerPressed;
        bool _inputRTriggerPressed;

        [SerializeField, ContextMenuItem("ResetParameter", "Reset")]
        float _moveSpeed;
        [SerializeField, ContextMenuItem("ResetParameter", "Reset")]
        float _rotateSpeed;

        Material _material;
        int _propertyID;

        void Awake()
        {
            //コントローラーの名前を取得
            foreach (var name in Input.GetJoystickNames())
                if (name.Length > 1)
                    _controllerName = name;

            //コントローラーの接続が無ければエラーとして終了する
            if (_controllerName == null)
            {
                Debug.LogError("Controller is not connected.");

                #if UNITY_EDITOR
                    EditorApplication.isPlaying = false;
                #endif

                return;
            }

            Debug.Log(_controllerName);

            //コントローラー別にInputManagerにPresetを反映
            if (_controllerName == "Controller (XBOX 360 For Windows)")
            {
                Object manager = AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/InputManager.asset");
                _XBoxPreset.ApplyTo(manager);
            }
            else if (_controllerName == "Wireless Controller")
            {
                Object manager = AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/InputManager.asset");
                _PS4Preset.ApplyTo(manager);
            }
        }

        void Start()
        {
            //初期化
            _inputStickAxisValue = Vector4.zero;
            _inputTriggerValue = Vector2.zero;

            //マテリアルとIDを取得
            _material = this.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
            _propertyID = Shader.PropertyToID("_Color");
        }

        void Update()
        {
            //左スティックの取得
            _inputStickAxisValue.x = Input.GetAxis("L_Horizontal");
            _inputStickAxisValue.y = Input.GetAxis("L_Vertical");
            //右スティックの取得
            _inputStickAxisValue.z = Input.GetAxis("R_Horizontal");
            _inputStickAxisValue.w = Input.GetAxis("R_Vertical");

            //スティック押し込みの取得
            _inputRStickPressed = Input.GetButtonDown("ViewReset");

            //トリガー手前のボタンを取得
            _inputLTriggerPressed = Input.GetButtonDown("L1_Button");
            _inputRTriggerPressed = Input.GetButtonDown("R1_Button");

            //トリガーの取得
            _inputTriggerValue.x = Input.GetAxis("L2_Trigger");
            _inputTriggerValue.y = Input.GetAxis("R2_Trigger");

            //色変更
            if (_inputLTriggerPressed) _material.SetColor(_propertyID, Color.white);
            if (_inputRTriggerPressed) _material.SetColor(_propertyID, Color.red);

            //リセット
            if (_inputRStickPressed)
            {
                this.transform.position = Vector3.zero;
                this.transform.rotation = Quaternion.Euler(0, 0, 0);
                _material.SetColor(_propertyID, Color.white);
            }
        }

        void FixedUpdate()
        {
            //平面移動処理
            transform.position += Vector3.right * _inputStickAxisValue.x * _moveSpeed;
            transform.position += Vector3.up * _inputStickAxisValue.y * _moveSpeed;

            //奥行き移動処理
            this.transform.position += -Vector3.forward * _inputTriggerValue.x * _moveSpeed;
            this.transform.position += Vector3.forward * _inputTriggerValue.y * _moveSpeed;

            //回転処理
            Quaternion quaternionYaw = Quaternion.AngleAxis(-_inputStickAxisValue.z * _rotateSpeed, Vector3.up);
            Quaternion quaternionPitch = Quaternion.AngleAxis(-_inputStickAxisValue.w * _rotateSpeed, Vector3.right);
            Quaternion quaternion = quaternionPitch * quaternionYaw;
            this.transform.rotation = quaternion * this.transform.rotation;
        }

        void Reset()
        {
            _moveSpeed = 0.15f;
            _rotateSpeed = 2;
        }
    }
}