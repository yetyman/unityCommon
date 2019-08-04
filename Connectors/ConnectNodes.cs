using Assets.CommonLibrary.GenericClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class IndexToIndex
{
    public delegate void IndexChangedEvent(IndexToIndex sender, int oldValue);
    public event IndexChangedEvent AChanged;
    public event IndexChangedEvent BChanged;
    [SerializeField]
    private int a;
    [SerializeField]
    private int b;
    public int A
    {
        get => a;
        set
        {
            var old = a;
            a = value;
            if(old != value)
                AChanged?.Invoke(this,old);
        }
    }
    public int B
    {
        get => b;
        set
        {
            var old = b;
            b = value;
            if(old != value)
                BChanged?.Invoke(this, b);
        }
    }

    public override int GetHashCode()
    {
        return a.GetHashCode() * 17 + b.GetHashCode();
    }
}
public class ConnectNodes : MonoBehaviour
{
    public Transform ConnectedNodesTransform;
    public GameObject ConnectorPrefab;
    private Dictionary<IndexToIndex, Connector> Connectors = new Dictionary<IndexToIndex, Connector>();
    [SerializeField]
    private List<IndexToIndex> _connections = new EventedList<IndexToIndex>();
    public EventedList<IndexToIndex> Connections
    {
        get { return (EventedList<IndexToIndex>)_connections; }
        set { _connections = new EventedList<IndexToIndex>(value); }
    }
        
    // Start is called before the first frame update
    void Start()
    {
        Connections.Added += Connections_Added;
        Connections.Removed += Connections_Removed;
        foreach (IndexToIndex connection in Connections)
            AddConnection(connection);
    }

    private void Connections_Added(EventedList<IndexToIndex> sender, IndexToIndex item, int index)
    {
        AddConnection(item);
    }
    private void Connections_Removed(EventedList<IndexToIndex> sender, IndexToIndex item, int index)
    {
        RemoveConnection(item);
    }

    private void ModifyConnection(IndexToIndex old, IndexToIndex newConnection){
        RemoveConnection(old);
        AddConnection(newConnection);
    }
    private void RemoveConnection(IndexToIndex item)
    {
        Destroy(Connectors[item]);//animate this later?
        Connectors.Remove(item);
    }
    private void AddConnection(IndexToIndex item)
    {
        item.AChanged += Item_AChanged;
        item.BChanged += Item_BChanged;
        var connector = Instantiate(ConnectorPrefab, transform).GetComponent<Connector>();
        connector.Begin = GetGameObjectAt(item.A);
        connector.End = GetGameObjectAt(item.B);
        Connectors.Add(item, connector);
    }

    private void Item_AChanged(IndexToIndex sender, int oldValue)
    {
        ModifyConnection(new IndexToIndex() { A = oldValue, B = sender.B }, sender);
    }
    private void Item_BChanged(IndexToIndex sender, int oldValue)
    {
        ModifyConnection(new IndexToIndex() { A = sender.A, B = oldValue }, sender);
    }

    private GameObject GetGameObjectAt(int index)
    {
        return ConnectedNodesTransform.GetChild(index).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //if indexes updated or connected indexes updated, then move every connector to its new proper place.
    }
}
