using UnityEngine;

public class ClickDetector : MonoBehaviour
{
    [SerializeField] private bool IsSecondaryCamera = false;
    [SerializeField] private Camera cam;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && (GameManager.Instance.GlobalInputEnabled || IsSecondaryCamera))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                if(hit.collider.TryGetComponent(out IClickable clickable))
                {
                    clickable.OnClick();
                }
            }
        }
    }
}