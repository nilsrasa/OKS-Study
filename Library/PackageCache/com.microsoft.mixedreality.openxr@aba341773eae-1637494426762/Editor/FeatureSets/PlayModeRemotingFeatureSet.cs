// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.OpenXR.Remoting;
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;

namespace Microsoft.MixedReality.OpenXR.Editor
{
#if UNITY_OPENXR_1_2_OR_NEWER
    [OpenXRFeatureSet(
        FeatureSetId = featureSetId,
        FeatureIds = new string[]
        {
            PlayModeRemotingPlugin.featureId,
            MixedRealityFeaturePlugin.featureId,
            HandTrackingFeaturePlugin.featureId,
        },
        RequiredFeatureIds = new string[]
        {
            PlayModeRemotingPlugin.featureId,
            MixedRealityFeaturePlugin.featureId,
        },
        DefaultFeatureIds = new string[]
        {
            PlayModeRemotingPlugin.featureId,
            MixedRealityFeaturePlugin.featureId,
            HandTrackingFeaturePlugin.featureId,
        },
        UiName = "Holographic Remoting for Play Mode",
        // This will appear as a tooltip for the (?) icon in the loader UI.
        Description = "Enable the Holographic Remoting for Play Mode features.",
        SupportedBuildTargets = new BuildTargetGroup[] { BuildTargetGroup.Standalone }
    )]
    sealed class PlayModeRemotingFeatureSet
    {
        internal const string featureSetId = "com.microsoft.openxr.featureset.playmoderemoting";
    }
#endif
}
