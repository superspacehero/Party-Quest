using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Sirenix.OdinInspector;

public class General
{
    #region Cached Animations

        public static readonly int Speed = Animator.StringToHash("Speed");
        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int Walk = Animator.StringToHash("Walk");
        public static readonly int Jump = Animator.StringToHash("Jump");
        public static readonly int Fall = Animator.StringToHash("Fall");
        public static readonly int Land = Animator.StringToHash("Land");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Crouch = Animator.StringToHash("Crouch");

    #endregion

    #region Delayed Functions

        public static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        public static void DelayedFunctionFrames(MonoBehaviour monoBehaviour, System.Action action, int delayFrames = 1)
        {
            monoBehaviour.StartCoroutine(DelayedFunctionCoroutine(action, delayFrames));
        }

        public static void DelayedFunctionSeconds(MonoBehaviour monoBehaviour, System.Action action, float delaySeconds = 0)
        {
            monoBehaviour.StartCoroutine(DelayedFunctionCoroutine(action, Mathf.RoundToInt(delaySeconds / Time.fixedDeltaTime)));
        }

        public static IEnumerator DelayedFunctionCoroutine(System.Action action, int framesToDelay = 0)
        {
            if (framesToDelay > 1)
            {
                framesDelayed = 0;
                while (framesDelayed < framesToDelay)
                {
                    framesDelayed++;
                    yield return waitForFixedUpdate;
                }
            }
            else
                yield return waitForFixedUpdate;

            action();
        }
        private static int framesDelayed;

    #endregion

    #region Users

        [System.Serializable]
        public class User
        {
            public string username, id, profilePicture;

            public static User GetUserFromID(string id)
            {
                foreach (User user in users)
                    if (user.id == id)
                        return user;
                return null;
            }

            public Sprite GetUserProfilePicture()
            {
                return StringToSprite(profilePicture);
            }
        }

        public static User currentUser;

        public static List<User> users = new List<User>();

    #endregion

    #region Sprite-String Conversion

        public static Sprite StringToSprite(string spriteString)
        {
            // Check if the string is empty or null
            if (string.IsNullOrEmpty(spriteString))
                return null;

            byte[] imageBytes = System.Convert.FromBase64String(spriteString);
            Texture2D tex = new Texture2D(2, 2);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.LoadImage( imageBytes );
            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

            return sprite;
        }

        public static string SpriteToString(Sprite sprite)
        {
            // If the sprite is null, return an empty string
            if (sprite == null || sprite.texture == null)
                return "";

            Texture2D tex = sprite.texture;
            byte[] imageBytes = tex.EncodeToPNG();

            string base64String = System.Convert.ToBase64String(imageBytes);

            return base64String;
        }

        public static string TextureToString(Texture2D texture)
        {
            byte[] imageBytes = texture.EncodeToPNG();
            string base64String = System.Convert.ToBase64String(imageBytes);

            return base64String;
        }

        public static string TextureToString(RenderTexture renderTexture)
        {
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();

            return TextureToString(texture);
        }

    #endregion

    #region UI

        [System.Serializable]
        public struct RectTransformAnchorPreset
        {
            public Vector2 anchorMin, anchorMax, pivot;

            // [Button]
            public void SetAnchors(RectTransform rectTransform)
            {
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
                rectTransform.pivot = pivot;
            }

            public void SetAnchors(GameObject gameObject)
            {
                if (gameObject.TryGetComponent(out RectTransform rectTransform))
                    SetAnchors(rectTransform);
                else
                    Debug.LogError("GameObject does not have a RectTransform component", gameObject);
            }
        }

    #endregion

    #region Scene Management

        public static void LoadLevelScene(string levelString, GameMode gameMode, string sceneName)
        {
            // Debug.Log($"Loading level in game mode {gameMode.ToString()} in scene {sceneName}");

            GameManager.levelString = levelString;
            GameManager.gameMode = gameMode;
            LoadScene(sceneName);
        }

        public static void LoadScene(string sceneName)
        {
            if (!string.IsNullOrEmpty(sceneName))
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        public static void LoadScene(int sceneIndex)
        {
            if (sceneIndex >= 0)
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
        }

        public static void LoadSceneAdditive(string sceneName)
        {
            if (!string.IsNullOrEmpty(sceneName))
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }

        public static void LoadSceneAdditive(int sceneIndex)
        {
            if (sceneIndex >= 0)
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }

        public static void UnloadScene(string sceneName)
        {
            if (!string.IsNullOrEmpty(sceneName))
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
        }

        public static void UnloadScene(int sceneIndex)
        {
            if (sceneIndex >= 0)
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneIndex);
        }

    #endregion

    #region Object Pools

    [System.Serializable]
    public class ObjectPool<T> where T : Behaviour
    {
        [SerializeField] private GameObject objectPrefab;
        private List<T> objectPool = new List<T>();

        private Transform parent
        {
            get
            {
                if (_parent == null)
                    _parent = new GameObject(objectPrefab.name + " Pool").transform;

                return _parent;
            }
        }
        private Transform _parent;

        public T GetObjectFromPool(Vector3 objectPosition)
        {
            T component = null;

            foreach (T pooledObject in objectPool)
            {
                if (!pooledObject.gameObject.activeInHierarchy)
                {
                    component = pooledObject;
                    break;
                }
            }

            if (component == null)
            {
                GameObject obj = GameObject.Instantiate(objectPrefab, objectPosition, Quaternion.identity, parent);
                obj.TryGetComponent(out component);

                if (component != null)
                    objectPool.Add(component);
            }

            component.transform.position = objectPosition;
            component.gameObject.SetActive(true);

            return component;
        }

        public void ReturnObjectToPool(T obj)
        {
            obj.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Other

    /// <summary>
    /// Finds the first descendant of the given transform with the specified tag.
    /// </summary>
    /// <param name="parent">The starting transform from which to begin the search.</param>
    /// <param name="tag">The tag of the desired game object.</param>
    /// <returns>The transform of the found game object, or null if no such game object is found.</returns>
    public static Transform FindDescendantWithTag(Transform parent, string tag)
    {
        // Check the current transform.
        if (parent.CompareTag(tag))
        {
            return parent;
        }

        // Check all children.
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Transform result = FindDescendantWithTag(child, tag);
            if (result != null)
            {
                return result;
            }
        }

        // If no children or the current transform match, return null.
        return null;
    }

    #endregion
}
