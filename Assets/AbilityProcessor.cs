using UnityEngine;

public class AbilityProcessor
{
    public void DoAbilityAnimation(EncounterTargeter targeter, ref int activeAnimationCount, AbilityAnimation abilityAnimationPrefab, Ability pickedAbility, Subject encounterSubject, EncounterWindowController encounterWindowController)
    {
        activeAnimationCount += targeter.SelectedTargets().Count;
        
        targeter.SelectedTargets().ForEach(x =>
        {
            var abilityAnimation = GameObject.Instantiate(abilityAnimationPrefab, Vector2.zero, Quaternion.identity)
                .GetComponent<AbilityAnimation>();
            abilityAnimation.transform.parent = encounterWindowController.transform;
            abilityAnimation.Setup(encounterSubject);
            abilityAnimation.PlayAnimation(x.transform, pickedAbility.animationName);
        });
    }
}