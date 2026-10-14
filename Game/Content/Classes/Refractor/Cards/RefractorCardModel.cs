using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public abstract class RefractorLevelUpCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : RefractorCardSide
	where TBottom : RefractorCardSide
{
	protected override string TexturePath => "res://Content/Classes/Refractor/Cards.jpg";
	protected override int ColumnCount => 5;
	protected override int RowCount => 3;
}

public abstract class RefractorCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : RefractorCardSide
	where TBottom : RefractorCardSide
{
	protected override string TexturePath => "res://Content/Classes/Refractor/Cards.jpg";
	protected override int ColumnCount => 5;
	protected override int RowCount => 3;
}

public abstract class RefractorCardSide : AbilityCardSideModel<Refractor>
{
	protected AbilityCardAbility RefractAttack(int damage, int range = 2, 
		Action<AttackAbility.AttackBuilder> configure = null)
	{
		return new AbilityCardAbility(AttackAbility.Builder()
			.WithDamage(damage)
			.WithRange(range)
			.Build());
	}

	private async GDTask PerformRefract(ScenarioEvents.AttackAfterTargetConfirmed.Parameters parameters)
{
    Hex obstacleHex = parameters.AbilityState.Target.Hex;

    // Prompt for a secondary target beyond the obstacle

    

    // Redirect the attack (or apply effects) to the bounce target
    // or apply bonus effects, extra damage, conditions, etc.
}
}