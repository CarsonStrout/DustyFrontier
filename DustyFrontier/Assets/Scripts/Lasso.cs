using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasso : MonoBehaviour
{
    public Camera cam;
    public LineRenderer lineRenderer;

    public LayerMask lassoMask; // What we can lasso / grapple
    public float speed = 2;
    public float lassoLength = 10;

    public int maxPoints = 3;

    public Rigidbody2D rb;
    private List<Vector2> points = new List<Vector2>();

    private void Start()
    {
        lineRenderer.positionCount = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)transform.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, lassoLength, lassoMask);
            if (hit.collider != null)
            {
                Vector2 hitPoint = hit.point;
                points.Add(hitPoint);

                if (points.Count > maxPoints)
                {
                    points.RemoveAt(0);
                }
            }
        }

        if (points.Count > 0)
        {
            Vector2 moveTo = centroid(points.ToArray());

            rb.MovePosition(Vector2.MoveTowards(transform.position, moveTo, Time.deltaTime * speed));

            lineRenderer.positionCount = 0;
            lineRenderer.positionCount = points.Count * 2;
            for (int i = 0, j = 0; i < points.Count * 2; i += 2, j++)
            {
                lineRenderer.SetPosition(i, transform.position);
                lineRenderer.SetPosition(i + 1, points[j]);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Detatch();
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity += Vector2.up * 12f;
        }
    }

    public void Detatch()
    {
        lineRenderer.positionCount = 0;
        points.Clear();
    }

    Vector2 centroid(Vector2[] points)
    {
        Vector2 center = Vector2.zero;
        foreach (Vector2 point in points)
        {
            center += point;
        }
        center /= points.Length;
        return center;
    }
}
