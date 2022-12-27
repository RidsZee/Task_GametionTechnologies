using UnityEngine;

public interface IPlayerAttack
{
    void DoAttack();
}

public interface IPlayerDefend
{
    public void DoDefend();
}

public interface IPlayerWalk
{
    void DoWalk(CellData TargetCell);
}

public interface IPlayerOccupyCell
{
    void DoOccupyCell(CustomDataStructures.CellIndex CellIndex);
}