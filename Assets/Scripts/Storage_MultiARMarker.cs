using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

// -----------------------------------------------------------
// �����̑g�̉摜�}�[�J�[�ƃI�u�W�F�N�g(Prefab)��Ή�������B
// AR�}�[�J�[�̃g���b�L���O��Ԃɍ��킹��
// �I�u�W�F�N�g�̈ʒu�A��]�p�x�A�A�N�e�B�u��ݒ肷��B
// -----------------------------------------------------------
public class Storage_MultiARMarker : MonoBehaviour
{

    // �}�[�J�[�p�I�u�W�F�N�g(Prefab)  �� UnityEditor ����o�^���Ă���
    [SerializeField] private GameObject[] arPrefabs;

    // ARSessionOrigin�ɃA�^�b�`����Ă���ARTrackedImageManager  �� UnityEditor ����o�^���Ă���
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    // �}�[�J�[�ƑΉ������I�u�W�F�N�g(Prefab)�ƕ�����̃y�A��Dictionary
    private readonly Dictionary<string, GameObject> nameWithPrefabDictionary = new Dictionary<string, GameObject>();

    // �{�^��(�T���l�C��)����������Ă���AR�}�[�J�[�̖��O
    private string selectedARMarkerName = "";
    // �{�^����������Ă���Ƃ��Ή�����AR�}�[�J�[�ɏd�˂ĕ\������I�u�W�F�N�g
    [SerializeField] private GameObject selectedPrefab = null;
    private GameObject selectedObject = null;
    // Debug
    [SerializeField] private Text debugText;


    private void Start()
    {
        // ARTrackedImageManager�̃C�x���g�utrackedImagesChanged�v���������郁�\�b�h��o�^����B
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;

        // AR�}�[�J�[�̖��O�ƃI�u�W�F�N�g(Prefab)���Z�b�g�œo�^����
        for (var i = 0; i < arPrefabs.Length; i++)
        {
            var arPrefab = Instantiate(arPrefabs[i]);
            nameWithPrefabDictionary.Add(trackedImageManager.referenceLibrary[i].name, arPrefab);

            // AR�I�u�W�F�N�g�͔�A�N�e�B�u��������ԂƂ��Ă����B
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

    // �C�x���g�o�^�̍폜
    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }


    // �F�������摜�}�[�J�[�ɑΉ�����AR�I�u�W�F�N�g��\������
    //
    // Add��Update�ŃC�x���g�����������}�[�J�[�ɑΉ�����AR�I�u�W�F�N�g��
    // �ʒu�A��]�p�x�A�A�N�e�B�u/���ݒ肷��B
    // 
    // ARTrackedImage trackedImage          : �F�������摜�}�[�J�[
    // 
    // [ARTrackedImage�̃v���p�e�B]
    //      XRReferenceImage referenceImage : 
    //      [XRReferenceImage �̃v���p�e�B]
    //          name    : ReferenceImageLibrary�ɐݒ肵��Name�B
    //          guid    : AR�}�[�J�[�̃O���[�o����ӎ��ʎq
    //          texture : 
    //          ...etc..
    private void ActivateARObject(ARTrackedImage trackedImage)
    {
        // �F�������摜�}�[�J�[�̖��O����Ή�����I�u�W�F�N�g���擾����
        GameObject arObject = nameWithPrefabDictionary[trackedImage.referenceImage.name];
    
        // �ʒu���킹
        arObject.transform.SetPositionAndRotation(trackedImage.transform.position, trackedImage.transform.rotation);
        arObject.transform.SetParent(trackedImage.transform);

        // �g���b�L���O�̏�Ԃɉ�����AR�I�u�W�F�N�g�̕\����؂�ւ���B
        // AR�}�[�J�[���g���b�L���O��(TrackingState.Tracking)�̃P�[�X true / ����ȊO false �ɐݒ肷��B
        arObject.SetActive(trackedImage.trackingState == TrackingState.Tracking);
    }


    // TrackedImagesChanged �C�x���g����
    // 
    // [ARTrackedImagesChangedEventArgs �� �v���p�e�B]
    // added (ARTrackedImage��List)
    // updated (ARTrackedImage��List)
    // removed (ARTrackedImage��List)
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


    // �{�^���ɂ��I������Ă���I�u�W�F�N�g��\��
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


        // �{�^������������Ă���AR�I�u�W�F�N�g���擾����B
        GameObject arObject = nameWithPrefabDictionary[selectedARMarkerName];

        // ��������Ă���{�^����AR�I�u�W�F�N�g���A�N�e�B�u�̏ꍇ
        if (arObject != null && arObject.activeSelf == true)
        {
            // �I������Ă��邱�Ƃ������I�u�W�F�N�g��\������
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
            // �{�^���ɑΉ�����I�u�W�F�N�g���������A��A�N�e�B�u�̏ꍇ
            // �I�𒆂������I�u�W�F�N�g���A�N�e�B�u�ɂ���
            selectedObject.SetActive(false);
        }
    }


    // �{�^���C�x���g�n���h��

    public void OnButtonDown(string marker_name)
    {
        selectedARMarkerName = marker_name;
    }

    public void OnButtonUp()
    {
        selectedARMarkerName = "";
    }
}