// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;
using static UnityEngine.XR.OpenXR.Features.OpenXRFeature;

namespace Microsoft.MixedReality.OpenXR
{
    internal class PlayModeRemotingValidator
    {
        internal const string RemotingNotConfigured = "Using Holographic Remoting requires the Remote Host Name in settings " +
            "to match the IP address displayed in the Holographic Remoting Player running on your HoloLens 2 device.";

        internal static readonly string NotProperlySetup = "Using Holographic Remoting requires the OpenXR loader and the following HoloLens features " +
            "to be enabled in the `Standalone settings` tab, because the Unity editor runs as a standalone application. " +
            "\n  - Eye Gaze Interaction Profile" +
            $"\n  - {HandTrackingFeaturePlugin.featureName}" +
            $"\n  - {MixedRealityFeaturePlugin.featureName}" +
            "\n  - Microsoft Hand Interaction Profile";

        internal const string PlayModeRemotingMenuPath = "Mixed Reality/Remoting/Holographic Remoting for Play Mode";
        internal const string PlayModeRemotingMenuPath2 = "Window/XR/Holographic Remoting for Play Mode";

        internal const string CannotAutoConfigureRemoting = "Could not automatically apply recommended settings to enable Holographic Remoting for Play Mode. " +
            "Please see https://aka.ms/openxr-unity-editor-remoting for manual set up instructions";

        internal static void GetValidationChecks(OpenXRFeature feature, List<ValidationRule> results, BuildTargetGroup targetGroup)
        {
            results.Add(new ValidationRule(feature)
            {
                message = NotProperlySetup,
                error = true,
                checkPredicate = () =>
                {
                    return IsProperlySetup();
                },
                fixIt = () =>
                {
                    SetupProperly();
                }
            });

            results.Add(new ValidationRule(feature)
            {
                message = RemotingNotConfigured,
                error = true,
                fixItAutomatic = false,
                helpText = "To open this feature's settings, click the \"Edit\" button here or click the settings icon to the right of the \"Holographic Remoting for Play Mode\" feature in the XR Plug-in Management settings.",
                checkPredicate = () =>
                {
                    FeatureHelpers.RefreshFeatures(BuildTargetGroup.Standalone);
                    Remoting.PlayModeRemotingPlugin remotingFeature = OpenXRSettings.Instance.GetFeature<Remoting.PlayModeRemotingPlugin>();
                    return remotingFeature != null && remotingFeature.HasValidSettings();
                },
                fixIt = () =>
                {
                    EditorApplication.ExecuteMenuItem(PlayModeRemotingMenuPath);
                }
            });
        }

        internal static bool IsProperlySetup()
        {
            return IsLoaderAssigned() && AreDependenciesEnabled();
        }

        private static bool IsLoaderAssigned()
        {
            XRGeneralSettings standaloneGeneralSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);
            return standaloneGeneralSettings != null &&
                standaloneGeneralSettings.AssignedSettings != null &&
                standaloneGeneralSettings.AssignedSettings.activeLoaders.Any(l => l.GetType().Equals(typeof(OpenXRLoader)));
        }

        private static bool AreDependenciesEnabled()
        {
            FeatureHelpers.RefreshFeatures(BuildTargetGroup.Standalone);
            OpenXRSettings openxrSettings = OpenXRSettings.Instance;
            return openxrSettings != null &&
                IsFeatureEnabled<MixedRealityFeaturePlugin>(openxrSettings) &&
                IsFeatureEnabled<HandTrackingFeaturePlugin>(openxrSettings) &&
                IsFeatureEnabled<EyeGazeInteraction>(openxrSettings) &&
                IsFeatureEnabled<MicrosoftHandInteraction>(openxrSettings);
        }

        internal static void SetupProperly()
        {
            if (!IsLoaderAssigned())
            {
                XRGeneralSettings standaloneGeneralSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);
                if (standaloneGeneralSettings == null ||
                    standaloneGeneralSettings.AssignedSettings == null ||
                    !XRPackageMetadataStore.AssignLoader(standaloneGeneralSettings.AssignedSettings, typeof(OpenXRLoader).Name, BuildTargetGroup.Standalone))
                {
                    Debug.LogError(CannotAutoConfigureRemoting);
                    return;
                }
            }
            if (!AreDependenciesEnabled())
            {
                EnableDependencies();
            }
        }

        private static void EnableDependencies()
        {
            FeatureHelpers.RefreshFeatures(BuildTargetGroup.Standalone);
            OpenXRSettings openxrSettings = OpenXRSettings.Instance;
            if (openxrSettings != null)
            {
                EnableFeature<MixedRealityFeaturePlugin>(openxrSettings);
                EnableFeature<HandTrackingFeaturePlugin>(openxrSettings);
                EnableFeature<EyeGazeInteraction>(openxrSettings);
                EnableFeature<MicrosoftHandInteraction>(openxrSettings);
            }
            else
            {
                Debug.LogError(CannotAutoConfigureRemoting);
            }
        }

        private static bool IsFeatureEnabled<T>(OpenXRSettings openxrSettings) where T : OpenXRFeature
        {
            var feature = openxrSettings.GetFeature<T>();
            return feature != null && feature.enabled;
        }

        private static void EnableFeature<T>(OpenXRSettings openxrSettings) where T : OpenXRFeature
        {
            var feature = openxrSettings.GetFeature<T>();
            if (feature != null)
            {
                feature.enabled = true;
            }
        }
    }
}
#endif
