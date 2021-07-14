using SolidWorks.Interop.swcommands;

namespace SldWorksLookup.Model
{
    public class SwCommand
    {
        public SwCommand(int command, int userCommand)
        {
            UserCommand = userCommand;
            Command = (swCommands_e)command;
        }

        public swCommands_e Command { get; set; }

        public int UserCommand { get; set; }

        public override string ToString()
        {
            return $"Command : {Command},  UserCommand : {UserCommand}";
        }
    }
}
