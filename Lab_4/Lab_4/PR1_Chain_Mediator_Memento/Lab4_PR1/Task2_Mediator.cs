using System;
using System.Collections.Generic;

namespace Lab4_PR1.Task2
{
    public interface ICommandCentre
    {
        void RegisterAircraft(Aircraft aircraft);
        void RegisterRunway(Runway runway);
        bool RequestTakeoff(Aircraft aircraft);
        void NotifyRunwayFree(Runway runway);
    }

    public class CommandCentre : ICommandCentre
    {
        private List<Aircraft> _aircrafts = new List<Aircraft>();
        private List<Runway> _runways = new List<Runway>();

        public void RegisterAircraft(Aircraft aircraft)
        {
            _aircrafts.Add(aircraft);
        }

        public void RegisterRunway(Runway runway)
        {
            _runways.Add(runway);
        }

        public bool RequestTakeoff(Aircraft aircraft)
        {
            foreach (var runway in _runways)
            {
                if (!runway.IsBusy)
                {
                    runway.Occupy(aircraft);
                    Console.WriteLine($"{aircraft.Name} злітає з {runway.Name}");
                    return true;
                }
            }
            Console.WriteLine($"{aircraft.Name} чекає на вільну смугу");
            return false;
        }

        public void NotifyRunwayFree(Runway runway)
        {
            Console.WriteLine($"{runway.Name} тепер вільна");
        }
    }

    public class Aircraft
    {
        public string Name { get; private set; }
        private ICommandCentre _commandCentre;

        public Aircraft(string name, ICommandCentre centre)
        {
            Name = name;
            _commandCentre = centre;
            _commandCentre.RegisterAircraft(this);
        }

        public void RequestTakeoff()
        {
            _commandCentre.RequestTakeoff(this);
        }
    }

    public class Runway
    {
        public string Name { get; private set; }
        public bool IsBusy { get; private set; }
        private ICommandCentre _commandCentre;

        public Runway(string name, ICommandCentre centre)
        {
            Name = name;
            _commandCentre = centre;
            _commandCentre.RegisterRunway(this);
        }

        public void Occupy(Aircraft aircraft)
        {
            IsBusy = true;
            Console.WriteLine($"{Name} зайнята літаком {aircraft.Name}");
        }

        public void Free()
        {
            IsBusy = false;
            _commandCentre.NotifyRunwayFree(this);
        }
    }

    public static class MediatorDemo
    {
        public static void Run()
        {
            Console.WriteLine("\nЗавдання 2: Посередник\n");

            var centre = new CommandCentre();
            var runway1 = new Runway("Злітна смуга 01", centre);
            var runway2 = new Runway("Злітна смуга 02", centre);

            var boeing = new Aircraft("Boeing 737", centre);
            var airbus = new Aircraft("Airbus A320", centre);

            Console.WriteLine("Спроба зльоту");
            boeing.RequestTakeoff();
            airbus.RequestTakeoff();

            Console.WriteLine("\nЗвільнення смуги");
            runway1.Free();

            Console.WriteLine("\nПовторна спроба зльоту");
            airbus.RequestTakeoff();
        }
    }
}