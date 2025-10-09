using UnityEngine;

public class IC_IntegrateInteraction : MonoBehaviour
{
    [SerializeField] private GameObject generateTakeOrderButton;
    [SerializeField] private GameObject makeOrderButton;
    [SerializeField] private GameObject serveOrderButton;
    public bool register = false;
    public bool machine = false;
    public bool servingTray = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (register) generateTakeOrderButton.SetActive(true);
            if (machine) makeOrderButton.SetActive(true);
            if (servingTray) serveOrderButton.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            generateTakeOrderButton?.SetActive(false);
            makeOrderButton?.SetActive(false);
            serveOrderButton?.SetActive(false);
        }
    }
}
