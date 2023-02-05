using UnityEngine;

public class ItemSwitching : MonoBehaviour
{
    public Lasso lasso;
    public int selectedItem = 0;

    private void Start()
    {
        SelectItem();
    }

    private void Update()
    {
        int previousSelectedItem = selectedItem;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            lasso.Detatch();

            if (selectedItem >= transform.childCount - 1)
                selectedItem = 0;
            else
                selectedItem++;
        }

        if (previousSelectedItem != selectedItem)
            SelectItem();
    }

    void SelectItem()
    {
        int i = 0;
        foreach (Transform item in transform)
        {
            if (i == selectedItem)
                item.gameObject.SetActive(true);
            else
                item.gameObject.SetActive(false);
            i++;
        }
    }
}
