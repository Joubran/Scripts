using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCRoaming : MonoBehaviour {
  [Header("Roaming Settings")]
  [SerializeField] private float roamRadius = 10f;
  [SerializeField] private float waitTime = 2f;
  private Animator anim;

  private NavMeshAgent agent;
  private Vector3 startPosition;

  private void Start() {
      agent = GetComponent<NavMeshAgent>();
      anim = GetComponent<Animator>();
      startPosition = transform.position;
      StartCoroutine(Roam());
  }

  private IEnumerator Roam() {
    while (true) {
        anim.SetBool("IsWalking", true);
        Vector3 destination = GetRandomNavMeshPoint(startPosition, roamRadius);
        agent.SetDestination(destination);

        // Wait until agent reaches the point
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance) {
              yield return null;
        }
        anim.SetBool("IsWalking", false);

        yield return new WaitForSeconds(waitTime);
    }
  }

  private Vector3 GetRandomNavMeshPoint(Vector3 center, float radius) {
      for (int i = 0; i < 30; i++) {
          Vector3 randomPoint = center + Random.insideUnitSphere * radius;
          randomPoint.y = center.y;
          if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas)) {
              return hit.position;
          }
      }
      return center;
  }
}
