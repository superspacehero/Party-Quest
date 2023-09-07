using UnityEngine;

public class FollowWorldPosition : MonoBehaviour
{
    public GameThing target
    {
        private get
        {
            if (_target == null)
            {
                // Recursively search for a GameThing component on the parent object
                Transform parent = transform.parent;

                while (parent.parent != null)
                    parent = parent.parent;

                parent.TryGetComponent(out _target);
            }
            return _target;
        }

        set
        {
            _target = value;
        }
    }
    private GameThing _target;

    [SerializeField, Range(0f, 1f)] private float followHeight = 1f;

    void Start()
    {
        Move();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        if (target != null)
        {
            // Set the position of the UI object to the position of the target object in world space
            (transform as RectTransform).position = (Vector2)GameplayCamera.instance.myCamera.WorldToScreenPoint(Vector3.Lerp(target.transform.position, target.thingTop.position, followHeight));
        }
    }
}