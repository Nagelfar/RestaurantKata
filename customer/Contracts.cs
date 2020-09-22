using System;
using Api.GuestExperience.Model;

namespace Customer
{
    public abstract class Event    
    { 
        DateTime On { get ; set; } = DateTime.Now;   
    }

    public class MenuRetrieved : Event
    {
        public Menu Menu {get;set;}
    }
}