using Landfall.TABS;
using UnityEngine;
using Landfall.TABS.UnitEditor;
using Landfall.TABS.Workshop;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Creative
{
	public class CRMain
    {
        public CRMain()
        {
            blueprints = creative.LoadAllAssets<UnitBlueprint>();
            var db = LandfallUnitDatabase.GetDatabase();
            if (blueprints != null)
            {
                foreach (var unit in blueprints)
                {
                    if (unit != null && unit.UnitBase != null)
                    {
                        var find = db.UnitBaseList.ToList().Find(x => x.name == unit.UnitBase.name);
                        if (find != null)
                        {
                            Debug.Log(find.name);
                            unit.UnitBase = find;
                        }
                        if (unit.Entity.SpriteIcon != null)
                        {
                            var icon = Resources.FindObjectsOfTypeAll<Sprite>().ToList().Find(x => x.name == unit.Entity.SpriteIcon.name);
                            if (icon != null)
                            {
                                unit.Entity.SpriteIcon = icon;
                            }
                        }
                    }
                }

            }
            creative.LoadAsset<UnitBlueprint>("CustomHorse").UnitBase = ((List<GameObject>)typeof(LandfallUnitDatabase).GetField("UnitBases", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db)).Find(x => x.name == "Horse_1 Prefabs_VB");
            foreach (var objecting in creative.LoadAllAssets<GameObject>())
            {
                if (objecting != null)
                {
                    if (objecting.GetComponent<Unit>())
                    {
                        List<GameObject> stuff = (List<GameObject>)typeof(LandfallUnitDatabase).GetField("UnitBases", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
                        stuff.Add(objecting);
                        typeof(LandfallUnitDatabase).GetField("UnitBases", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, stuff);
                    }
                    else if (objecting.GetComponent<WeaponItem>())
                    {
                        List<GameObject> stuff = (List<GameObject>)typeof(LandfallUnitDatabase).GetField("Weapons", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
                        stuff.Add(objecting);
                        typeof(LandfallUnitDatabase).GetField("Weapons", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, stuff);
                    }
                    else if (objecting.GetComponent<ProjectileEntity>())
                    {
                        List<GameObject> stuff = (List<GameObject>)typeof(LandfallUnitDatabase).GetField("Projectiles", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
                        stuff.Add(objecting);
                        typeof(LandfallUnitDatabase).GetField("Projectiles", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, stuff);
                    }
                    else if (objecting.GetComponent<SpecialAbility>())
                    {
                        List<GameObject> stuff = (List<GameObject>)typeof(LandfallUnitDatabase).GetField("CombatMoves", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
                        stuff.Add(objecting);
                        typeof(LandfallUnitDatabase).GetField("CombatMoves", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, stuff);
                    }
                    else if (objecting.GetComponent<PropItem>())
                    {
                        List<GameObject> stuff = (List<GameObject>)typeof(LandfallUnitDatabase).GetField("CharacterProps", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
                        stuff.Add(objecting);
                        typeof(LandfallUnitDatabase).GetField("CharacterProps", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, stuff);
                    }
                }
            }
            new GameObject()
            {
                name = "Bullshit",
                hideFlags = HideFlags.HideAndDontSave
            }.AddComponent<UnitCreatorHijacker>();
            foreach (var mat in creative.LoadAllAssets<Material>())
            {
                if (mat.shader.name == "Standard")
                {
                    mat.shader = Shader.Find("Standard");
                }
            }
            ServiceLocator.GetService<CustomContentLoaderModIO>().QuickRefresh(WorkshopContentType.Unit, null);
        }

        public static AssetBundle creative = AssetBundle.LoadFromMemory(BeeCreative.Properties.Resources.creative);

        public static Material wet;

        public static UnitBlueprint[] blueprints;
    }
}
