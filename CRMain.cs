﻿using Landfall.TABS;
using UnityEngine;
using Landfall.TABS.UnitEditor;
using Landfall.TABS.Workshop;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using DM;

namespace Creative
{
	public class CRMain
    {
        public CRMain()
        {
            var db = ContentDatabase.Instance().LandfallContentDatabase;
            
            new GameObject()
            {
                name = "Bullshit",
                hideFlags = HideFlags.HideAndDontSave
            }.AddComponent<UnitCreatorHijacker>();
            
            foreach (var mat in creative.LoadAllAssets<Material>()) if (Shader.Find(mat.shader.name)) mat.shader = Shader.Find(mat.shader.name);
            
            blueprints = creative.LoadAllAssets<UnitBlueprint>();
            if (blueprints != null)
            {
                foreach (var unit in blueprints)
                {
                    if (unit != null && unit.UnitBase != null)
                    {
                        var find = db.GetUnitBases().ToList().Find(x => x.name == unit.UnitBase.name);
                        if (find)
                        {
                            Debug.Log(find.name);
                            unit.UnitBase = find;
                        }
                        if (unit.Entity.GetSpriteIconPath() != null)
                        {
                            unit.Entity.GetSpriteIconAsync(
                                sprite =>
                                {
                                    var icon = Resources.FindObjectsOfTypeAll<Sprite>().ToList().Find(x => x.name == sprite.name);
                                    if (icon) unit.Entity.SetSpriteIcon(icon);
                                });
                        }
                    }
                }
            }
            
            creative.LoadAsset<UnitBlueprint>("CustomHorse").UnitBase = db.GetUnitBases().ToList().Find(x => x.name == "Horse_1 Prefabs_VB");
            
            foreach (var objecting in creative.LoadAllAssets<GameObject>()) 
            {
                if (objecting != null) {

                    if (objecting.GetComponent<Unit>()) newBases.Add(objecting);
                    else if (objecting.GetComponent<WeaponItem>()) {
                        newWeapons.Add(objecting);
                        int totalSubmeshes = 0;
                        foreach (var rend in objecting.GetComponentsInChildren<MeshFilter>()) {
                            if (rend.gameObject.activeSelf && rend.gameObject.activeInHierarchy && rend.mesh.subMeshCount > 0 && rend.GetComponent<MeshRenderer>() && rend.GetComponent<MeshRenderer>().enabled == true) {

                                totalSubmeshes += rend.mesh.subMeshCount;
                            }
                        }
                        foreach (var rend in objecting.GetComponentsInChildren<SkinnedMeshRenderer>()) {
                            if (rend.gameObject.activeSelf && rend.sharedMesh.subMeshCount > 0 && rend.enabled) {

                                totalSubmeshes += rend.sharedMesh.subMeshCount;
                            }
                        }
                        if (totalSubmeshes != 0) {
                            float average = 1f / totalSubmeshes;
                            var averageList = new List<float>();
                            for (int i = 0; i < totalSubmeshes; i++) { averageList.Add(average); }
                            objecting.GetComponent<WeaponItem>().SubmeshArea = null;
                            objecting.GetComponent<WeaponItem>().SubmeshArea = averageList.ToArray();
                        }
                    }
                    else if (objecting.GetComponent<ProjectileEntity>()) newProjectiles.Add(objecting);
                    else if (objecting.GetComponent<SpecialAbility>()) newAbilities.Add(objecting);
                    else if (objecting.GetComponent<PropItem>()) {
                        newProps.Add(objecting);
                        int totalSubmeshes = 0;
                        foreach (var rend in objecting.GetComponentsInChildren<MeshFilter>()) {
                            if (rend.gameObject.activeSelf && rend.gameObject.activeInHierarchy && rend.mesh.subMeshCount > 0 && rend.GetComponent<MeshRenderer>() && rend.GetComponent<MeshRenderer>().enabled == true) {

                                totalSubmeshes += rend.mesh.subMeshCount;
                            }
                        }
                        foreach (var rend in objecting.GetComponentsInChildren<SkinnedMeshRenderer>()) {
                            if (rend.gameObject.activeSelf && rend.sharedMesh.subMeshCount > 0 && rend.enabled) {

                                totalSubmeshes += rend.sharedMesh.subMeshCount;
                            }
                        }
                        if (totalSubmeshes != 0) {
                            float average = 1f / totalSubmeshes;
                            var averageList = new List<float>();
                            for (int i = 0; i < totalSubmeshes; i++) { averageList.Add(average); }
                            objecting.GetComponent<PropItem>().SubmeshArea = null;
                            objecting.GetComponent<PropItem>().SubmeshArea = averageList.ToArray();
                        }
                    }
                }
            }
            
            AddContentToDatabase();
        }
        
