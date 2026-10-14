using System.Collections.Generic;
using Fractural.Tasks;

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
}