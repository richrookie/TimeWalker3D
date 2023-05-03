using UnityEngine;

public class Managers : MonoBehaviour
{
    ///<summary>내부적으로 사용되는 Managers 변수</summary>
    static Managers _instance;
    ///<summary>내부적으로 사용되는 Managers Property</summary>
    static Managers Instance { get { Init(); return _instance; } }

    [SerializeField]
    DataManager _data = new DataManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SoundManager _sound = new SoundManager();

    [HideInInspector]
    GameManager _game;

    public static DataManager Data => Instance._data;
    public static PoolManager Pool => Instance._pool;
    public static ResourceManager Resource => Instance._resource;
    public static SoundManager Sound => Instance._sound;

    public static GameManager Game => Instance._game;



    ///<summary>가장 처음 매니저 만들때 한번 Init</summary>
    static void Init()
    {
        if (_instance == null && Application.isPlaying)
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            UnityEngine.Input.multiTouchEnabled = false;

            GameObject go = new GameObject { name = "@Managers" };
            go.AddComponent<Managers>();
            DontDestroyOnLoad(go);
            _instance = go.GetComponent<Managers>();

            _instance._sound.Init();
            _instance._data.Init();
            _instance._pool.Init();
            _instance._resource.Init();

            _instance._game = go.AddComponent<GameManager>();

            GameInit();
        }
    }

    public static void GameInit()
    {
        Instance._game.Init();
    }

    ///<summary>새로운 씬으로 갈때마다 클리어</summary>
    public static void Clear()
    {
        Sound.Clear();
        Pool.Clear();
        Resource.Clear();
        Game.Clear();
    }

}
