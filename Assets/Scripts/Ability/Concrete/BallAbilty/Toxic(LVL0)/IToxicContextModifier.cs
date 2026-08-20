using UnityEngine;

public interface IToxicContextModifier
{
    void ModifyToxicContextAdd(AbilityContext toxicContext);
    void ModifyToxicContextSubtract(AbilityContext toxicContext);
    void ModifyToxicContextMultiple(AbilityContext toxicContext);
    void ModifyToxicContextDivide(AbilityContext toxicContext);

}
