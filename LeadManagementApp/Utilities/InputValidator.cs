namespace LeadManagementSystem.Utilities;

public static class InputValidator
{
    public static string GetRequiredString(string prompt)
    {
        string? input;
        do
        {
            Console.Write(prompt);
            input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Error: This field is required.");
            }
        } while (string.IsNullOrWhiteSpace(input));

        return input;
    }

    public static int GetValidInt(string prompt)
    {
        int result;
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out result))
            {
                return result;
            }

            Console.WriteLine("Error: Please enter a valid number.");
        }
    }
}
