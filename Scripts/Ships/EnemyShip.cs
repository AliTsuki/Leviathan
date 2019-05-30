// Controls the enemy ships
public class EnemyShip : Ship
{
    // Enemy ship types
    public enum EnemyShipType
    {
        Standard,
        Ramming,
        Broadside
    }
    public EnemyShipType Type;
}
