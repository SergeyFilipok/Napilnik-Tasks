using System;
using System.IO;

public class Programm {
    public void Main() {
        ILoger fileLoger = new FileLogWritter();
        ILoger consoleLoger = new ConsoleLogWritter();
        ILoger fridayFileLoger = new FridayLogWritter(fileLoger);
        ILoger fridayConsoleLoger = new FridayLogWritter(consoleLoger);
        ILoger consoleFridayFileLogger = new MultiLogWritter(consoleLoger, fridayFileLoger);

        Pathfinder file = new Pathfinder(fileLoger);
        Pathfinder console = new Pathfinder(consoleLoger);
        Pathfinder fridayFile = new Pathfinder(fridayFileLoger);
        Pathfinder fridayConsole = new Pathfinder(fridayConsoleLoger);
        Pathfinder consoleFridayFile = new Pathfinder(consoleFridayFileLogger);
    }
}

public class Pathfinder {
    private ILoger _loger;

    public Pathfinder(ILoger loger) {
        _loger = loger;
    }

    public void Find() {
        string someMessage = "";
        _loger.WriteError(someMessage);
    }
}

public interface ILoger {
    void WriteError(string message);
}

public class ConsoleLogWritter : ILoger {
    public void WriteError(string message) {
        Console.WriteLine(message);
    }
}

public class FileLogWritter : ILoger {
    public void WriteError(string message) {
        File.WriteAllText("log.txt", message);
    }
}

public class FridayLogWritter : ILoger {
    private ILoger _loger;

    public FridayLogWritter(ILoger loger) {
        _loger = loger;
    }

    public void WriteError(string message) {
        if (DateTime.Now.DayOfWeek == DayOfWeek.Friday) {
            _loger.WriteError(message);
        }
    }
}

public class MultiLogWritter : ILoger {
    public ILoger[] _logers;

    public MultiLogWritter(params ILoger[] logers) {
        _logers = logers;
    }

    public void WriteError(string message) {
        foreach (var loger in _logers) {
            loger.WriteError(message);
        }
    }
}
