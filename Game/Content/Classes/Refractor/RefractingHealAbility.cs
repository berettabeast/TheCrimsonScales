using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fractural.Tasks;
using Godot;

public class RefractingHealAbility : HealAbility
{
	public static class RefractionState
	{
		public static void SetDidRefract(AbilityState state, bool value, int index = 0) =>
			state.SetCustomValue(typeof(RefractingHealAbility), $"DidRefract_{index}", value);

		public static bool DidRefract(AbilityState state, int index = 0) =>
        state.GetCustomValue<bool>(typeof(RefractingHealAbility), $"DidRefract_{index}");

		public static void SetRefractCount(AbilityState state, int value, int index = 0) =>
			state.SetCustomValue(typeof(RefractingHealAbility), $"RefractCount_{index}", value);

		public static int RefractCount(AbilityState state, int index = 0) =>
			state.GetCustomValue<int>(typeof(RefractingHealAbility), $"RefractCount_{index}");

		public static void SetUsedPrisms(AbilityState state, List<Prism> prisms, int index = 0) =>
			state.SetCustomValue(typeof(RefractingHealAbility), $"UsedPrisms_{index}", prisms);

		public static List<Prism> GetUsedPrisms(AbilityState state, int index = 0) =>
			state.GetCustomValue<List<Prism>>(typeof(RefractingHealAbility), $"UsedPrisms_{index}");
	}

    private Hex _startingPerformHex;

    public class RefractingHealBuilder : AbstractBuilder<RefractingHealBuilder, RefractingHealAbility>
    {
        internal RefractingHealBuilder() { }
    }

    public static new RefractingHealBuilder Builder()
    {
        return new RefractingHealBuilder();
    }


	protected override async GDTask Perform(State abilityState)
	{		
        if (CustomGetPerformHex != null)
        {
            Hex performHex = CustomGetPerformHex(abilityState);
            abilityState.SetPerformHex(performHex);
        }

		int healIndex = 0;
        _startingPerformHex = abilityState.GetPerformHex;
		Figure performer = abilityState.Performer;
        
        if(abilityState.AbilityAOEPattern != null)
		{
			List<AOEHex> aoeHexes;

			Hex aoeOrigin = CustomGetPerformHex != null
				? abilityState.GetPerformHex
				: TargetHex;

            await PerformRefraction(abilityState);
			
            AOEPrompt.Answer aoeAnswer =
                await PromptManager.Prompt(
                    new AOEPrompt(abilityState.Performer, abilityState.AbilityAOEPattern, aoeOrigin, null, () => "Select where to target",
                        abilityState.AbilityRange),
                    abilityState.Authority);

            if(aoeAnswer.Skipped)
            {
                return;
            }

            aoeHexes = aoeAnswer.AOEHexes;
			

			abilityState.TargetedAOEHexes = aoeHexes;
		}

        int targetsOutOfAOE = 0;

        //may need to move this around
        Action<List<Figure>> getValidTargets = figures => GetValidTargets(abilityState, figures, targetsOutOfAOE);

        while(true)
		{
			if(abilityState.Blocked || performer.IsDead)
			{
				break;
			}

            abilityState.SetPerformHex(_startingPerformHex);
            await PerformRefraction(abilityState, healIndex);
			healIndex++;

			InitAbilityStateForSingleTarget(abilityState);

			EffectCollection duringTargetedAbilityEffectCollection = CreateDuringTargetedAbilityEffectCollection(abilityState);

			Figure target = null;

            bool autoSelectIfOne =
                Mandatory ||
                abilityState.AbilityTarget == Target.Self ||
                (TargetHex != null && abilityState.AbilityAOEPattern == null);
            target = await AbilityCmd.SelectFigure(abilityState, getValidTargets, mandatory: Mandatory,
                autoSelectIfOne: autoSelectIfOne, autoSkipIfNone: false,
                duringTargetedAbilityEffectCollection,
                () => _getTargetingHintText(abilityState));

			if(target == null)
			{
				break;
			}

			abilityState.AddSingleTargetState(target);
			abilityState.UniqueTargetedFigures.AddIfNew(target);
			abilityState.TargetedHexes.AddIfNew(target.Hex);
			if(!abilityState.GetRedAOEHexes().Contains(target.Hex))
			{
				targetsOutOfAOE++;
			}

			abilityState.SetPerformed();

			await AfterTargetConfirmedBeforeConditionsApplied(abilityState, target);

			await ApplyConditions(abilityState, target, abilityState.SingleTargetConditionModels);

			await AfterConditionsApplied(abilityState, target);

			// Pull
			if(!performer.IsDestroyed && !target.IsDestroyed && abilityState.SingleTargetPull > 0)
			{
				await ForcedMovement(abilityState, abilityState.GetPerformHex, target, abilityState.SingleTargetPull, ForcedMovementType.Pull,
					() => $"Select a path to {Icons.HintText(Icons.Pull)}{abilityState.SingleTargetPull} target");
			}

			// Push
			if(!performer.IsDestroyed && !target.IsDestroyed && abilityState.SingleTargetPush > 0)
			{
				await ForcedMovement(abilityState, abilityState.GetPerformHex, target, abilityState.SingleTargetPush, ForcedMovementType.Push,
					() => $"Select a path to {Icons.HintText(Icons.Push)}{abilityState.SingleTargetPush} target");
			}

			// Swing
			if(!performer.IsDestroyed && !target.IsDestroyed && abilityState.SingleTargetSwing > 0)
			{
				await ForcedMovement(abilityState, abilityState.GetPerformHex, target, abilityState.SingleTargetSwing, ForcedMovementType.Swing,
					() => $"Select a path to {Icons.HintText(Icons.Swing)}{abilityState.SingleTargetSwing} target");
			}

			await AfterEffects(abilityState, target);

			if(performer.IsDestroyed)
			{
				break;
			}

			if(abilityState.AbilityAOEPattern != null)
			{
				if(abilityState.TargetedHexes.Count == abilityState.AbilityAOEPattern.LocalHexes.Count &&
				   targetsOutOfAOE == abilityState.AbilityTargets - 1)
				{
					break;
				}
			}
			else if(abilityState.SingleTargetStates.Count == abilityState.AbilityTargets)
			{
				break;
			}

            abilityState.SetPerformHex(_startingPerformHex);
		}
	}

