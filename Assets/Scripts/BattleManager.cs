using UnityEngine;

public class BattleManager : MonoBehaviour
{

    private const string FILE_NAME = "GameConfig";
    [SerializeField] private BattleView _view;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private Transform _spriteSurface;

    private BattleModel _model;
    private BattleController _controller;
    private bool _simulationStarted;

    private void Awake()
    {
        var parser = new JSONParser();
        var gameData = parser.ParseFile<GameConfigData>(FILE_NAME);

        _model = new BattleModel(gameData);
        _controller = new BattleController(_model);
    }
    // Start is called before the first frame update
    void Start()
    {
        _controller.Init();
        _controller.SimulationEnded += OnSimulationEnded;

        _view.Init();
        _view.RestartButtonPressed += OnRestartButtonPressed;
        _view.SliderValueChanged += OnSliderValueChanged;
        _view.ScreenMoved += OnScreenMoved;
        _view.ObjectsCreated += ()=> _simulationStarted = true;
        _view.CreateObjects(_model.CirclesList.ToArray(),_model.SpawnDelay);
    }
    private void OnScreenMoved(float deltaX)
    {
        _cameraController.Move(deltaX);
    }
    private void OnRestartButtonPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    private void OnSliderValueChanged(float value)
    {
        _controller.ChangeSimulationSpeed(value);
    }
    public void OnSimulationEnded(float time, string winner)
    {
        _controller.SimulationEnded -= OnSimulationEnded;
        _simulationStarted = false;
        _view.ShowBattleOverPanel(time, winner);
    }

    // Update is called once per frame
    void Update()
    {
        if (_simulationStarted)
        {
            Simulate();
        }
    }

    private void Simulate()
    {
        _controller.UpdateSimulation(Time.deltaTime);

        float blue = _model.GetSquare(CircleColor.Blue);
        float red = _model.GetSquare(CircleColor.Red);
        _view.ShowRatio(blue / (blue+red));
        _view.UpdateCircles();
    }
}
