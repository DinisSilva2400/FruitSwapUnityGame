using UnityEngine;

public class Fruit : MonoBehaviour
{
    public int x;
    public int y;
    public int type;

    public Board board;

    private Vector2 startTouch;
    private bool isDragging = false;

    void Update()
    {
        // --- Mouse input ---
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(worldPos, transform.position) < 0.5f)
            {
                startTouch = worldPos;
                isDragging = true;
            }
        }

        if (isDragging && Input.GetMouseButtonUp(0))
        {
            Vector2 endTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            HandleSwipe(endTouch);
        }

        // --- Touch input ---
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(t.position);

            if (t.phase == TouchPhase.Began)
            {
                if (Vector2.Distance(worldPos, transform.position) < 0.5f)
                {
                    startTouch = worldPos;
                    isDragging = true;
                }
            }

            if (isDragging && (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled))
            {
                HandleSwipe(worldPos);
            }
        }
    }

    void HandleSwipe(Vector2 endTouch)
    {
        Vector2 swipe = endTouch - startTouch;

        // Tap
        if (swipe.magnitude < 0.1f)
        {
            isDragging = false;
            if (board != null)
                board.SelectFruit(this);
            return;
        }

        Vector2 dir;

        // Horizontal swipe
        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
        {
            dir = new Vector2(Mathf.Sign(swipe.x), 0);
        }
        else // Vertical swipe
        {
            dir = new Vector2(0, Mathf.Sign(swipe.y));
        }

        // 🔥 Garantir EXACTAMENTE -1 ou 1 (sem floats estranhos)
        dir.x = Mathf.Clamp(dir.x, -1f, 1f);
        dir.y = Mathf.Clamp(dir.y, -1f, 1f);

        // Chamar swap no Board
        if (board != null)
            board.SwapWithNeighbor(this, dir);

        isDragging = false;
    }
}
