using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

// -----------------------------------------------------------
// 複数の組の画像マーカーとオブジェクト(Prefab)を対応させる。
// ARマーカーのトラッキング状態に合わせて
// オブジェクトの位置、回転角度、アクティブを設定する。
// -----------------------------------------------------------
public class Storage_MultiARMarker : MonoBehaviour
{

    // マーカー用オブジェクト(Prefab)  ※ UnityEditor から登録しておく
    [SerializeField] private GameObject[] arPrefabs;

    // ARSessionOriginにアタッチされているARTrackedImageManager  ※ UnityEditor から登録しておく
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    // マーカーと対応したオブジェクト(Prefab)と文字列のペアのDictionary
    private readonly Dictionary<string, GameObject> nameWithPrefabDictionary = new Dictionary<string, GameObject>();

    // ボタン(サムネイル)が押下されているARマーカーの名前
    private string selectedARMarkerName = "";
    // ボタンが押されているとき対応するARマーカーに重ねて表示するオブジェクト
    [SerializeField] private GameObject selectedPrefab = null;
    private GameObject selectedObject = null;
    // Debug
    [SerializeField] private Text debugText;


    private void Start()
    {
        // ARTrackedImageManagerのイベント「trackedImagesChanged」を処理するメソッドを登録する。
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;

        // ARマーカーの名前とオブジェクト(Prefab)をセットで登録する
        for (var i = 0; i < arPrefabs.Length; i++)
        {
            var arPrefab = Instantiate(arPrefabs[i]);
            nameWithPrefabDictionary.Add(trackedImageManager.referenceLibrary[i].name, arPrefab);

            // ARオブジェクトは非アクティブを初期状態としておく。
            arPrefab.SetActive(false);
        }

        if(selectedPrefab != null)
        {
            selectedObject = Instantiate(selectedPrefab);
            selectedObject.SetActive(false);
        }

        // debug
        debugText.text = "debug.";
        string debug_str = "";
        
        //Debug.Log("/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_");
        for (var i = 0; i < trackedImageManager.referenceLibrary.count; i++)
        {
            GameObject obj = nameWithPrefabDictionary[trackedImageManager.referenceLibrary[i].name];
            //Debug.Log(trackedImageManager.referenceLibrary[i].name);
            //Debug.Log(obj.name);
            //Debug.Log("------------------------");

            debug_str = debug_str + trackedImageManager.referenceLibrary[i].name + " : " + obj.name;
            debug_str = debug_str + "\n";
        }
        debugText.text = debug_str;
    }

    // イベント登録の削除
    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }


    // 認識した画像マーカーに対応したARオブジェクトを表示する
    //
    // AddとUpdateでイベントが発生したマーカーに対応するARオブジェクトの
    // 位置、回転角度、アクティブ/非を設定する。
    // 
    // ARTrackedImage trackedImage          : 認識した画像マーカー
    // 
    // [ARTrackedImageのプロパティ]
    //      XRReferenceImage referenceImage : 
    //      [XRReferenceImage のプロパティ]
    //          name    : ReferenceImageLibraryに設定したName。
    //          guid    : ARマーカーのグローバル一意識別子
    //          texture : 
    //          ...etc..
    private void ActivateARObject(ARTrackedImage trackedImage)
    {
        // 認識した画像マーカーの名前から対応するオブジェクトを取得する
        GameObject arObject = nameWithPrefabDictionary[trackedImage.referenceImage.name];
    
        // 位置合わせ
        arObject.transform.SetPositionAndRotation(trackedImage.transform.position, trackedImage.transform.rotation);
        arObject.transform.SetParent(trackedImage.transform);

        // トラッキングの状態に応じてARオブジェクトの表示を切り替える。
        // ARマーカーがトラッキング中(TrackingState.Tracking)のケース true / それ以外 false に設定する。
        arObject.SetActive(trackedImage.trackingState == TrackingState.Tracking);
    }


    // TrackedImagesChanged イベント処理
    // 
    // [ARTrackedImagesChangedEventArgs の プロパティ]
    // added (ARTrackedImageのList)
    // updated (ARTrackedImageのList)
    // removed (ARTrackedImageのList)
    // 
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            ActivateARObject(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            ActivateARObject(trackedImage);
        }
    }

    private void Update()
    {
        if (selectedARMarkerName.Equals("") != true)
        {
            ShowSelectedObject();
        }
        else
        {
            selectedObject.SetActive(false);
        }
    }


    // ボタンにより選択されているオブジェクトを表示
    private void ShowSelectedObject()
    {

        // ----- debug -----
        /*
        Debug.Log("/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_");
        for (var i = 0; i < trackedImageManager.referenceLibrary.count; i++)
        {
            GameObject obj = nameWithPrefabDictionary[trackedImageManager.referenceLibrary[i].name];
            Debug.Log(trackedImageManager.referenceLibrary[i].name);
            Debug.Log(obj.name);

            Debug.Log("------------------------");
        }
        */
        // -----------------


        // ボタンが押下されているARオブジェクトを取得する。
        GameObject arObject = nameWithPrefabDictionary[selectedARMarkerName];

        // 押下されているボタンのARオブジェクトがアクティブの場合
        if (arObject != null && arObject.activeSelf == true)
        {
            // 選択されていることを示すオブジェクトを表示する
            if (selectedObject != null)
            {
                // ----- debug ------
                debugText.text = arObject.name + " : " + selectedARMarkerName;
                // ------------------

                selectedObject.transform.SetPositionAndRotation(arObject.transform.position, arObject.transform.rotation);
                selectedObject.SetActive(true);
            }
        }
        else if(selectedObject != null)
        {
            // ボタンに対応するオブジェクトが無いか、非アクティブの場合
            // 選択中を示すオブジェクトを非アクティブにする
            selectedObject.SetActive(false);
        }
    }


    // ボタンイベントハンドラ

    public void OnButtonDown(string marker_name)
    {
        selectedARMarkerName = marker_name;
    }

    public void OnButtonUp()
    {
        selectedARMarkerName = "";
    }
}