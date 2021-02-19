using System.Collections.Generic;

[System.Serializable]
public class GameConfigData
{
    public Dictionary<string, float> GameConfig = new Dictionary<string, float>();

    public float GameAreaWidth => GameConfig["gameAreaWidth"];
    public float GameAreaHeight => GameConfig["gameAreaHeight"];
    public int NumUnitsToSpawn =>(int)GameConfig["numUnitsToSpawn"];
    public float UnitSpawnDelay => GameConfig["unitSpawnDelay"];
    public float UnitSpawnMinRadius => GameConfig["unitSpawnMinRadius"];
    public float UnitSpawnMaxRadius => GameConfig["unitSpawnMaxRadius"];
    public float UnitSpawnMinSpeed => GameConfig["unitSpawnMinSpeed"];
    public float UnitSpawnMaxSpeed => GameConfig["unitSpawnMaxSpeed"];
    public float UnitDestroyRadius => GameConfig["unitDestroyRadius"];


}