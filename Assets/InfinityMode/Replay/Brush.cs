using System.Collections.Generic;
using UnityEngine;

public class Brush : MonoBehaviour
{
    // References
    [SerializeField]
    private GameObject m_pointPrefab;
    private List<GameObject> m_points;
    private Transform m_drawingTransform;

    // Attributes
    private Vector3 m_lastPosition;
    private Vector3 m_nullPosition = new Vector3(-1000, -1000, -1000);
    public bool drawing { get; set; }

    public void Initialize(Transform drawingTransform)
    {
        m_points = new List<GameObject>();
        drawing = false;
        m_lastPosition = m_nullPosition;
        m_drawingTransform = drawingTransform;
    }

    private void Update()
    {
        if (drawing)
        {
            DrawingUpdate();
        }
    }
    public void DrawingUpdate()
    {
        if (m_drawingTransform != null && m_lastPosition != transform.position)
        {
            GameObject point = Instantiate(m_pointPrefab, m_drawingTransform);
            Vector3 position = transform.position;
            if (m_lastPosition == m_nullPosition)
            {
                point.transform.position = position;
                point.transform.rotation = Quaternion.identity;
                point.transform.localScale = new Vector3(1 / point.transform.lossyScale.x, 1 / point.transform.lossyScale.y, 1 / point.transform.lossyScale.z);
            }
            else
            {
                float dst = Mathf.Sqrt(Mathf.Pow(position.x - m_lastPosition.x, 2) + Mathf.Pow(position.z - m_lastPosition.z, 2));

                point.transform.position = new Vector3((position.x + m_lastPosition.x) / 2f, (position.y + m_lastPosition.y) / 2f, (position.z + m_lastPosition.z) / 2f);
                point.transform.rotation = Quaternion.Euler(0f, (Mathf.Rad2Deg * Mathf.Acos((position.x - m_lastPosition.x) / dst)), 0f); // Produit scalaire
                point.transform.localScale = new Vector3(dst / point.transform.lossyScale.x + 1, 1 / point.transform.lossyScale.y, 1 / point.transform.lossyScale.z);
            }

            m_points.Add(point);
            m_lastPosition = position;
        }
    }
    public void SimpleUpdate()
    {
        if (m_drawingTransform != null && m_lastPosition != transform.position)
        {
            GameObject point = Instantiate(m_pointPrefab, m_drawingTransform);
            point.transform.position = transform.position;
            point.transform.rotation = Quaternion.identity;
            point.transform.localScale = new Vector3(1 / point.transform.lossyScale.x, 1 / point.transform.lossyScale.y, 1 / point.transform.lossyScale.z);

            m_points.Add(point);
            m_lastPosition = transform.position;
        }
    }

    public void Erase()
    {
        foreach (GameObject point in m_points)
        {
            Destroy(point);
        }
        m_points.Clear();
        m_lastPosition = m_nullPosition;
    }

    public void Replace(Vector3 position)
    {
        transform.position = position;
        GameObject point = Instantiate(m_pointPrefab, m_drawingTransform);

        point.transform.position = position;
        point.transform.rotation = Quaternion.identity;
        point.transform.localScale = new Vector3(1 / point.transform.lossyScale.x, 1 / point.transform.lossyScale.y, 1 / point.transform.lossyScale.z);

        m_points.Add(point);
        m_lastPosition = position;
    }
}
