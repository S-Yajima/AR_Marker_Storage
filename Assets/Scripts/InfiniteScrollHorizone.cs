using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ---------------------------------------------------------
// 
// ---------------------------------------------------------
public class InfiniteScrollHorizone : MonoBehaviour
{
    // 自身(ScrollView)のContent
    [SerializeField] private GameObject m_content = null;
    private RectTransform m_content_recttransform;

    // Cell 1 つ分の 幅と隙間の大きさ
    private float m_cell_size = 0;

    // 最後にセルを移動させたContentのアンカーの位置
    private float m_last_content_anchored_x = 0;

    // CellのLinkList
    private LinkedList<RectTransform> m_cell_list = new LinkedList<RectTransform>();


    // Start is called before the first frame update
    void Start()
    {

        // Content の GridLayoutGroup を取得する。
        GridLayoutGroup content_grid = m_content.transform.GetComponent<GridLayoutGroup>();
        // Cell 1つ分の横の大きさを算出しメンバ変数に取得する。
        m_cell_size = content_grid.cellSize.x + content_grid.spacing.x;

        // ContentのRectTransformを取得する。
        m_content_recttransform = m_content.transform.GetComponent<RectTransform>();
        // Contentのアンカー位置を取得する。
        m_last_content_anchored_x = m_content_recttransform.anchoredPosition.x;

        // Contentの子オブジェクトのRectTransformを取得する。
        for (int i = 0; i < m_content.transform.childCount; i++)
        {
            m_cell_list.AddLast(m_content.transform.GetChild(i).GetComponent<RectTransform>());
        }


        //StartCoroutine("test");
    }

    // Update is called once per frame
    void Update()
    {
        // サムネイルがどれだけ動かされたかを取得する。
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
                // 左にスライドされたケース
                // サムネイルの左側のアイコン(ボタン、イメージ)をListの最後に付け替える。
                m_cell_list.RemoveFirst();
                m_cell_list.AddLast(first);

                // 左端のサムネイルのアンカー位置を右端に付け替える。
                first.anchoredPosition = new Vector2(
                    last.anchoredPosition.x + m_cell_size, 
                    first.anchoredPosition.y);
            }
            else
            {
                // 右にスライドされたケース
                // サムネイルの右側のアイコン(ボタン、イメージ)をListの最初に付け替える。
                m_cell_list.RemoveLast();
                m_cell_list.AddFirst(last);

                // 右端のサムネイルのアンカー位置を左端に付け替える。
                last.anchoredPosition = new Vector2(
                    first.anchoredPosition.x - m_cell_size, 
                    first.anchoredPosition.y);
            }

            // 最後にセルを移動させたContentのアンカーの位置を更新する。
            m_last_content_anchored_x = m_content_recttransform.anchoredPosition.x;
        }

    }


}
