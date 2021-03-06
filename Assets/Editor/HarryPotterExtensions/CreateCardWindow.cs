﻿using System;
using System.IO;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Cards.PlayRequirements;
using HarryPotterUnity.DeckGeneration.Requirements;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Type = HarryPotterUnity.Enums.Type;

namespace HarryPotterExtensions
{
    public class CreateCardWindow : EditorWindow 
    {
        private struct CreateCardRequest
        {
            public string CardName { get; set; }
            public Texture2D CardGraphic { get; set; }

            public Type CardType { get; set; }
            public ClassificationTypes Classification { get; set; }
            public Rarity Rarity { get; set; }
            
            public bool AddLessonRequirement { get; set; }
            public LessonTypes LessonType { get; set; }
            public int LessonAmtRequired { get; set; }
            
            public bool AddCardLimit { get; set; }
            public int MaxAllowedInDeck { get; set; }
            
            public bool AddExistingScript { get; set; }
            public MonoScript AttachedScript { get; set; }

            public bool IsValid
            {
                get
                {
                    bool validScript = false;

                    if (AddExistingScript == false)
                    {
                        validScript = true;
                    }
                    else if (AttachedScript != null)
                    {
                        validScript = AttachedScript.GetClass().IsSubclassOf(typeof(BaseCard));
                    }

                    return CardGraphic != null && !string.IsNullOrEmpty(CardName)
                        && validScript;
                }
            }
        }

        private CreateCardRequest _cardRequest;


        [MenuItem("Harry Potter TCG/Add New Card"), UsedImplicitly]
        public static void AddCard()
        {
            GetWindow(typeof(CreateCardWindow), true, "New Card");
        }
        
        private static void CreateCard(CreateCardRequest request)
        {
            GameObject card = InstantiateCardTemplate();

            try
            {
                card.transform.name = request.CardName;

                var newMaterial = CreateNewMaterial(request);
                card.transform.FindChild("Front").gameObject.GetComponent<Renderer>().material = newMaterial;

                AddChosenComponents(request, card);

                TryCreatePrefab(request, card);

                DestroyImmediate(card);
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("Error: {0}", e.Message));
                Debug.Log("Card was not created");
                Destroy(card);
            }
        }

        private static GameObject InstantiateCardTemplate()
        {
            string templatePath =
                AssetDatabase.GetAllAssetPaths().Single(path => path.Contains("CardTemplate.prefab"));

            var template = AssetDatabase.LoadAssetAtPath(templatePath, typeof (GameObject)) as GameObject;

            return (GameObject) Instantiate(template, Vector3.zero, Quaternion.identity);
        }

