using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Fractural.Tasks;
using Godot;

public partial class Refractor : Character
{
	private RefractorModel _RefractorModel;	

	public override async GDTask OnScenarioSetupCompleted()
	{
		await base.OnScenarioSetupCompleted();

		object subscriber = new object();
	}

	public override async GDTask Spawn(SavedCharacter savedCharacter, int index)
	{
		await base.Spawn(savedCharacter, index);

		_RefractorModel = (RefractorModel)savedCharacter.ClassModel;
	}

	public static AbilityCardAbility RefractingAttack(int damage, int range = 2, Action<AttackAbility.AttackBuilder> configure = null)
	{
		return new AbilityCardAbility(OtherAbility.Builder()
        .WithPerformAbility(async state =>
        {
            Hex selected = await AbilityCmd.SelectHex(state, list =>
            {
                foreach (Figure f in RangeHelper.GetFiguresInRange(state.Performer.Hex, range, false)
                    .Where(f => f.EnemiesWith(state.Performer)))
                    list.Add(f.Hex);

                foreach (var obstacle in RangeHelper.GetOverlayTilesInRange<Obstacle>(state.Performer.Hex, range, true, true))
					if (obstacle.Hex.Creator == state.Performer)
						list.Add(obstacle.Hex);
            }, true, "Select target or obstacle");

            if (selected.HasHexObjectOfType<Obstacle>())
            {
                // await DoRefract(state, selected, damage);
				var attack = AttackAbility.Builder()
					.WithDamage(damage)
					.WithAOEPattern(new AOEPattern([
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
					]))
					.WithCustomGetPerformHex(hex => selected);

					state.SetCustomValue(attack, "Refracted", true);

					configure?.Invoke(attack);

					await attack.Build().Perform(state.ActionState);
            }
            else
            {
                Figure target = selected.GetFigures()
                    .FirstOrDefault(f => f.EnemiesWith(state.Performer));

                if (target != null)
                {
                    var attack = AttackAbility.Builder()
                        .WithDamage(damage)
                        .WithRange(range)
						.WithCustomGetTargets((state, list) => list.Add(target));

                    configure?.Invoke(attack);       // conditions, pierce, etc.

                    await attack.Build().Perform(state.ActionState);
                }
            }

            state.SetPerformed();
        })
        .Build());
	}
}