        public void AddContentToDatabase()
        {
	        Dictionary<DatabaseID, UnityEngine.Object> nonStreamableAssets = (Dictionary<DatabaseID, UnityEngine.Object>)typeof(AssetLoader).GetField("m_nonStreamableAssets", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ContentDatabase.Instance().AssetLoader);
	        
            var db = ContentDatabase.Instance().LandfallContentDatabase;
            
            Dictionary<DatabaseID, UnitBlueprint> units = (Dictionary<DatabaseID, UnitBlueprint>)typeof(LandfallContentDatabase).GetField("m_unitBlueprints", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var unit in newUnits)
            {
                units.Add(unit.Entity.GUID, unit);
                nonStreamableAssets.Add(unit.Entity.GUID, unit);
            }
            typeof(LandfallContentDatabase).GetField("m_unitBlueprints", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, units);
            
            Dictionary<DatabaseID, Faction> factions = (Dictionary<DatabaseID, Faction>)typeof(LandfallContentDatabase).GetField("m_factions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            List<DatabaseID> defaultHotbarFactions = (List<DatabaseID>)typeof(LandfallContentDatabase).GetField("m_defaultHotbarFactionIds", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var faction in newFactions)
            {
	            if (faction != null)
	            {
		            factions.Add(faction.Entity.GUID, faction);
		            nonStreamableAssets.Add(faction.Entity.GUID, faction);
		            defaultHotbarFactions.Add(faction.Entity.GUID);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_factions", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, factions);
            typeof(LandfallContentDatabase).GetField("m_defaultHotbarFactionIds", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, defaultHotbarFactions.OrderBy(x => factions[x].index).ToList());
            foreach (var fac in ContentDatabase.Instance().GetDefaultHotbarFactions())
            {
	            if (fac != null)
	            {
		            Debug.Log(fac.Entity.Name);
	            }
            }
            
            Dictionary<DatabaseID, TABSCampaignAsset> campaigns = (Dictionary<DatabaseID, TABSCampaignAsset>)typeof(LandfallContentDatabase).GetField("m_campaigns", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var campaign in newCampaigns)
            {
	            if (!campaigns.ContainsKey(campaign.Entity.GUID))
	            {
		            campaigns.Add(campaign.Entity.GUID, campaign);
		            nonStreamableAssets.Add(campaign.Entity.GUID, campaign);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_campaigns", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, campaigns);
            
            Dictionary<DatabaseID, TABSCampaignLevelAsset> campaignLevels = (Dictionary<DatabaseID, TABSCampaignLevelAsset>)typeof(LandfallContentDatabase).GetField("m_campaignLevels", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var campaignLevel in newCampaignLevels)
            {
	            if (!campaignLevels.ContainsKey(campaignLevel.Entity.GUID))
	            {
		            campaignLevels.Add(campaignLevel.Entity.GUID, campaignLevel);
		            nonStreamableAssets.Add(campaignLevel.Entity.GUID, campaignLevel);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_campaignLevels", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, campaignLevels);
            
            Dictionary<DatabaseID, VoiceBundle> voiceBundles = (Dictionary<DatabaseID, VoiceBundle>)typeof(LandfallContentDatabase).GetField("m_voiceBundles", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var voiceBundle in newVoiceBundles)
            {
	            if (!voiceBundles.ContainsKey(voiceBundle.Entity.GUID))
	            {
		            voiceBundles.Add(voiceBundle.Entity.GUID, voiceBundle);
		            nonStreamableAssets.Add(voiceBundle.Entity.GUID, voiceBundle);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_voiceBundles", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, voiceBundles);
            
            List<DatabaseID> factionIcons = (List<DatabaseID>)typeof(LandfallContentDatabase).GetField("m_factionIconIds", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var factionIcon in newFactionIcons)
            {
	            if (!factionIcons.Contains(factionIcon.Entity.GUID))
	            {
		            factionIcons.Add(factionIcon.Entity.GUID);
		            nonStreamableAssets.Add(factionIcon.Entity.GUID, factionIcon);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_factionIconIds", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, factionIcons);
            
            Dictionary<DatabaseID, GameObject> unitBases = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_unitBases", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var unitBase in newBases)
            {
	            if (!unitBases.ContainsKey(unitBase.GetComponent<Unit>().Entity.GUID))
	            {
		            unitBases.Add(unitBase.GetComponent<Unit>().Entity.GUID, unitBase);
		            nonStreamableAssets.Add(unitBase.GetComponent<Unit>().Entity.GUID, unitBase);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_unitBases", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, unitBases);
            
            Dictionary<DatabaseID, GameObject> props = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_characterProps", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var prop in newProps)
            {
	            if (!props.ContainsKey(prop.GetComponent<PropItem>().Entity.GUID))
	            {
		            props.Add(prop.GetComponent<PropItem>().Entity.GUID, prop);
		            nonStreamableAssets.Add(prop.GetComponent<PropItem>().Entity.GUID, prop);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_characterProps", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, props);
            
            Dictionary<DatabaseID, GameObject> abilities = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_combatMoves", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var ability in newAbilities)
            {
	            if (!abilities.ContainsKey(ability.GetComponent<SpecialAbility>().Entity.GUID))
	            {
		            abilities.Add(ability.GetComponent<SpecialAbility>().Entity.GUID, ability);
		            nonStreamableAssets.Add(ability.GetComponent<SpecialAbility>().Entity.GUID, ability);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_combatMoves", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, abilities);
            
            Dictionary<DatabaseID, GameObject> weapons = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_weapons", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var weapon in newWeapons)
            {
	            if (!weapons.ContainsKey(weapon.GetComponent<WeaponItem>().Entity.GUID))
	            {
		            weapons.Add(weapon.GetComponent<WeaponItem>().Entity.GUID, weapon);
		            nonStreamableAssets.Add(weapon.GetComponent<WeaponItem>().Entity.GUID, weapon);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_weapons", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, weapons);
            
            Dictionary<DatabaseID, GameObject> projectiles = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_projectiles", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var proj in newProjectiles)
            {
	            if (!projectiles.ContainsKey(proj.GetComponent<ProjectileEntity>().Entity.GUID))
	            {
		            projectiles.Add(proj.GetComponent<ProjectileEntity>().Entity.GUID, proj);
		            nonStreamableAssets.Add(proj.GetComponent<ProjectileEntity>().Entity.GUID, proj);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_projectiles", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, projectiles);


            
            ServiceLocator.GetService<CustomContentLoaderModIO>().QuickRefresh(WorkshopContentType.Unit, null);
        }
        
        public List<UnitBlueprint> newUnits = new List<UnitBlueprint>();

        public List<Faction> newFactions = new List<Faction>();
        
        public List<TABSCampaignAsset> newCampaigns = new List<TABSCampaignAsset>();
        
        public List<TABSCampaignLevelAsset> newCampaignLevels = new List<TABSCampaignLevelAsset>();
        
        public List<VoiceBundle> newVoiceBundles = new List<VoiceBundle>();
        
        public List<FactionIcon> newFactionIcons = new List<FactionIcon>();
        
        public List<GameObject> newBases = new List<GameObject>();

        public List<GameObject> newProps = new List<GameObject>();
        
        public List<GameObject> newAbilities = new List<GameObject>();

        public List<GameObject> newWeapons = new List<GameObject>();
        
        public List<GameObject> newProjectiles = new List<GameObject>();

        public static AssetBundle creative = AssetBundle.LoadFromMemory(BeeCreative.Properties.Resources.creative);

        public static UnitBlueprint[] blueprints;
    }
}
