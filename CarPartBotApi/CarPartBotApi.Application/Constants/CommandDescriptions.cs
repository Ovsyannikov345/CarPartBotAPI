namespace CarPartBotApi.Application.Constants;

public static class CommandDescriptions
{
    public const string Start = "Initializes the bot for the current user";

    public const string Help = "Prints a list of commands that this bot can execute";

    public const string AddCar = "Adds a new car";

    public const string RemoveCar = "Removes a saved car";

    public const string ListCars = "Prints a list of saved cars";

    // Admin commands.
    public const string Users = "Prints a list users that are registered within the bot";
}
