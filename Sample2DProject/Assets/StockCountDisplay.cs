using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StockCountDisplay : MonoBehaviour
{
    public TextMeshProUGUI stockCountText;
    // Start is called before the first frame update
    public void Start()
    {
        stockCountText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        stockCountText.text = GameManager.Instance.stockCount.ToString();
    }
}
