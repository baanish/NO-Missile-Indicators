namespace NoMissileIndicators.Tests;

internal static class Program
{
    private static int Main()
    {
        var tests = new List<(string Name, Action Test)>();
        tests.AddRange(ScreenEdgeTests.All);
        tests.AddRange(WingmanSphereTests.All);
        tests.AddRange(KnownPositionsTests.All);
        tests.AddRange(OldSettingsImportTests.All);
        tests.AddRange(PvpRuleTests.All);

        var failed = 0;
        foreach (var (name, test) in tests)
        {
            try
            {
                test();
                Console.WriteLine($"PASS {name}");
            }
            catch (Exception exception)
            {
                failed++;
                Console.WriteLine($"FAIL {name}: {exception.Message}");
            }
        }

        Console.WriteLine(failed == 0 ? $"All {tests.Count} tests passed." : $"{failed} of {tests.Count} tests failed.");
        return failed == 0 ? 0 : 1;
    }

    internal static void ExpectText(string actual, string expected) =>
        Expect(actual == expected, $"expected '{expected}', got '{actual}'");

    internal static void Expect(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
