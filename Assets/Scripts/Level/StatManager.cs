using UnityEngine;

public class StatManager : MonoBehaviour
{
    [Header("Tower Stats")]
    public int _brickTotal;
    public int _towerHeight;
    public int _totalEssenceEarn;
    public int _totalPureEssenceEarn;
    public int _totalPureEssenceSpent;
    public int _totalDamageTaken;
    public int _highestDamageTaken;
    [Header("Ball Stats")]
    public int _maxDamage;
    public int _totalDamage;
    public int _avrDamage;
    public int _totalCard;
    public int _totalCommonCard, _totalUncommonCard, _totalRareCard, _totalLegendaryCard;
    public int _totalSpell;
    public int _totalExplosionSpell,_totalDischargeSpell,_totalToxicSpell,_totalCritSpell;
    public int _totalTimeRedirectBall;
    public int _totalBallDestroyed;
    public int _highestCombo;
    [Header("Enemy")]
    public int _totalBrickDestroyed;


    #region Tower Stats

    public void AddBrickTotal()
    {
        _brickTotal++;
    }

    public void AddTowerHeight()
    {
        _towerHeight ++;
    }

    public void AddTotalEssenceEarn(int amt = 1)
    {
        _totalEssenceEarn += amt;
    }

    public void AddTotalPureEssenceEarn()
    {
        _totalPureEssenceEarn++;
    }

    public void AddTotalPureEssenceSpent(int amount = 1)
    {
        _totalPureEssenceSpent += amount;
    }

    public void AddTotalDamageTaken(int amount)
    {
        _totalDamageTaken += amount;
    }

    public void AddHighestDamageTaken(int amount)
    {
        if (amount > _highestDamageTaken)
            _highestDamageTaken = amount;
    }
    public int GetTowerHeight() => _towerHeight;

    #endregion


    #region Ball Stats

    public void AddMaxDamage(int amount)
    {
        if (amount > _maxDamage)
            _maxDamage = amount;
    }
    public void AddTotalDamage(int amount)
    {
        _totalDamage += amount;
    }
    public void AddTotalCard(int amount = 1)
    {
        _totalCard += amount;
    }

    public void AddTotalCommonCard(int amount = 1)
    {
        _totalCommonCard += amount;
    }

    public void AddTotalUncommonCard(int amount = 1)
    {
        _totalUncommonCard += amount;
    }

    public void AddTotalRareCard(int amount = 1)
    {
        _totalRareCard += amount;
    }

    public void AddTotalLegendaryCard(int amount = 1)
    {
        _totalLegendaryCard += amount;
    }

    public void AddBallSpell()
    {
        _totalSpell ++;
    }

    public void AddTotalExplosionSpell(int amount = 1)
    {
        _totalExplosionSpell += amount;
    }

    public void AddTotalDischargeSpell(int amount = 1)
    {
        _totalDischargeSpell += amount;
    }

    public void AddTotalToxicSpell(int amount = 1)
    {
        _totalToxicSpell += amount;
    }

    public void AddTotalCritSpell(int amount = 1)
    {
        _totalCritSpell += amount;
    }

    public void AddTotalTimeRedirectBall()
    {
        _totalTimeRedirectBall ++;
    }

    public void AddTotalBallDestroyed()
    {
        _totalBallDestroyed ++;
    }

    public void AddHighestCombo(int amount)
    {
        if (amount > _highestCombo)
            _highestCombo = amount;
    }

    #endregion


    #region Enemy Stats

    public void AddTotalBrickDestroyed()
    {
        _totalBrickDestroyed ++;
    }

    #endregion
}
