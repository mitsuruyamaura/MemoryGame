using System;
using System.Collections.Generic;

[Serializable]
public class EventDetailMasterData {
    public int event_detail_id;
    public string help;
    public int element;
    public List<EventElement> list;
}

[Serializable]
public class EventElement {
    public int id;
    public string type;
    public string text_name;
    public string text_dialog_1;
}