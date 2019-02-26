// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Rémi Carreira - 2016
//
// Edited by Franck-Olivier FILIN - 2017
//
// Machine.cs

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[DisallowMultipleComponent]
public class Machine : MonoBehaviour
{
    #region Events

    public enum EVENTS
    {
        NONE,
        //DEFAULTS
        STORY_MACHINE,
        ITEM_TAKE,
        ITEM_IN_PLACE,
        TRUE_FALSE,
        LOCK_UNLOCK,
        OPEN_CLOSE,
        START_FINAL,
        ON_OFF,
        LEFT_RIGHT
    }

    [System.Serializable]
    public class EventsMachine
    {
        [SerializeField]
        public EVENTS _event = EVENTS.NONE;
        [SerializeField]
        public List<Action> _listActions = new List<Action>();
        public bool _actionListOpenned;

        [System.Serializable]
        public class Action
        {
            [SerializeField]
            public string _name;
            [SerializeField]
            public bool _value;
        }
    }

    [SerializeField]
    private List<EventsMachine> _events = new List<EventsMachine>();
    public List<EventsMachine> Events { get { return _events; } set { _events = value; } }

#endregion

    public bool SetConditionValue(EVENTS myEvent,  string name, bool value)
    {
        foreach (EventsMachine eventMachine in Events)
            if(null != eventMachine && eventMachine._event == myEvent)
                foreach(EventsMachine.Action action in eventMachine._listActions)
                    if (action._name == name)
                        return action._value = value;

        return false;
    }

    public bool GetConditionValue(EVENTS myEvent, string name)
    {
        foreach (EventsMachine eventMachine in Events)
            if (null != eventMachine && eventMachine._event == myEvent)
                foreach (EventsMachine.Action action in eventMachine._listActions)
                    if (action._name == name)
                        return action._value;

        return false;
    }

    public int GetIdEvent(EVENTS eventName)
    {
        foreach (EventsMachine myEvent in Events)
            if (myEvent._event == eventName)
                return Events.IndexOf(myEvent);

        return 0;
    }
}
