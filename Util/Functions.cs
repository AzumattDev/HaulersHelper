using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine;

namespace HaulersHelper.Util;

public static class Functions
{
    internal static void FixDuhWagons()
    {
        foreach (Vagon vagon in Resources.FindObjectsOfTypeAll<Vagon>())
        {
            string prefabName = Utils.GetPrefabName(vagon.transform.root.gameObject);

            if (!HaulersHelperPlugin.Wagons.Contains(prefabName)) continue;
            vagon.m_detachDistance = HaulersHelperPlugin.WagonDetachDistanceTweakConfigs[prefabName].Value;
            vagon.m_attachOffset = HaulersHelperPlugin.WagonAttachOffsetTweakConfigs[prefabName].Value;
            vagon.m_lineAttachOffset = HaulersHelperPlugin.WagonLineAttachOffsetTweakConfigs[prefabName].Value;
            vagon.m_breakForce = HaulersHelperPlugin.WagonBreakForceTweakConfigs[prefabName].Value;
            vagon.m_spring = HaulersHelperPlugin.WagonSpringTweakConfigs[prefabName].Value;
            vagon.m_springDamping = HaulersHelperPlugin.WagonSpringDampingTweakConfigs[prefabName].Value;
            vagon.m_baseMass = HaulersHelperPlugin.WagonBaseMassTweakConfigs[prefabName].Value;
            vagon.m_itemWeightMassFactor = HaulersHelperPlugin.WagonItemWeightMassFactorTweakConfigs[prefabName].Value;
            vagon.m_minPitch = HaulersHelperPlugin.WagonMinPitchTweakConfigs[prefabName].Value;
            vagon.m_maxPitch = HaulersHelperPlugin.WagonMaxPitchTweakConfigs[prefabName].Value;
            vagon.m_maxPitchVel = HaulersHelperPlugin.WagonMaxPitchVelTweakConfigs[prefabName].Value;
            vagon.m_maxVol = HaulersHelperPlugin.WagonMaxVolTweakConfigs[prefabName].Value;
            vagon.m_maxVolVel = HaulersHelperPlugin.WagonMaxVolVelTweakConfigs[prefabName].Value;
            vagon.m_audioChangeSpeed = HaulersHelperPlugin.WagonAudioChangeSpeedTweakConfigs[prefabName].Value;
        }
    }
    private static void OnWagonsChanged(object o, EventArgs e) => FixDuhWagons();
    internal static void GetThemWagons(IEnumerable<GameObject> prefabs)
    {
        foreach (Vagon vagon in prefabs.Select(p => p.GetComponentInChildren<Vagon>()).Where(s => s != null))
        {
            int order = 0;

            string prefabName = Utils.GetPrefabName(vagon.transform.root.gameObject);

            if (HaulersHelperPlugin.Wagons.Contains(prefabName))
            {
                continue;
            }
            
            int i = HaulersHelperPlugin.Wagons.Count + 2;
            HaulersHelperPlugin.ConfigurationManagerAttributes attributes = new() { Category = $"{i} - {prefabName}", Order = --order };
            var groupName = HaulersHelperPlugin.CleaningRegex.Replace(prefabName, "");
			
            HaulersHelperPlugin.WagonDetachDistanceTweakConfigs[prefabName] = HaulersHelperPlugin.ModContext.config($"{i} - {groupName}", "Attach & Detach Distance", vagon.m_detachDistance, new ConfigDescription($"The distance from {prefabName} that you can attach yourself to it.", null, attributes));
            HaulersHelperPlugin.WagonAttachOffsetTweakConfigs[prefabName] = HaulersHelperPlugin.ModContext.config($"{i} - {groupName}", "Attachment Offset", vagon.m_attachOffset, new ConfigDescription($"The offset of the attachment point for {prefabName}.", null, attributes));
            HaulersHelperPlugin.WagonLineAttachOffsetTweakConfigs[prefabName] = HaulersHelperPlugin.ModContext.config($"{i} - {groupName}", "Line Attachment Offset", vagon.m_lineAttachOffset, new ConfigDescription($"The offset of the rope attachment point to the connected body for {prefabName}.", null, attributes));
            HaulersHelperPlugin.WagonBreakForceTweakConfigs[prefabName] = HaulersHelperPlugin.ModContext.config($"{i} - {groupName}", "Break Force", vagon.m_breakForce, new ConfigDescription($"The amount of force it takes to break the connection to the attachment point for {prefabName}. This means, the force needing to be applied before you break your attachment to the cart/wagon.", null, attributes));
            HaulersHelperPlugin.WagonSpringTweakConfigs[prefabName] = HaulersHelperPlugin.ModContext.config($"{i} - {groupName}", "Spring", vagon.m_spring, new ConfigDescription($"Represents the stiffness of the joint attachment for {prefabName}. Determines how much the joint will resist being compressed or stretched.", null, attributes));
            HaulersHelperPlugin.WagonSpringDampingTweakConfigs[prefabName] = HaulersHelperPlugin.ModContext.config($"{i} - {groupName}", "Spring Damping", vagon.m_springDamping, new ConfigDescription($"Used to restrict the distance that the joint can move in a certain direction for {prefabName}.", null, attributes));
            HaulersHelperPlugin.WagonBaseMassTweakConfigs[prefabName] = HaulersHelperPlugin.ModContext.config($"{i} - {groupName}", "Base Mass", vagon.m_baseMass, new ConfigDescription($"The mass of {prefabName} that is calculated as the base. This will allow you to make the cart heavier by default before any weight from the items it stores is calculated.", null, attributes));
            HaulersHelperPlugin.WagonItemWeightMassFactorTweakConfigs[prefabName] = HaulersHelperPlugin.ModContext.config($"{i} - {groupName}", "Item Weight Mass Factor", vagon.m_itemWeightMassFactor, new ConfigDescription($"The item weight factor for {prefabName}. This value is multiplied by the weight of the items in {prefabName} and added to the base mass. Higher values make it heavier and harder to pull.", null, attributes));
            HaulersHelperPlugin.WagonMinPitchTweakConfigs[prefabName] = HaulersHelperPlugin.ModContext.config($"{i} - {groupName}", "Min Pitch", vagon.m_minPitch, new ConfigDescription($"The minimum pitch of {prefabName}'s wheel audio.", null, attributes));
            HaulersHelperPlugin.WagonMaxPitchTweakConfigs[prefabName] = HaulersHelperPlugin.ModContext.config($"{i} - {groupName}", "Max Pitch", vagon.m_maxPitch, new ConfigDescription($"The maximum pitch of {prefabName}'s wheel audio.", null, attributes));
            HaulersHelperPlugin.WagonMaxPitchVelTweakConfigs[prefabName] = HaulersHelperPlugin.ModContext.config($"{i} - {groupName}", "Max Pitch Velocity", vagon.m_maxPitchVel, new ConfigDescription($"The maximum velocity of {prefabName}'s wheel audio. This will be the pitch control based on the velocity of the wheel.", null, attributes));
            HaulersHelperPlugin.WagonMaxVolTweakConfigs[prefabName] = HaulersHelperPlugin.ModContext.config($"{i} - {groupName}", "Max Volume", vagon.m_maxVol, new ConfigDescription($"The maximum volume of {prefabName}'s wheel audio.", null, attributes));
            HaulersHelperPlugin.WagonMaxVolVelTweakConfigs[prefabName] = HaulersHelperPlugin.ModContext.config($"{i} - {groupName}", "Max Volume Velocity", vagon.m_maxVolVel, new ConfigDescription($"The maximum velocity at which {prefabName}'s wheels can play audio at the maximum volume.", null, attributes));
            HaulersHelperPlugin.WagonAudioChangeSpeedTweakConfigs[prefabName] = HaulersHelperPlugin.ModContext.config($"{i} - {groupName}", "Audio Change Speed", vagon.m_audioChangeSpeed, new ConfigDescription($"How quickly the audio volume and pitch can change for {prefabName}.", null, attributes));
 

            HaulersHelperPlugin.WagonDetachDistanceTweakConfigs[prefabName].SettingChanged += OnWagonsChanged;
            HaulersHelperPlugin.WagonAttachOffsetTweakConfigs[prefabName].SettingChanged += OnWagonsChanged;
            HaulersHelperPlugin.WagonLineAttachOffsetTweakConfigs[prefabName].SettingChanged += OnWagonsChanged;
            HaulersHelperPlugin.WagonBreakForceTweakConfigs[prefabName].SettingChanged += OnWagonsChanged;
            HaulersHelperPlugin.WagonSpringTweakConfigs[prefabName].SettingChanged += OnWagonsChanged;
            HaulersHelperPlugin.WagonSpringDampingTweakConfigs[prefabName].SettingChanged += OnWagonsChanged;
            HaulersHelperPlugin.WagonBaseMassTweakConfigs[prefabName].SettingChanged += OnWagonsChanged;
            HaulersHelperPlugin.WagonItemWeightMassFactorTweakConfigs[prefabName].SettingChanged += OnWagonsChanged;
            HaulersHelperPlugin.WagonMinPitchTweakConfigs[prefabName].SettingChanged += OnWagonsChanged;
            HaulersHelperPlugin.WagonMaxPitchTweakConfigs[prefabName].SettingChanged += OnWagonsChanged;
            HaulersHelperPlugin.WagonMaxPitchVelTweakConfigs[prefabName].SettingChanged += OnWagonsChanged;
            HaulersHelperPlugin.WagonMaxVolTweakConfigs[prefabName].SettingChanged += OnWagonsChanged;
            HaulersHelperPlugin.WagonMaxVolVelTweakConfigs[prefabName].SettingChanged += OnWagonsChanged;
            HaulersHelperPlugin.WagonAudioChangeSpeedTweakConfigs[prefabName].SettingChanged += OnWagonsChanged;
            
            HaulersHelperPlugin.Wagons.Add(prefabName);
        }
    }
}