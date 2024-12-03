using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public class CharacterSelectorUI : MonoBehaviour
{
    [SerializeField] private Transform characterSelector;
    [SerializeField] private TMP_InputField playerNameInput;

    private List<SO_PlayerData> playerDetailsList;
    private GameObject playerSelectionPrefab;
    private SO_CurrentPlayerData currentPlayer;
    private List<GameObject> playerCharacterGameObjectList = new List<GameObject>();
    private Coroutine coroutine;
    private int selectedPlayerIndex = 0;
    private float offset = 4f;

    //===========================================================================
    private void Awake()
    {
        playerSelectionPrefab = GameResources.Instance.playerSelectionPrefab;
        playerDetailsList = GameResources.Instance.playerDetailsList;
        currentPlayer = GameResources.Instance.currentPlayerData;
    }

    private void Start()
    {
        // Instatiate player characters
        for (int i = 0; i < playerDetailsList.Count; i++)
        {
            GameObject playerSelectionObject = Instantiate(playerSelectionPrefab, characterSelector);
            playerCharacterGameObjectList.Add(playerSelectionObject);
            playerSelectionObject.transform.localPosition = new Vector3((offset * i), 0f, 0f);
            PopulatePlayerDetails(playerSelectionObject.GetComponent<PlayerSelectionUI>(), playerDetailsList[i]);
        }

        playerNameInput.text = currentPlayer.playerName;

        // Initialise the current player
        currentPlayer.playerData = playerDetailsList[selectedPlayerIndex];
    }

    //===========================================================================
    private void PopulatePlayerDetails(PlayerSelectionUI playerSelection, SO_PlayerData playerDetails)
    {
        playerSelection.playerHandSpriteRenderer.sprite = playerDetails.HandSprite;
        playerSelection.playerHandNoWeaponSpriteRenderer.sprite = playerDetails.HandSprite;
        playerSelection.playerWeaponSpriteRenderer.sprite = playerDetails.StartingWeapon.weaponSprite;
        playerSelection.animator.runtimeAnimatorController = playerDetails.RuntimeAnimatorController;
    }

    public void NextCharacter()
    {
        if (selectedPlayerIndex >= playerDetailsList.Count - 1)
            return;
        selectedPlayerIndex++;

        currentPlayer.playerData = playerDetailsList[selectedPlayerIndex];

        MoveToSelectedCharacter(selectedPlayerIndex);
    }

    public void PreviousCharacter()
    {
        if (selectedPlayerIndex == 0)
            return;

        selectedPlayerIndex--;

        currentPlayer.playerData = playerDetailsList[selectedPlayerIndex];

        MoveToSelectedCharacter(selectedPlayerIndex);
    }

    private void MoveToSelectedCharacter(int index)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(MoveToSelectedCharacterRoutine(index));
    }

    private IEnumerator MoveToSelectedCharacterRoutine(int index)
    {
        float currentLocalXPosition = characterSelector.localPosition.x;
        float targetLocalXPosition = index * offset * characterSelector.localScale.x * -1f;

        while (Mathf.Abs(currentLocalXPosition - targetLocalXPosition) > 0.01f)
        {
            currentLocalXPosition = Mathf.Lerp(currentLocalXPosition, targetLocalXPosition, Time.deltaTime * 10f);

            characterSelector.localPosition = new Vector3(currentLocalXPosition, characterSelector.localPosition.y, 0f);
            yield return null;
        }

        characterSelector.localPosition = new Vector3(targetLocalXPosition, characterSelector.localPosition.y, 0f);
    }

    public void UpdatePlayerName()
    {
        playerNameInput.text = playerNameInput.text.ToUpper();

        currentPlayer.playerName = playerNameInput.text;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(characterSelector), characterSelector);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerNameInput), playerNameInput);
    }
#endif
}