# UnitySourceGenerator

## How to use in Unity

1. Import the .dll into your unity project
2. Select the file
3. Go to Select platforms for plugin and disable Any Platform.
4. Go to Include Platforms and disable Editor and Standalone.
5. Add the `RoslynAnalyzer` in the Asset Labels
6. Add the `[NotifyChange]` attribute to a field.
7. Implement the `OnXXXChanged(newValue)` and `OnXXXChanging(oldValue, newValue)`.

Example:
```cs
using UnitySourceGenerators;

public partial class Counter : MonoBehaviour
{
    [SerializeField, NotifyChange]
    private int number = 0;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(Increase());
    }

    private IEnumerator Increase() {
        while (true) {
            Number++;
            yield return new WaitForSeconds(1);
        }
    }

    partial void OnNumberChanged(int oldValue, int newValue) {
        Debug.Log($"Number changed from {oldValue} to {newValue}");
    }
}
```

More information can be found [here](https://docs.unity3d.com/Manual/roslyn-analyzers.html).
