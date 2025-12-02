using UnityEngine;
using UnityEngine.AI;

public class PlayerParticleController : MonoBehaviour
{
    private ParticleSystem ps;
    private NavMeshAgent agent;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        agent = GetComponentInParent<NavMeshAgent>();
    }

    private void Update()
    {
        if (agent == null || ps == null) return;

        if (agent.velocity.magnitude > 1f)
        {
            if (!ps.isPlaying) ps.Play();
        }
        else
        {
            if (ps.isPlaying) ps.Stop();
        }
    }
}
