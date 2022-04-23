using System;
using UnityEngine;

public class WolfBehavior : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rigidbody2d;
    private LineRenderer lineRenderer;

    public GameObject favoriteTree;

    public float speed = 1.5f;
    public float timeToWalkTotal = 3.0f;
    public float timeToWalkRemaining = 0;
    public bool walkingLeft = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2d = GetComponent<Rigidbody2D>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.05f;

        // Choose a favorite tree randomly
        TreeBehavior[] treeBehaviors = FindObjectsOfType<TreeBehavior>();
        if (treeBehaviors.Length > 0)
        {
            favoriteTree = treeBehaviors[UnityEngine.Random.Range(0, treeBehaviors.Length)].gameObject;
        }

        DynamicObject dynamicObject = GetComponent<DynamicObject>();
        dynamicObject.prepareToSaveDelegates += PrepareToSaveObjectState;
        dynamicObject.loadObjectStateDelegates += LoadObjectState;
    }

    void Update()
    {
        timeToWalkRemaining -= Time.deltaTime;
        if (timeToWalkRemaining <= 0)
        {
            timeToWalkRemaining = timeToWalkTotal;
            walkingLeft = !walkingLeft;
        }
        animator.SetFloat("MoveX", walkingLeft ? -1 : 1);
    }

    private void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position += speed * (walkingLeft ? Vector2.left : Vector2.right) * Time.deltaTime;
        rigidbody2d.MovePosition(position);

        if (favoriteTree == null)
        {
            favoriteTree = null;
            lineRenderer.positionCount = 0;
        } else
        {
            Vector3[] positions = { transform.position, favoriteTree.transform.position };
            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(positions);
        }
    }

    private void PrepareToSaveObjectState(ObjectState objectState)
    {
        objectState.genericValueMap["WolfBehavior.speed"] = speed.ToString();
        objectState.genericValueMap["WolfBehavior.timeToWalkRemaining"] = timeToWalkRemaining.ToString();
        objectState.genericValueMap["WolfBehavior.walkingLeft"] = walkingLeft.ToString();
        if (favoriteTree != null)
        {
            objectState.genericValueMap["WolfBehavior.favoriteTree"] = favoriteTree.GetComponent<DynamicObject>().objectState.guid;
        } else if (objectState.genericValueMap.ContainsKey("WolfBehavior.favoriteTree"))
        {
            objectState.genericValueMap.Remove("WolfBehavior.favoriteTree");
        }
    }

    private void LoadObjectState(ObjectState objectState)
    {
        speed = Convert.ToSingle(objectState.genericValueMap["WolfBehavior.speed"]);
        timeToWalkRemaining = Convert.ToSingle(objectState.genericValueMap["WolfBehavior.timeToWalkRemaining"]);
        walkingLeft = Convert.ToBoolean(objectState.genericValueMap["WolfBehavior.walkingLeft"]);
        if (objectState.genericValueMap.ContainsKey("WolfBehavior.favoriteTree"))
        {
            favoriteTree = SaveUtils.FindDynamicObjectByGuid(Convert.ToString(objectState.genericValueMap["WolfBehavior.favoriteTree"])).gameObject;
        }
    }
}
