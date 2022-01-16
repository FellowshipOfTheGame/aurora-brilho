using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Portal : MonoBehaviour
{
    [SerializeField] Canvas endCanvas;
    [SerializeField] Animator cameraAnimator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            endCanvas.gameObject.SetActive(true);
            cameraAnimator.SetTrigger("zoom_out");

            collision.gameObject.GetComponent<AuroraMovement>().StopInput(true);
        }
    }
}
