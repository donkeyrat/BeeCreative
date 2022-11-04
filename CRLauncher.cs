using BepInEx;
using UnityEngine;

namespace Creative
{
	[BepInPlugin("teamgrad.beecreative", "BeeCreative", "1.1.0")]
	public class CRLauncher : BaseUnityPlugin
	{
		public CRLauncher()
		{
			CRBinder.UnitGlad();
		}
	}
}
