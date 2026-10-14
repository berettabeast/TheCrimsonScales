using System.Collections.Generic;
using Godot;

public class RefractorModel : ClassModel
{
	public override string Name => "Refractor";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Low;
	public override int HandSize => 10;
	public override Ancestry Ancestry => Ancestry.Orchid;

	public override List<EventModel> UnlockEvents { get; } =
	[
		ModelDB.Event<City41>(),
		ModelDB.Event<Road41>(),
	];

	public override List<EventModel> RetirementEvents { get; } =
	[
		ModelDB.Event<City42>(),
		ModelDB.Event<Road42>(),
	];

	public override string AssetPath => "res://Content/Classes/Refractor";
	public override Color PrimaryColor => Color.FromHtml("b9a983");
	public override Color SecondaryColor => Color.FromHtml("82f7c3");
	public override bool HasAnimatedSprite => false;

	public override PackedScene Scene => SceneLoader.LoadPackedScene($"{AssetPath}/Refractor.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<CrystalShrapnel>(),
		ModelDB.AbilityCard<CalculatedTrajectory>(),
		ModelDB.AbilityCard<SunBurn>(),
		ModelDB.AbilityCard<BlindingSpeed>(),
		ModelDB.AbilityCard<StrongFoundation>(),
		ModelDB.AbilityCard<PrismaticLance>(),
		ModelDB.AbilityCard<CrystallineGrowth>(),
		ModelDB.AbilityCard<FocusedRay>(),
		ModelDB.AbilityCard<RestorativeLight>(),
		ModelDB.AbilityCard<NaturalGrowth>(),
		ModelDB.AbilityCard<Shiny>(),
		ModelDB.AbilityCard<CrystalJump>(),
		ModelDB.AbilityCard<SunRays>(),
	];

	public override List<PerkModel> Perks { get; } =
	[
		ModelDB.Perk<RefractorPerks.ReplaceOneMinusOneWithOnePlusOneIfLightIsStrongOrWaning>(),
		ModelDB.Perk<RefractorPerks.ReplaceOneMinusOneWithOnePlusOneIfLightIsStrongOrWaning>(),

		ModelDB.Perk<RefractorPerks.ReplaceOneMinusTwoWithOnePlusZero>(),

		ModelDB.Perk<RefractorPerks.ReplaceOnePlusZeroWithOnePlusTwo>(),

		ModelDB.Perk<RefractorPerks.AddThreePlusOneIfLightIsStrongOrWaning>(),
		ModelDB.Perk<RefractorPerks.AddThreePlusOneIfLightIsStrongOrWaning>(),

		ModelDB.Perk<RefractorPerks.AddTwoInfuseLightRolling>(),

		ModelDB.Perk<RefractorPerks.AddThreeInfuseEarthIfLightIsStrongOrWaningRolling>(),

		ModelDB.Perk<RefractorPerks.AddOneCreateHazardousTerrain>(),
		ModelDB.Perk<RefractorPerks.AddOneCreateHazardousTerrain>(),

		ModelDB.Perk<RefractorPerks.AddOneRetaliateThreeOnHazardousTerrainRolling>(),
		ModelDB.Perk<RefractorPerks.AddOneRetaliateThreeOnHazardousTerrainRolling>(),

		ModelDB.Perk<RefractorPerks.AddOneShieldThreeOnHazardousTerrainRolling>(),
		ModelDB.Perk<RefractorPerks.AddOneShieldThreeOnHazardousTerrainRolling>(),

		ModelDB.Perk<RefractorPerks.IgnoreItemEffectsAndAddOnePlusOneIfLightIsStrongOrWaningRolling>(),

		ModelDB.Perk<RefractorPerks.BrambleBulwark>(),
		ModelDB.Perk<RefractorPerks.RiseAndShine>(),
	];
}