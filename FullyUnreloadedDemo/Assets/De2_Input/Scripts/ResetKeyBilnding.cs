using UnityEngine;
using UnityEngine.InputSystem;

public class ResetKeyBilnding : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;


    public void ResetAllBindings()
    {
        foreach (InputActionMap map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }

        PlayerPrefs.DeleteKey("rebinds");
    }


}
