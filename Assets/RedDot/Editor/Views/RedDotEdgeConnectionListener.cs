using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace RedDot.Editor.Views
{
    public class RedDotEdgeConnectionListener : IEdgeConnectorListener
    {
        public RedDotEdgeConnectionListener()
        {
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            Debug.Log("=----HelloEdgeConnectionListener-----OnDropOutsidePort-");
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            Debug.Log("=----HelloEdgeConnectionListener-----OnDrop-");
        }
    }
}