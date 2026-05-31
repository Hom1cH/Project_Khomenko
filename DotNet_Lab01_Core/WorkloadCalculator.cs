namespace DotNet_Lab01_Core;

public static class WorkloadCalculator
{
    public static double CalculateImportance(int credits, int difficulty, DateTime createdAt, DateTime deadline)
    {
        int normalizedCredits = Math.Clamp(credits, 1, 100);
        int normalizedDifficulty = Math.Clamp(difficulty, 1, 100);
        double days = Math.Max(1, (deadline - createdAt).TotalDays);
        double urgency = 100 / (1 + days / 14);

        return Math.Round(
            normalizedCredits * 0.45 +
            normalizedDifficulty * 0.35 +
            urgency * 0.20,
            2);
    }
}
