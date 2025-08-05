using System;
using System.Collections.Generic;
using System.Linq;
using DigiTalino_Plugin;
using FlaxEngine;

namespace Game;

/// <summary>
/// MessManager Manager. This class follows the singleton pattern!
/// </summary>
[Category("Manager")]
public class MessManager : InstanceManagerScript
{

    [ShowInEditor, Serialize] private Actor _officeDesktopPositions;
    [ShowInEditor, Serialize] private Prefab _dirtyItemPrefab;

    [ShowInEditor, Serialize] private List<Model> _dirtyItemModels;

    [ShowInEditor, Serialize] private BezierCurve<float> _bezierCurve;

    private Actor[] _spawnPositions;

    public event Action OnFloorCleaned;

    private int _dirtyItems;

    /// <inheritdoc/>
    public override void OnAwake()
    {
        base.OnAwake(); // Do not remove since it is required

        if (_officeDesktopPositions == null || _officeDesktopPositions.ChildrenCount <= 0)
            Debug.LogError("Office desktop positions are null or empty");

        _spawnPositions = _officeDesktopPositions.Children;
    }

    public override void OnStart()
    {
        SingletonManager.Get<FloorManager>().OnFloorChangeRequested += OnFloorChanged;
    }

    public override void OnDisable()
    {
        SingletonManager.Get<FloorManager>().OnFloorChangeRequested -= OnFloorChanged;
        OnFloorCleaned = null;
        base.OnDisable();
    }

    private void OnFloorChanged(int floor)
    {
        if (_bezierCurve == null)
        {
            Debug.LogError("Bezier curve is null");
            return;
        }

        _bezierCurve.Evaluate(out float result, floor);
        int numberOfDirtyItems = Mathf.Clamp(Mathf.RoundToInt(result), 0, _spawnPositions.Length);

        if (_dirtyItemModels == null || _dirtyItemModels.Count == 0)
        {
            Debug.LogError("No dirty item models assigned.");
            return;
        }

        List<Actor> shuffled = new(_spawnPositions);
        Shuffle(shuffled);

        Random random = new Random();

        for (int i = 0; i < numberOfDirtyItems; i++)
        {
            Actor spawnPoint = shuffled[i];
            Vector3 position = spawnPoint.Transform.LocalToWorldVector(spawnPoint.Position);


            // Spawn prefab at position
            Actor instance = PrefabManager.SpawnPrefab(_dirtyItemPrefab, position);
            if (instance.Children.Length <= 0)
                Debug.LogError("Dirty item prefab has no children.");

            if (!instance.Children[0].TryGetScript(out IInteract interactScript))
                Debug.LogError("Dirty item prefab has no interact script.");

            interactScript.OnInteracted += OnDirtyItemInteracted;
            _dirtyItems++;

            // Get a random model from the list
            int randomIndex = random.Next(_dirtyItemModels.Count);
            Model selectedModel = _dirtyItemModels[randomIndex];

            StaticModel staticModel = new()
            {
                Model = selectedModel,

            };

            Level.SpawnActor(staticModel, instance);
            staticModel.LocalPosition = Vector3.Zero;
        }
    }

    private void OnDirtyItemInteracted(Actor instigator)
    {
        _dirtyItems--;

        if (_dirtyItems > 0)
            return;
        OnFloorCleaned?.Invoke();
    }

    private static void Shuffle<T>(IList<T> list)
    {
        Random random = new Random();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

}