        private static Material CreateNewMaterial(CreateCardRequest request)
        {
            string newMaterialAssetPath = string.Format("Assets/Materials/{0}/{1}/{2}Mat.mat",
                request.CardType == Type.Match ? "Matches" : request.CardType + "s",
                request.Classification,
                request.CardName);

            if (request.Classification == ClassificationTypes.Character || request.Classification == ClassificationTypes.Adventure)
            {
                var stringToRemove = request.Classification + "/";
                newMaterialAssetPath = newMaterialAssetPath.Replace(stringToRemove, "");
            }

            var material = new Material(Shader.Find("Diffuse"))
            {
                mainTexture = request.CardGraphic
            };
            
            if (AssetDatabase.GetAllAssetPaths().Contains(newMaterialAssetPath) == false)
            {
                var path = Path.GetDirectoryName(newMaterialAssetPath);
                if (path != null && !Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                AssetDatabase.CreateAsset(material, newMaterialAssetPath);
            }
            else
            {
                Debug.LogError(string.Format("Asset at {0} already exists!", newMaterialAssetPath));
                return null;
            }
            
            return material;
        }

        private static void AddChosenComponents(CreateCardRequest request, GameObject card)
        {
            if (request.AddLessonRequirement)
            {
                var newComponent = card.AddComponent<LessonRequirement>();
                newComponent.AmountRequired = request.LessonAmtRequired;
                newComponent.LessonType = request.LessonType;
            }

            if (request.AddCardLimit)
            {
                var newComponent = card.AddComponent<DeckCardLimitRequirement>();
                newComponent.MaximumAmountAllowed = request.MaxAllowedInDeck;
            }

            if (request.AddExistingScript)
            {
                var scriptType = request.AttachedScript.GetClass();

                var newComponent = (BaseCard) card.AddComponent(scriptType);
                newComponent.Classification = request.Classification;
                newComponent.Rarity = request.Rarity;
            }
        }

        private static void TryCreatePrefab(CreateCardRequest request, GameObject card)
        {
            string newPrefabAssetPath = string.Format("Assets/Prefabs/Resources/Cards/{0}/{1}/{2}.prefab",
                request.Classification,
                request.CardType == Type.Match ? "Matches" : request.CardType + "s",
                request.CardName);

            if (request.Classification == ClassificationTypes.Character || request.Classification == ClassificationTypes.Adventure)
            {
                var stringToRemove = request.Classification + "/";
                newPrefabAssetPath = newPrefabAssetPath.Replace(stringToRemove, "");
            }

            if (AssetDatabase.LoadAssetAtPath(newPrefabAssetPath, typeof (GameObject)))
            {
                string message = string.Format("A prefab already exists at\n{0}\nReplace?", newPrefabAssetPath);

                if (EditorUtility.DisplayDialog("Prefab already exists", message, "Yes", "No"))
                {
                    CreatePrefab(newPrefabAssetPath, card);
                }
            }
            else
            {
                CreatePrefab(newPrefabAssetPath, card);
            }
        }

        private static void CreatePrefab(string newPrefabAssetPath, GameObject card)
        {
            var path = Path.GetDirectoryName(newPrefabAssetPath);
            if (path != null && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Object empty = PrefabUtility.CreateEmptyPrefab(newPrefabAssetPath);
            var newPrefab = PrefabUtility.ReplacePrefab(card, empty);

            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(newPrefabAssetPath);
            EditorGUIUtility.PingObject(newPrefab);
            EditorUtility.FocusProjectWindow();
        }

        [UsedImplicitly]
        // ReSharper disable once InconsistentNaming
        public void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Create New Card", EditorStyles.boldLabel);

            ChooseCardTexture();
            ChooseCardProperties();
            ChooseAddLessonRequirement();
            ChooseAddCardLimitRequirement();
            ChooseScript();
            ValidateRequest();

            GUILayout.EndVertical();
        }

        private void ValidateRequest()
        {
            GUILayout.Space(10);
            if (!_cardRequest.IsValid) return;

            if (GUILayout.Button("Create Card"))
            {
                EditorApplication.delayCall += () => CreateCard(_cardRequest);
                Close();
            }
        }

        private void ChooseAddCardLimitRequirement()
        {
            GUILayout.Space(10);

            _cardRequest.AddCardLimit = EditorGUILayout.BeginToggleGroup("Add Card Limit", _cardRequest.AddCardLimit);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Maximum Allowed In Deck: ");
            _cardRequest.MaxAllowedInDeck = EditorGUILayout.IntField(_cardRequest.MaxAllowedInDeck);
            GUILayout.EndHorizontal();

            EditorGUILayout.EndToggleGroup();
        }

        private void ChooseAddLessonRequirement()
        {
            GUILayout.Space(10);

            _cardRequest.AddLessonRequirement = EditorGUILayout.BeginToggleGroup("Add Lesson Requirement", _cardRequest.AddLessonRequirement);
            
            _cardRequest.LessonType = (LessonTypes) EditorGUILayout.EnumPopup("Lesson Type: ", _cardRequest.LessonType);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Amount Required: ");
            _cardRequest.LessonAmtRequired = EditorGUILayout.IntField(_cardRequest.LessonAmtRequired);
            GUILayout.EndHorizontal();

            EditorGUILayout.EndToggleGroup();
        }

        private void ChooseCardProperties()
        {
            GUILayout.Space(10);

            _cardRequest.CardType = (Type) EditorGUILayout.EnumPopup("Card Type: ", _cardRequest.CardType);
            _cardRequest.Classification = (ClassificationTypes) EditorGUILayout.EnumPopup("Classification: ", _cardRequest.Classification);
            _cardRequest.Rarity = (Rarity)EditorGUILayout.EnumPopup("Rarity: ", _cardRequest.Rarity);
        }

        private void ChooseCardTexture()
        {
            GUILayout.Space(10);

            _cardRequest.CardGraphic =
                (Texture2D)
                    EditorGUILayout.ObjectField("Card Image:", _cardRequest.CardGraphic, typeof (Texture2D), false);
            if (_cardRequest.CardGraphic != null)
            {
                _cardRequest.CardName = _cardRequest.CardGraphic.name.Replace("_", "").Replace("'", "");
            }
        }

        private void ChooseScript()
        {
            GUILayout.Space(20);

            _cardRequest.AddExistingScript = EditorGUILayout.BeginToggleGroup("Add Existing BaseCard Script", _cardRequest.AddExistingScript);

            _cardRequest.AttachedScript =
                (MonoScript) EditorGUILayout.ObjectField("Script:", _cardRequest.AttachedScript, typeof (MonoScript), false);

            EditorGUILayout.EndToggleGroup();
        }
    }
}
