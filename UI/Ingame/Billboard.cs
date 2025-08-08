using UnityEngine;

public class Billboard : MonoBehaviour {
  [SerializeField] private BillboardType billboardType;

  [Header("Lock Rotation")]
  [SerializeField] private bool lockX;
  [SerializeField] private bool lockY;
  [SerializeField] private bool lockZ;

  [Header("Hover Settings")]
  [SerializeField] private bool enableHover = true;
  [SerializeField] private float hoverAmplitude = 0.25f;
  [SerializeField] private float hoverFrequency = 2f;

  private Vector3 originalRotation;
  private Vector3 startPosition;

  public enum BillboardType { LookAtCamera, CameraForward };

  private void Awake() {
    originalRotation = transform.rotation.eulerAngles;
    startPosition = transform.position;
  }

  // Use Late update so everything should have finished moving.
  void LateUpdate() {
    ApplyBillboard();
    ApplyHover();
  }

  private void ApplyBillboard() {
    switch (billboardType) {
      case BillboardType.LookAtCamera:
        transform.LookAt(Camera.main.transform.position, Vector3.up);
        break;
      case BillboardType.CameraForward:
        transform.forward = Camera.main.transform.forward;
        break;
    }

    Vector3 rotation = transform.rotation.eulerAngles;
    if (lockX) rotation.x = originalRotation.x;
    if (lockY) rotation.y = originalRotation.y;
    if (lockZ) rotation.z = originalRotation.z;
    transform.rotation = Quaternion.Euler(rotation);
  }

  private void ApplyHover() {
    if (!enableHover) return;
    float hoverOffset = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
    transform.position = startPosition + new Vector3(0f, hoverOffset, 0f);
  }
}