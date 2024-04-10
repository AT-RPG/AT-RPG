using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AT_RPG
{
    public partial class SceneManagerSettings
    {
        [Space(20)]
        [Space(10)]
        public readonly string Init = "Init";
        public List<AssetLabelReference> InitAddressableLabelMap = new List<AssetLabelReference>();

        [Space(10)]
        public readonly string IntroScene = "IntroScene";
        public List<AssetLabelReference> IntroSceneAddressableLabelMap = new List<AssetLabelReference>();

        [Space(10)]
        public readonly string LoadingScene = "LoadingScene";
        public List<AssetLabelReference> LoadingSceneAddressableLabelMap = new List<AssetLabelReference>();

        [Space(10)]
        public readonly string TitleScene = "TitleScene";
        public List<AssetLabelReference> TitleSceneAddressableLabelMap = new List<AssetLabelReference>();

        [Space(10)]
        public readonly string MainScene = "MainScene";
        public List<AssetLabelReference> MainSceneAddressableLabelMap = new List<AssetLabelReference>();

        [Space(10)]
        public readonly string MainScene_BJW = "MainScene_BJW";
        public List<AssetLabelReference> MainScene_BJWAddressableLabelMap = new List<AssetLabelReference>();

        [Space(10)]
        public readonly string MainScene_CSH = "MainScene_CSH";
        public List<AssetLabelReference> MainScene_CSHAddressableLabelMap = new List<AssetLabelReference>();

        [Space(10)]
        public readonly string MainScene_IJH = "MainScene_IJH";
        public List<AssetLabelReference> MainScene_IJHAddressableLabelMap = new List<AssetLabelReference>();

        [Space(10)]
        public readonly string MainScene_JUJ = "MainScene_JUJ";
        public List<AssetLabelReference> MainScene_JUJAddressableLabelMap = new List<AssetLabelReference>();

    }
}

