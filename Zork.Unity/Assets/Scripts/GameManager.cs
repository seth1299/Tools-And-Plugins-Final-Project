
using UnityEngine;
using Zork;
using Newtonsoft.Json;
using TMPro;

public class GameManager : MonoBehaviour
    
{

    [SerializeField]
    private UnityInputService InputService;

    [SerializeField]
    private UnityOutputService OutputService;

    [SerializeField]
    private TextMeshProUGUI CurrentLocationText;

    [SerializeField]
    private TextMeshProUGUI ScoreText;

    [SerializeField]
    private TextMeshProUGUI MovesText;

    void Start()
    {
        TextAsset gameTextAsset = Resources.Load<TextAsset>(ZorkGameFilename); //No file extension due to Unity just referring it to Zork. 
        _game = JsonConvert.DeserializeObject<Game>(gameTextAsset.text);
        _game.Player.LocationChanged += (sender, newLocation) => CurrentLocationText.text = newLocation.ToString();

        if ( InputService != null && OutputService != null )
            _game?.Start(InputService, OutputService);
    }

    
    private void Update()
    {
        if (!_game.IsRunning)
        {
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        }

        if ( _game.Player.Location != _previousLocation )
        {
            CurrentLocationText.text = _game.Player.Location.ToString();
            _previousLocation = _game.Player.Location;
        }
        if ( _game.GetPlayerScore() != _previousScore)
        {
            ScoreText.text = ($"Score: {_game.Player.Score.ToString()}");
            _previousScore = _game.GetPlayerScore();
        }
        if (_game.GetPlayerMoves() != _previousMoves)
        {
            MovesText.text = ($"Moves: {_game.Player.Moves.ToString()}");
            _previousMoves = _game.GetPlayerMoves();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            InputService.ProcessInput();
        }
    }

    [SerializeField]
    private string ZorkGameFilename = "Zork";

    private Game _game;
    private Room _previousLocation;
    private int _previousScore, _previousMoves;
}
