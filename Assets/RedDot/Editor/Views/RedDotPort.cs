using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace RedDot.Editor.Views
{
    public class RedDotPort : Port
    {
        RedDotPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type)
            : base(portOrientation, portDirection, portCapacity, type)
        {
            
        }


        public static Port Create(Direction portDirection, IEdgeConnectorListener connectorListener)
        {
            var port = new RedDotPort(Orientation.Horizontal, portDirection, Capacity.Multi, typeof(RedDotPort))
            {
                m_EdgeConnector = new EdgeConnector<Edge>(connectorListener),
            };
            port.AddManipulator((IManipulator) port.m_EdgeConnector);
//            port.Slot = logicSlot;
            return port;
        }
    }
}