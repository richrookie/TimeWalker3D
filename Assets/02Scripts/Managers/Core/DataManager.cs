using UnityEngine;

[System.Serializable]
public class DataManager
{
    ///<summary>Manager생산할때 만들어짐</summary>
    public void Init()
    {
        GetData();
    }


    public bool UseHaptic
    {
        get => _useHaptic;
        set
        {
            _useHaptic = value;
            PlayerPrefs.SetInt("Haptic", _useHaptic ? 1 : 0);
        }
    }
    [SerializeField]
    private bool _useHaptic;

    public bool UseSound
    {
        get => _useSound;
        set
        {
            _useSound = value;
            PlayerPrefs.SetInt("Sound", _useSound ? 1 : 0);
            Managers.Sound.BgmOnOff(value);
        }
    }
    [SerializeField]
    private bool _useSound;


    public void SaveData()
    {
    }

    public void GetData()
    {
        UseHaptic = (PlayerPrefs.GetInt("Haptic", 1) != 0);
        UseSound = (PlayerPrefs.GetInt("Sound", 1) != 0);
    }
}
