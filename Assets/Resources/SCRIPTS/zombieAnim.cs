using UnityEngine;
using System.Collections;

public class zombieAnim : MonoBehaviour
{

    private Animator anim;

    private Rigidbody rigidBody;

    private NavMeshAgent agent;

    private AudioSource audio;

    public Transform Target;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        audio = GetComponent<AudioSource>();
    }
	
    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(Target.position);
        anim.SetFloat("velocity", rigidBody.velocity.magnitude);

        int random = Random.Range(1, 500);

        if (random == 50)
        {
            if (!audio.isPlaying)
            {
                audio.Play();
            }
        }
    }
}
