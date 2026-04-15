using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public const int UNITS_LAYER = 6;

   public static GameAssets instance {  get; private set; }

    private void Awake() {
        instance = this;   
    }
}
