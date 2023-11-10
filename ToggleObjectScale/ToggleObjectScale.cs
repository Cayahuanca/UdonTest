using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Praecipua.Udon
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ToggleObjectScale : UdonSharpBehaviour
    {
        [Header("変更情報を同期をするか")]
        [SerializeField] private bool isGlobal = false;

        [Space(10)]

        [Header("Scale を変更するオブジェクト")]
        [SerializeField] private GameObject[] targetObjectScale;

        [Space(10)]

        [Header("Scale の変更状態を表示するオブジェクト")]
        [SerializeField] private GameObject[] targetObjectActive;
        [Header("一つは表示状態に、(もう一つ入れる場合は、もう一つは非表示状態に、)しておくことを推奨")]

        [UdonSynced] bool[] isScaled;
        [UdonSynced] bool[] isActive;
        [UdonSynced] Vector3[] defaultScale;

        void Start()
        {
            // 配列を初期化
            isScaled = new bool[targetObjectScale.Length];
            isActive = new bool[targetObjectActive.Length];

            defaultScale = new Vector3[targetObjectScale.Length];
            for (int i = 0; i < targetObjectScale.Length; i = i + 1)
            {
                if (targetObjectScale[i] != null)     //配列内の Null チェック
                {
                    defaultScale[i] = targetObjectScale[i].transform.localScale;
                }
            }

            if (isGlobal)   //Global の場合
            {
                for (int i = 0;i < targetObjectScale.Length; i = i + 1)
                {
                    if (targetObjectScale[i] != null)     //配列内の Null チェック
                    {
                        isScaled[i] = false;    //同期変数の Bool を初期化
                    }
                }

                for (int i = 0; i < targetObjectActive.Length; i = i + 1)
                {
                    if (targetObjectActive[i] != null)     //配列内の Null チェック
                    {
                        isActive[i] = targetObjectActive[i].activeSelf;    //同期変数の Bool を初期化
                    }
                }
            }
        }

        public override void Interact()
        {
            if (isGlobal)  //Global の場合
            {
                if (!Networking.IsOwner(gameObject))
                {
                    Networking.SetOwner(Networking.LocalPlayer, gameObject);
                }

                for (int i = 0; i < targetObjectScale.Length; i = i + 1)
                {
                    if (targetObjectScale[i] != null)     //配列内の Null チェック
                    {
                        isScaled[i] = !isScaled[i];       //同期変数の Bool を反転
                    }
                }

                for (int i = 0; i < targetObjectActive.Length; i = i + 1)
                {
                    if (targetObjectActive[i] != null)     //配列内の Null チェック
                    {
                        isActive[i] = !isActive[i];       //同期変数の Bool を反転
                    }
                }
                SetObjectScaleGlobal();   //オブジェクトの Scale を切り替え
                RequestSerialization();     //同期をリクエスト
            }
            else  //Local の場合
            {
                SetObjectScaleLocal();    //オブジェクトの Scale を切り替え
            }
        }

        public void SetObjectScaleGlobal()
        {
            for (int i = 0; i < targetObjectScale.Length; i = i + 1)
            {
                if (targetObjectScale[i] != null)     //配列内の Null チェック
                {
                    if (isScaled[i])    // Scale = true の場合、元の Scaleに戻す
                    {
                        targetObjectScale[i].transform.localScale = defaultScale[i];
                    }
                    else   // Scale != false の場合、Scale = 0 にする
                    {
                        targetObjectScale[i].transform.localScale = new Vector3(0, 0, 0);
                    }
                }
            }

            for (int i = 0; i < targetObjectActive.Length; i = i + 1)
            {
                if (targetObjectActive[i] != null)     //配列内の Null チェック
                {
                    targetObjectActive[i].SetActive(!isActive[i]);    //オブジェクトのアクティブを反映
                }
            }
        }

        public override void OnDeserialization()
        {
            if (isGlobal)
            {
                if (!Networking.IsOwner(gameObject))
                {
                    SetObjectScaleGlobal();      //受信した Scale と isScaled の状態を反映
                }
            }
        }

        public void SetObjectScaleLocal()     //オブジェクトのアクティブを切り替え
        {
            for (int i = 0; i < targetObjectScale.Length; i = i + 1)
            {
                if (targetObjectScale[i] != null)     //配列内の Null チェック
                {
                    if(targetObjectScale[i].transform.localScale == defaultScale[i])   // defaultScale と Scale が等しい場合、Scale = 0 にする
                    {
                        targetObjectScale[i].transform.localScale = new Vector3(0, 0, 0);
                    }
                    else if(targetObjectScale[i].transform.localScale == new Vector3(0, 0, 0))   // Scale = 0 の場合、元の Scale に戻す
                    {
                        targetObjectScale[i].transform.localScale = defaultScale[i];
                    }
                }
            }

            for (int i = 0; i < targetObjectActive.Length; i = i + 1)
            {
                if (targetObjectActive[i] != null)     //配列内の Null チェック
                {
                    targetObjectActive[i].SetActive(!targetObjectActive[i].activeSelf);    //オブジェクトのアクティブを反転
                }
            }
        }
    }
}