	protected override void GetValidTargets(State abilityState, List<Figure> figures, int targetsOutOfAOE)
	{
		base.GetValidTargets(abilityState, figures, targetsOutOfAOE);

        if (abilityState.AbilityPerformHex != null && abilityState.AbilityPerformHex != abilityState.Performer.Hex)
        {
            Hex origin = abilityState.AbilityPerformHex;

            for(int i = figures.Count - 1; i >= 0; i--)
            {
                if (!IsStraightLine(origin, figures[i].Hex))
                {
                    figures.RemoveAt(i);
                }
            }
        }
	}

    protected override async GDTask AfterTargetConfirmedBeforeConditionsApplied(State abilityState, Figure target)
    {
        Hex saved = abilityState.AbilityPerformHex;
        abilityState.SetPerformHex(_startingPerformHex);

        await base.AfterTargetConfirmedBeforeConditionsApplied(abilityState, target);

        abilityState.SetPerformHex(saved);
    }

    protected async GDTask PerformRefraction(State abilityState, int index = 0)
    {
        List<Prism> usedPrisms = new List<Prism>();
        Hex origin = abilityState.GetPerformHex;
        int range = abilityState.AbilityRange;

	    while (true)
	    {
            //usedPrisms.Clear();
	        var targetableHexes = RangeHelper.GetHexesInRange(origin, range, false, true)
	            .Where(h =>
	                h.GetHexObjectsOfType<Prism>().Any(p => !usedPrisms.Contains(p)) &&
	                (usedPrisms.Count == 0 || IsStraightLine(origin, h)))
	            .ToList();

	        if (!targetableHexes.Any())
	        {
	            break;
	        }

	        Hex selectedHex = await AbilityCmd.SelectHex(abilityState, list =>
	        {
	            foreach (Hex hex in targetableHexes)
	            {
	                list.Add(hex);
	            }
	        }, false, "Select a prism to target:");

	        if (selectedHex == null)
	        {
	            break;
	        }

	        if (selectedHex.HasHexObjectOfType<Prism>())
	        {
	            usedPrisms.Add(selectedHex.GetHexObjectOfType<Prism>());
	            origin = selectedHex;
	            continue;
	        }

	        break;
	    }

        if (usedPrisms.Any())
        {
            abilityState.SetPerformHex(usedPrisms.Last().Hex);

            RefractionState.SetDidRefract(abilityState, true, index);
            RefractionState.SetRefractCount(abilityState, usedPrisms.Count, index);
			RefractionState.SetUsedPrisms(abilityState, usedPrisms, index);
        }
    }

    public bool IsStraightLine(Hex from, Hex to)
	{
		if (from == to)
			return true;

		int dq = to.Coords.X - from.Coords.X;
		int dr = to.Coords.Y - from.Coords.Y;

		int ds = -dq - dr;
		return dq == 0 || dr == 0 || ds == 0;
	}
}