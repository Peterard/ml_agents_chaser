using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class Escapee : Agent
{
    private Rigidbody2D rb;
    private List<GameObject> chasers;
    private float maxSpeed;
    private Vector2 parentPosition;

    public override void InitializeAgent()
    {
        rb = GetComponent<Rigidbody2D>();

        parentPosition = gameObject.transform.parent.transform.position;

        Transform parent = gameObject.transform.parent;
        float childCount = parent.childCount;

        chasers = new List<GameObject>();
        for (int i = 0; i < childCount; i++){
            Transform child = parent.GetChild(i);
            if(child.tag == "chaser"){
                chasers.Add(child.gameObject.gameObject);
            }
        }

        maxSpeed = 4f;
    }

    public override void CollectObservations()
    {
        AddVectorObs((gameObject.transform.position.x - parentPosition.x) / 11);
        AddVectorObs((gameObject.transform.position.y - parentPosition.y) / 5);
        foreach (GameObject chaser in chasers)
        {
            AddVectorObs((chaser.transform.position.x - parentPosition.x) / 11);
            AddVectorObs((chaser.transform.position.y - parentPosition.y) / 5);
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        Debug.Log("Inputs");
        Debug.Log(vectorAction[0]);
        Debug.Log(vectorAction[1]);
        Vector2 directedVelocity = 4 * new Vector2(vectorAction[0], vectorAction[1]);
        directedVelocity = Vector2.ClampMagnitude(directedVelocity, maxSpeed);

        rb.velocity = directedVelocity;

        foreach (GameObject chaser in chasers)
        {
            Debug.Log("Distance");
            Debug.Log(Vector2.Distance(gameObject.transform.position, chaser.transform.position));
            AddReward(0.00005f * Vector2.Distance(gameObject.transform.position, chaser.transform.position));
        }
    }

    public override void AgentReset()
    {
        gameObject.transform.position = parentPosition + new Vector2(0, 0);

        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        gameObject.transform.position = parentPosition + new Vector2(Random.Range(-5f, 5f), Random.Range(-3f, 3f));

    }

}
