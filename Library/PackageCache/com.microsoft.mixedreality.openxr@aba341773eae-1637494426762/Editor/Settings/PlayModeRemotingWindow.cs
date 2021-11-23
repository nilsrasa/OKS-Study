// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.OpenXR.Remoting;
using System.Reflection;
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.XR.OpenXR;

namespace Microsoft.MixedReality.OpenXR.Editor
{
    /// <summary>
    /// Represents a standalone window for accessing holographic remoting for play mode settings.
    /// </summary>
    internal class PlayModeRemotingWindow : EditorWindow
    {
        private const string ConnectionInfo = "Remoting will automatically connect to the above IP address on play.";

        private static readonly GUIContent FeatureEnabledLabel = EditorGUIUtility.TrTextContent("Disable Holographic Remoting for Play Mode");
        private static readonly GUIContent FeatureDisabledLabel = EditorGUIUtility.TrTextContent("Enable Holographic Remoting for Play Mode");
        private static readonly GUIContent SetupProperlyLabel = EditorGUIUtility.TrTextContent("Setup up for remoting");

        private UnityEditor.Editor PlayModeRemotingPluginEditor;
        private PlayModeRemotingPlugin feature;
        private Vector2 scrollPos;

        /// <summary>
        /// Initializes the Remoting Window class
        /// </summary>
        [MenuItem(PlayModeRemotingValidator.PlayModeRemotingMenuPath)]
        [MenuItem(PlayModeRemotingValidator.PlayModeRemotingMenuPath2)]
        private static void Init()
        {
            GetWindow<PlayModeRemotingWindow>("Holographic Remoting for Play Mode");
        }

        private void OnGUI()
        {
            using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPos))
            {
                scrollPos = scroll.scrollPosition;

                if (feature == null)
                {
                    FeatureHelpers.RefreshFeatures(BuildTargetGroup.Standalone);
                    feature = OpenXRSettings.Instance.GetFeature<PlayModeRemotingPlugin>();
                }

                if (PlayModeRemotingPluginEditor == null)
                {
                    if (feature != null)
                    {
                        PlayModeRemotingPluginEditor = UnityEditor.Editor.CreateEditor(feature);
                    }
                }

                PlayModeRemotingPluginEditor.DrawDefaultInspector();

                if (feature != null)
                {
                    EditorGUILayout.Space();

                    if (feature.enabled)
                    {
                        bool hasValidSettings = feature.HasValidSettings();
                        bool isProperlySetup = PlayModeRemotingValidator.IsProperlySetup();

                        if (hasValidSettings && isProperlySetup)
                        {
                            EditorGUILayout.HelpBox(ConnectionInfo, MessageType.Info);
                        }
                        else
                        {
                            if (!hasValidSettings)
                            {
                                EditorGUILayout.HelpBox(PlayModeRemotingValidator.RemotingNotConfigured, MessageType.Error);
                            }

                            if (!isProperlySetup)
                            {
                                EditorGUILayout.HelpBox(PlayModeRemotingValidator.NotProperlySetup, MessageType.Error);
                                if (GUILayout.Button(SetupProperlyLabel))
                                {
                                    PlayModeRemotingValidator.SetupProperly();
                                }
                            }
                        }
                    }
                }

                EditorGUILayout.Space();
                if (GUILayout.Button(feature.enabled ? FeatureEnabledLabel : FeatureDisabledLabel))
                {
                    var buildTarget = BuildTargetGroup.Standalone;
                    var shouldEnable = !feature.enabled;
                    var featureSet = OpenXRFeatureSetManager.GetFeatureSetWithId(buildTarget, PlayModeRemotingFeatureSet.featureSetId);

                    // Because the feature is required for the feature set, must turn on/off the feature set first before change the feature.
                    featureSet.isEnabled = shouldEnable;
                    AppRemotingValidator.OpenXREditorSettingsInstanceSetFeatureSetSelected(buildTarget, PlayModeRemotingFeatureSet.featureSetId, shouldEnable);
                    OpenXRFeatureSetManager.SetFeaturesFromEnabledFeatureSets(buildTarget);

                    // This change won't affect "OpenXRFeatureEditor" UX until UX is refreshed.
                    feature.enabled = shouldEnable;
                }
            }
        }
    }
}
