using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    public partial class SceneManagerSettings
    {
        [Space(20)]
        [Space(10)]
        public AssetReferenceScene IntroSceneAsset;
        public List<string> IntroSceneAddressableLabelMap = new List<string>();

        [Space(10)]
        public AssetReferenceScene LoadingSceneAsset;
        public List<string> LoadingSceneAddressableLabelMap = new List<string>();

        [Space(10)]
        public AssetReferenceScene TitleSceneAsset;
        public List<string> TitleSceneAddressableLabelMap = new List<string>();

        [Space(10)]
        public AssetReferenceScene MainSceneAsset;
        public List<string> MainSceneAddressableLabelMap = new List<string>();

        [Space(10)]
        public AssetReferenceScene MainScene_BJWAsset;
        public List<string> MainScene_BJWAddressableLabelMap = new List<string>();

        [Space(10)]
        public AssetReferenceScene MainScene_CSHAsset;
        public List<string> MainScene_CSHAddressableLabelMap = new List<string>();

        [Space(10)]
        public AssetReferenceScene MainScene_IJHAsset;
        public List<string> MainScene_IJHAddressableLabelMap = new List<string>();

        [Space(10)]
        public AssetReferenceScene MainScene_JUJAsset;
        public List<string> MainScene_JUJAddressableLabelMap = new List<string>();

    }
}

