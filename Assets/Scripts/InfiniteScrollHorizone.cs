using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ---------------------------------------------------------
// 
// ---------------------------------------------------------
public class InfiniteScrollHorizone : MonoBehaviour
{
    // ���g(ScrollView)��Content
    [SerializeField] private GameObject m_content = null;
    private RectTransform m_content_recttransform;

    // Cell 1 ���� ���ƌ��Ԃ̑傫��
    private float m_cell_size = 0;

    // �Ō�ɃZ�����ړ�������Content�̃A���J�[�̈ʒu
    private float m_last_content_anchored_x = 0;

    // Cell��LinkList
    private LinkedList<RectTransform> m_cell_list = new LinkedList<RectTransform>();


    // Start is called before the first frame update
    void Start()
    {

        // Content �� GridLayoutGroup ���擾����B
        GridLayoutGroup content_grid = m_content.transform.GetComponent<GridLayoutGroup>();
        // Cell 1���̉��̑傫�����Z�o�������o�ϐ��Ɏ擾����B
        m_cell_size = content_grid.cellSize.x + content_grid.spacing.x;

        // Content��RectTransform���擾����B
        m_content_recttransform = m_content.transform.GetComponent<RectTransform>();
        // Content�̃A���J�[�ʒu���擾����B
        m_last_content_anchored_x = m_content_recttransform.anchoredPosition.x;

        // Content�̎q�I�u�W�F�N�g��RectTransform���擾����B
        for (int i = 0; i < m_content.transform.childCount; i++)
        {
            m_cell_list.AddLast(m_content.transform.GetChild(i).GetComponent<RectTransform>());
        }


        //StartCoroutine("test");
    }

    // Update is called once per frame
    void Update()
    {
        // �T���l�C�����ǂꂾ���������ꂽ�����擾����B
        float diff = Mathf.Clamp(
            m_content_recttransform.anchoredPosition.x - m_last_content_anchored_x,
            -m_cell_size, m_cell_size);

        //Debug.Log(diff);
        //Debug.Log(m_cell_size);

        if (Mathf.Abs(diff) >= m_cell_size)
        {
            var first = m_cell_list.First.Value;
            var last = m_cell_list.Last.Value;

            if (diff < 0)
            {
                // ���ɃX���C�h���ꂽ�P�[�X
                // �T���l�C���̍����̃A�C�R��(�{�^���A�C���[�W)��List�̍Ō�ɕt���ւ���B
                m_cell_list.RemoveFirst();
                m_cell_list.AddLast(first);

                // ���[�̃T���l�C���̃A���J�[�ʒu���E�[�ɕt���ւ���B
                first.anchoredPosition = new Vector2(
                    last.anchoredPosition.x + m_cell_size, 
                    first.anchoredPosition.y);
            }
            else
            {
                // �E�ɃX���C�h���ꂽ�P�[�X
                // �T���l�C���̉E���̃A�C�R��(�{�^���A�C���[�W)��List�̍ŏ��ɕt���ւ���B
                m_cell_list.RemoveLast();
                m_cell_list.AddFirst(last);

                // �E�[�̃T���l�C���̃A���J�[�ʒu�����[�ɕt���ւ���B
                last.anchoredPosition = new Vector2(
                    first.anchoredPosition.x - m_cell_size, 
                    first.anchoredPosition.y);
            }

            // �Ō�ɃZ�����ړ�������Content�̃A���J�[�̈ʒu���X�V����B
            m_last_content_anchored_x = m_content_recttransform.anchoredPosition.x;
        }

    }


}
