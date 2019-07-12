// Controls the enemy ships
public abstract class EnemyShip : Ship
{
    // Enemy ship types
    public enum EnemyShipType
    {
        Standard,
        Ramming,
        Broadside,
        Flanker
    }
    protected EnemyShipType Type;
}
