using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class Chaser : Agent
{
    private Rigidbody2D rb;
    private List<GameObject> escapees;
    private float maxSpeed;
    private Vector2 parentPosition;

    public override void InitializeAgent()
    {
        rb = GetComponent<Rigidbody2D>();

        parentPosition = gameObject.transform.parent.transform.position;

        Transform parent = gameObject.transform.parent;
        float childCount = parent.childCount;

        escapees = new List<GameObject>();
        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.tag == "escapee")
            {
                escapees.Add(child.gameObject);
            }
        }

        maxSpeed = 3f;
    }

    public override void CollectObservations()
    {
        AddVectorObs((gameObject.transform.position.x - parentPosition.x) / 11);
        AddVectorObs((gameObject.transform.position.y - parentPosition.y) / 5);
        foreach(GameObject escapee in escapees){
            AddVectorObs((escapee.transform.position.x - parentPosition.x) / 11);
            AddVectorObs((escapee.transform.position.y - parentPosition.y) / 5);
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        Debug.Log("CInputs");
        Debug.Log(vectorAction[0]);
        Debug.Log(vectorAction[1]);

        Vector2 directedVelocity = 4 * new Vector2(vectorAction[0], vectorAction[1]);
        directedVelocity = Vector2.ClampMagnitude(directedVelocity, maxSpeed);

        rb.velocity = directedVelocity;

        foreach (GameObject escapee in escapees)
        {
            Debug.Log("CDistance");
            Debug.Log(Vector2.Distance(gameObject.transform.position, escapee.transform.position));
            AddReward(0.00075f - 0.00005f * Vector2.Distance(gameObject.transform.position, escapee.transform.position));
        }
    }

    public override void AgentReset()
    {
        gameObject.transform.position = parentPosition + new Vector2(0, 0);
    }

}
