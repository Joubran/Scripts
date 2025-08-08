using TMPro;
using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour
{
    [SerializeField] private CollectableType collectableType;
    [SerializeField] private int frequency = 1; // how often does it appear after deactivating in seconds 
    [SerializeField] private TMP_Text flyTextAmount;
    [SerializeField] public CoinAmount amountToCollect;
    private Animator anim;

    public enum CollectableType { Exp, Energy };

    void Start()
    {
        anim = GetComponent<Animator>();
        flyTextAmount.text = CoinManager.Instance.GetFormattedCoins(amountToCollect);
        flyTextAmount.gameObject.SetActive(false);
    }
    public void Collect()
    {
        if (collectableType == CollectableType.Exp)
        {
            CoinManager.Instance.AddCoins(amountToCollect);
            Debug.Log("[Collectable.cs] Collected: " + amountToCollect.mantissa.ToString() + "x 10^" + amountToCollect.exponent.ToString() + " EXP!!!");
        }
        else if (collectableType == CollectableType.Energy)
        {
            CoinManager.Instance.AddEnergy(amountToCollect);
            Debug.Log("[Collectable.cs] Collected: " + amountToCollect.mantissa.ToString() + "x 10^" + amountToCollect.exponent.ToString() + " Energy!!!");
        }
        else
        {
            Debug.Log("[Collectable.cs] Error!! Specify type!!!");
        }
        flyTextAmount.text = CoinManager.Instance.GetFormattedCoins(amountToCollect);
        anim.SetTrigger("Collect");
        StartCoroutine(WaitAndActivate(frequency));
    }

    private IEnumerator WaitAndActivate(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        anim.SetTrigger("Activate");
    }

}
