using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject returnToMainMenuButton;

    private bool isInstructionSceneLoaded = false;
    private bool isHighScoresSceneLoaded = false;

    //===========================================================================
    private void Start()
    {
        // Play Music
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);

        // Load Character selector scene additively
        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);

        returnToMainMenuButton.SetActive(false);
    }

    //===========================================================================
    public void PlayGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    public void LoadCharacterSelector()
    {
        returnToMainMenuButton.SetActive(false);

        if (isHighScoresSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("HighScoreScene");
            isHighScoresSceneLoaded = false;
        }
        else if (isInstructionSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("InstructionsScene");
            isInstructionSceneLoaded = false;
        }

        playButton.SetActive(true);
        quitButton.SetActive(true);

        // Load character selector scene additively
        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(playButton), playButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(quitButton), quitButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(returnToMainMenuButton), returnToMainMenuButton);
    }
#endif
}