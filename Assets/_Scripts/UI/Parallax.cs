using UnityEngine;
using UnityEngine.UI;

public class Parallax : MonoBehaviour
{
    public static Parallax Instance;

    [SerializeField] private BackgroundLayer[] backgroundLayers;
    [SerializeField] private bool disableParallax;

    private Transform player;
    private Vector3 initialPlayerPosition;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player != null) {
            initialPlayerPosition = player.position;
        }
    }

    private void Update() {
        if (disableParallax || player == null) return;

        foreach (BackgroundLayer layer in backgroundLayers) {
            UpdateBackgroundLayer(layer);
        }

        initialPlayerPosition = player.position;
    }

    private void UpdateBackgroundLayer(BackgroundLayer layer) {
        Vector3 playerOffset = player.position - initialPlayerPosition;

        Vector2 offset = new Vector2(playerOffset.x * layer.scroll.x, playerOffset.y * layer.scroll.y);
        layer.image.uvRect = new Rect(
            layer.image.uvRect.position + offset * Time.deltaTime, 
            layer.image.uvRect.size);
    }

    public void ToggleScrolling() {
        disableParallax = !disableParallax;
    }
}

[System.Serializable]
public class BackgroundLayer
{
    public string layerName;
    public RawImage image;
    public Vector2 scroll;
}

