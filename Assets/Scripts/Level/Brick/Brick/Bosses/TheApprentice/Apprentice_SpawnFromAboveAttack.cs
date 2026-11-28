using System.Collections;
using UnityEngine;

public class Apprentice_SpawnFromAboveAttack : BaseBossAttack
{

    [Header("Meteor Attack")]
    [SerializeField] GameObject _meteor;
    [SerializeField] GameObject _meteorWarning;
    [SerializeField] float _meteorWarningDelay;
    [SerializeField] int _amountToDrop;
    [SerializeField] Transform _spawnPos;

    [Header("Spawn Settings")]
    [SerializeField]  float _minSpawnX;
    [SerializeField] float _maxSpawnX;
    [SerializeField]  float _spawnInterval;

    protected override IEnumerator ExecuteAttack()
    {
        if (_meteor == null || _spawnPos == null)
        {
            Debug.LogError("Meteor prefab or spawn position is missing!");
            yield break;
        }

        yield return StartMovementAndWaitForReady();

        // Step 2: If movement failed or was interrupted,
        // do not activate the ability.
        if (!IsRunning || !IsMovementReady())
            yield break;

        // Step 3: Assign the stats and activate the ability.
        _abilityManager.SetBrickAbilityStats(_abilityStats);
        _abilityManager.ActivateAbilities(_ability);


        // Step 5: Wait for the movement or static
        // duration to finish.
        while (_bossManager.IsMoving)
        {
            yield return null;
        }

        for (int i = 0; i < _amountToDrop; i++)
        {
            // Generate a random X position.
            float randomX = Random.Range(_minSpawnX, _maxSpawnX);


            // Keep Y and Z based on the spawn position.
            Vector3 spawnPosition = new Vector3(
                randomX,
                _spawnPos.position.y,
                _spawnPos.position.z
            );

            Instantiate(_meteorWarning, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(_meteorWarningDelay);
            // Spawn the meteor.
            Instantiate(_meteor, spawnPosition, Quaternion.identity);

            // Wait before spawning the next meteor.
            if (i < _amountToDrop - 1)
                yield return new WaitForSeconds(_spawnInterval);
        }
        CompleteAttack();
    }
    protected override void OnAttackCompleted()
    {
        _abilityManager?.ResetAllAbility();
    }
}
